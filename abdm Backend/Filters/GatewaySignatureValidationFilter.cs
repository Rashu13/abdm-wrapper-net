using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AbdmWrapperNet.Configuration;

namespace AbdmWrapperNet.Filters;

/// <summary>
/// Attribute to skip Gateway Signature Validation for specific actions (e.g., peer-to-peer data transfers).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SkipGatewaySignatureValidationAttribute : Attribute
{
}

/// <summary>
/// Action Filter to inspect and validate the ABDM Gateway signature on callback endpoints.
/// </summary>
public class GatewaySignatureValidationFilter : IAsyncAuthorizationFilter
{
    private readonly AbdmConfig _abdmConfig;
    private readonly ILogger<GatewaySignatureValidationFilter> _logger;
    private static JsonWebKeySet? _cachedJwks;
    private static DateTime _nextRefresh = DateTime.MinValue;
    private static readonly object _lock = new();

    public GatewaySignatureValidationFilter(
        IOptions<AbdmConfig> abdmConfig,
        ILogger<GatewaySignatureValidationFilter> logger)
    {
        _abdmConfig = abdmConfig.Value;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check if signature validation is explicitly disabled in settings
        if (_abdmConfig.DisableGatewaySignatureValidation == true)
        {
            _logger.LogWarning("Gateway Signature Validation is bypassed because DisableGatewaySignatureValidation is set to true.");
            return;
        }

        // Allow actions to skip validation via SkipGatewaySignatureValidationAttribute
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is SkipGatewaySignatureValidationAttribute))
        {
            _logger.LogDebug("Skipping Gateway Signature Validation for endpoint: {Path}", context.HttpContext.Request.Path);
            return;
        }

        var request = context.HttpContext.Request;

        // 1. Inspect the Authorization header
        if (!request.Headers.TryGetValue("Authorization", out var authHeaderValues) || 
            string.IsNullOrEmpty(authHeaderValues.ToString()))
        {
            _logger.LogWarning("Gateway Callback Signature Verification Failed: Missing Authorization header on path {Path}.", request.Path);
            context.Result = new UnauthorizedObjectResult(new { error = "Missing Authorization header" });
            return;
        }

        var authHeader = authHeaderValues.ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Gateway Callback Signature Verification Failed: Authorization header is not a Bearer token on path {Path}.", request.Path);
            context.Result = new UnauthorizedObjectResult(new { error = "Authorization header must be Bearer token" });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Gateway Callback Signature Verification Failed: Bearer token is empty on path {Path}.", request.Path);
            context.Result = new UnauthorizedObjectResult(new { error = "Bearer token is empty" });
            return;
        }

        try
        {
            // 2. Retrieve public certificates from NHA Gateway certs endpoint
            var jwks = await GetJwksAsync();
            if (jwks == null || jwks.Keys == null || jwks.Keys.Count == 0)
            {
                _logger.LogError("Gateway Callback Signature Verification Failed: Unable to fetch or parse JWKS keys from NHA Gateway.");
                context.Result = new StatusCodeResult(502); // Bad Gateway
                return;
            }

            // 3. Validate the JWT token signature using the retrieved keys
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "gateway", "https://dev.abdm.gov.in", "https://apis.abdm.gov.in" },
                ValidateAudience = true,
                ValidAudience = _abdmConfig.ClientId, // Matches client ID (Bridge ID)
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = jwks.GetSigningKeys(),
                ClockSkew = TimeSpan.FromMinutes(5) // Allowance for clock skew
            };

            var handler = new JwtSecurityTokenHandler();
            
            // ValidateToken will throw an exception if validation fails
            handler.ValidateToken(token, validationParameters, out var validatedToken);
            
            _logger.LogInformation("Gateway Callback Signature Verification Succeeded for request path: {Path}", request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gateway Callback Signature Verification Failed for path {Path}: {Message}", request.Path, ex.Message);
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid token signature", details = ex.Message });
        }
    }

    private async Task<JsonWebKeySet?> GetJwksAsync()
    {
        if (_cachedJwks != null && DateTime.UtcNow < _nextRefresh)
        {
            return _cachedJwks;
        }

        string baseUrl = _abdmConfig.Gateway?.BaseUrl ?? string.Empty;
        if (string.IsNullOrEmpty(baseUrl))
        {
            baseUrl = _abdmConfig.Environment?.Equals("production", StringComparison.OrdinalIgnoreCase) == true
                ? "https://apis.abdm.gov.in/api"
                : "https://dev.abdm.gov.in/api";
        }

        // Clean up trailing "/hiecm/gateway" suffix if present to get raw endpoint root
        if (baseUrl.EndsWith("/hiecm/gateway", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - "/hiecm/gateway".Length);
        }
        else if (baseUrl.EndsWith("/hiecm/gateway/", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - "/hiecm/gateway/".Length);
        }

        baseUrl = baseUrl.TrimEnd('/');
        string certsUrl = $"{baseUrl}/hiecm/gateway/v3/certs";

        try
        {
            _logger.LogInformation("Fetching NHA Gateway public certificates from: {Url}", certsUrl);
            using var httpClient = new HttpClient();
            var jwksJson = await httpClient.GetStringAsync(certsUrl);
            
            var jwks = new JsonWebKeySet(jwksJson);
            
            lock (_lock)
            {
                _cachedJwks = jwks;
                _nextRefresh = DateTime.UtcNow.AddHours(6); // Cache for 6 hours
            }

            return _cachedJwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching JWKS keys from NHA Gateway certs endpoint {Url}.", certsUrl);
            // If we have cached keys, return them as a fallback even if expired
            if (_cachedJwks != null)
            {
                _logger.LogWarning("Using expired cached JWKS keys as fallback.");
                return _cachedJwks;
            }
            throw;
        }
    }
}
