using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var abdmConfigSection = builder.Configuration.GetSection("AbdmConfig");
builder.Services.Configure<AbdmConfig>(abdmConfigSection);
var abdmConfig = abdmConfigSection.Get<AbdmConfig>() ?? new AbdmConfig();

bool isEfCore = abdmConfig.DatabaseProvider?.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) == true 
             || abdmConfig.DatabaseProvider?.Equals("Postgres", StringComparison.OrdinalIgnoreCase) == true;

if (isEfCore)
{
    builder.Services.AddDbContext<AppDbContext>();
    builder.Services.AddScoped<IRequestLogV3Service, SqlServerRequestLogV3Service>();
    builder.Services.AddScoped<IPatientV3Service, SqlServerPatientV3Service>();
    builder.Services.AddScoped<ILinkTokenService, SqlServerLinkTokenService>();
    builder.Services.AddScoped<IConsentPatientV3Service, SqlServerConsentPatientV3Service>();
}
else
{
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddScoped<IRequestLogV3Service, RequestLogV3Service>();
    builder.Services.AddScoped<IPatientV3Service, PatientV3Service>();
    builder.Services.AddScoped<ILinkTokenService, LinkTokenService>();
    builder.Services.AddScoped<IConsentPatientV3Service, ConsentPatientV3Service>();
}

builder.Services.AddScoped<IWorkflowV3Manager, WorkflowV3Manager>();
builder.Services.AddSingleton<ICryptographyService, CryptographyService>();
builder.Services.AddSingleton<IGatewayClient, GatewayClient>();
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AbdmConfig>>().Value;
    var settings = new ABDM.Models.AbdmSettings
    {
        ClientId = config.ClientId,
        ClientSecret = config.ClientSecret,
        HipId = !string.IsNullOrEmpty(config.HipId) ? config.HipId : (config.HipSetup?.BaseUrl ?? string.Empty),
        HipName = !string.IsNullOrEmpty(config.HipName) ? config.HipName : "MIDHA HOSPITAL",
        Environment = config.Environment ?? "Sandbox",
        CmId = "sbx",
        BaseUrl = config.Gateway?.BaseUrl?.Contains("/hiecm/gateway") == true 
            ? config.Gateway.BaseUrl 
            : (config.Gateway?.BaseUrl ?? "https://dev.abdm.gov.in/api").TrimEnd('/') + "/hiecm/gateway",
        AbhaServiceUrl = !string.IsNullOrEmpty(config.AbhaServiceUrl) 
            ? config.AbhaServiceUrl 
            : (config.Environment?.ToLower() == "production" 
                ? "https://abha.abdm.gov.in/abha/api/v3" 
                : "https://abhasbx.abdm.gov.in/abha/api/v3")
    };
    return new ABDM.Api.AbdmApiClient(settings);
});

// HIP Workflow Services (real implementations — migrated from Java)
builder.Services.AddScoped<IDiscoveryV3Service, DiscoveryV3Service>();
builder.Services.AddScoped<ILinkV3Service, LinkV3Service>();
builder.Services.AddScoped<IHIPLinkV3Service, HIPLinkV3Service>();
builder.Services.AddScoped<IConsentV3Service, ConsentV3Service>();
builder.Services.AddScoped<IHIPHealthInformationV3Service, HIPHealthInformationV3Service>();
builder.Services.AddScoped<IProfileShareV3Service, ProfileShareV3Service>();
builder.Services.AddScoped<IDeepLinkingV3Service, DeepLinkingV3Service>();
builder.Services.AddScoped<IFhirMapperService, FhirMapperService>();

// HIU Services
builder.Services.AddScoped<HIUConsentV3Service>();
builder.Services.AddScoped<HIUHealthInformationV3Service>();
builder.Services.AddScoped<HIUSubscriptionV3Service>();

// Consent Request Mapping Service (tracks consentRequestId ↔ gatewayRequestId for HIU callbacks)
if (isEfCore)
    builder.Services.AddScoped<IConsentRequestMappingService, SqlServerConsentRequestMappingService>();
else
    builder.Services.AddSingleton<IConsentRequestMappingService, MongoConsentRequestMappingService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAll");

// Auto-initialize SQL Server Database Schema if configured
if (isEfCore)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "ABDM Wrapper API V1");
    options.RoutePrefix = "swagger";
});

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Json(new { status = "Healthy", service = "ABDM Wrapper .NET Web API" }));

app.Run();
