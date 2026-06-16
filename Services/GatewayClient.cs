using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class GatewayClient : IGatewayClient
{
    private readonly AbdmConfig _config;
    private readonly ILogger<GatewayClient> _logger;
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);
    private string? _cachedToken;
    private DateTime _tokenExpiryTime = DateTime.MinValue;

    public GatewayClient(IOptions<AbdmConfig> config, ILogger<GatewayClient> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();
        if (_config.UseProxySettings && !string.IsNullOrEmpty(_config.ProxyHost))
        {
            handler.Proxy = new WebProxy(_config.ProxyHost, _config.ProxyPort);
            handler.UseProxy = true;
        }

        var client = new HttpClient(handler);
        
        string baseUrl = _config.Gateway.BaseUrl;
        if (string.IsNullOrEmpty(baseUrl))
        {
            // Fallback to sandbox or production based on environment
            baseUrl = _config.Environment.Equals("prod", StringComparison.OrdinalIgnoreCase) 
                ? _config.Gateway.ProductionBaseUrl 
                : _config.Gateway.SandboxBaseUrl;
        }

        if (!string.IsNullOrEmpty(baseUrl))
        {
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            client.BaseAddress = new Uri(baseUrl);
        }
        
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiryTime)
        {
            return _cachedToken;
        }

        await _tokenSemaphore.WaitAsync();
        try
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiryTime)
            {
                return _cachedToken;
            }

            var requestBody = new CreateSessionRequest
            {
                ClientId = _config.ClientId,
                ClientSecret = _config.ClientSecret,
                GrantType = "client_credentials"
            };

            using var client = CreateHttpClient();
            client.DefaultRequestHeaders.Add("X-CM-ID", _config.Environment);
            client.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("TIMESTAMP", Utils.GetCurrentTimeStamp());

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var sessionPath = _config.Gateway.CreateSessionPath?.TrimStart('/') ?? "";
            if (client.BaseAddress != null && client.BaseAddress.AbsolutePath.EndsWith("/gateway/") && sessionPath.StartsWith("gateway/"))
            {
                sessionPath = sessionPath["gateway/".Length..];
            }
            var fullUrl = client.BaseAddress != null ? new Uri(client.BaseAddress, sessionPath).ToString() : sessionPath;
            _logger.LogInformation($"Requesting new access token from gateway full URL: {fullUrl}");
            var response = await client.PostAsync(sessionPath, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(responseString);
            if (sessionResponse == null || string.IsNullOrEmpty(sessionResponse.AccessToken))
            {
                throw new Exception("Session creation returned empty access token");
            }

            _cachedToken = "Bearer " + sessionResponse.AccessToken;
            
            int expiresInSeconds = sessionResponse.ExpiresIn > 0 ? sessionResponse.ExpiresIn : 1200;
            _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresInSeconds - 120);

            _logger.LogInformation("Successfully refreshed gateway access token.");
            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start/refresh gateway session");
            throw;
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    public async Task<GenericV3Response?> PostToGatewayAsync<T>(string path, T body, Dictionary<string, string>? customHeaders = null)
    {
        try
        {
            var token = await GetAccessTokenAsync();
            using var client = CreateHttpClient();
            
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("X-CM-ID", _config.Environment);

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (client.DefaultRequestHeaders.Contains(header.Key))
                    {
                        client.DefaultRequestHeaders.Remove(header.Key);
                    }
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var json = JsonSerializer.Serialize(body);

            var cleanPath = path?.TrimStart('/') ?? "";
            if (client.BaseAddress != null && client.BaseAddress.AbsolutePath.EndsWith("/gateway/") && cleanPath.StartsWith("gateway/"))
            {
                cleanPath = cleanPath["gateway/".Length..];
            }
            if (_config.LogCurl)
            {
                var fullUrl = client.BaseAddress != null ? new Uri(client.BaseAddress, cleanPath).ToString() : cleanPath;
                LogCurlRequest(fullUrl, json, client.DefaultRequestHeaders);
            }

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(cleanPath, content);
            
            var responseString = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Gateway call status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(responseString))
                {
                    return new GenericV3Response { HttpStatus = "OK" };
                }
                return JsonSerializer.Deserialize<GenericV3Response>(responseString);
            }

            return new GenericV3Response
            {
                HttpStatus = response.StatusCode.ToString(),
                Status = "Error",
                Message = responseString
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling Gateway path: {path}");
            return new GenericV3Response
            {
                HttpStatus = "InternalServerError",
                Status = "Error",
                Message = ex.Message
            };
        }
    }

    private void LogCurlRequest(string url, string jsonBody, System.Net.Http.Headers.HttpRequestHeaders headers)
    {
        var sb = new StringBuilder();
        sb.Append($"curl -X POST \"{url}\"");
        foreach (var header in headers)
        {
            sb.Append($" -H \"{header.Key}: {string.Join(", ", header.Value)}\"");
        }
        sb.Append($" -d '{jsonBody}'");
        _logger.LogInformation($"[CURL] {sb}");
    }
}
