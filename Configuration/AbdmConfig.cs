namespace AbdmWrapperNet.Configuration;

public class AbdmConfig
{
    public string Environment { get; set; } = "sbx";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string HipId { get; set; } = string.Empty;
    public string HipName { get; set; } = string.Empty;
    public string AbhaServiceUrl { get; set; } = string.Empty;
    public bool LogCurl { get; set; }
    public bool UseProxySettings { get; set; }
    public string ProxyHost { get; set; } = string.Empty;
    public int ProxyPort { get; set; } = 8080;
    
    public string DatabaseProvider { get; set; } = "MongoDb"; // "MongoDb", "SqlServer", or "Postgres"
    
    public MongoDbConfig MongoDb { get; set; } = new();
    public SqlServerConfig SqlServer { get; set; } = new();
    public PostgresConfig Postgres { get; set; } = new();
    public GatewayConfig Gateway { get; set; } = new();
    public HipSetupConfig HipSetup { get; set; } = new();
    public HiuSetupConfig HiuSetup { get; set; } = new();
}

public class PostgresConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class SqlServerConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class MongoDbConfig
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "abdm_wrapper";
}

public class GatewayConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SandboxBaseUrl { get; set; } = string.Empty;
    public string ProductionBaseUrl { get; set; } = string.Empty;
    public string CreateSessionPath { get; set; } = string.Empty;
    public string ProfileOnSharePath { get; set; } = string.Empty;
    public string GenerateLinkTokenPath { get; set; } = string.Empty;
    public string AddCareContextsPath { get; set; } = string.Empty;
    public string LinkContextNotifyPath { get; set; } = string.Empty;
    public string DeepLinkingSmsNotifyPath { get; set; } = string.Empty;
    public string OnDiscoverPath { get; set; } = string.Empty;
    public string OnInitLinkPath { get; set; } = string.Empty;
    public string OnConfirmLinkPath { get; set; } = string.Empty;
    public string ConsentOnNotifyPath { get; set; } = string.Empty;
    public string HealthInformationOnRequestPath { get; set; } = string.Empty;
    public string HealthInformationPushNotificationPath { get; set; } = string.Empty;
    public string ConsentInitPath { get; set; } = string.Empty;
    public string ConsentStatusPath { get; set; } = string.Empty;
    public string ConsentHiuOnNotifyPath { get; set; } = string.Empty;
    public string FetchConsentPath { get; set; } = string.Empty;
    public string HealthInformationConsentManagerPath { get; set; } = string.Empty;
}

public class HipSetupConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string GetPatientPath { get; set; } = string.Empty;
    public string PatientDiscoverPath { get; set; } = string.Empty;
    public string GetPatientCareContextsPath { get; set; } = string.Empty;
    public string GetHealthInformationPath { get; set; } = string.Empty;
    public string ShareProfilePath { get; set; } = string.Empty;
    public string RequestOtpPath { get; set; } = string.Empty;
    public string VerifyOtpPath { get; set; } = string.Empty;
}

public class HiuSetupConfig
{
    public string HiuId { get; set; } = string.Empty;
    public string DataPushUrl { get; set; } = string.Empty;
    public string ConsentInitPath { get; set; } = "api/v3/consent/request/init";
    public string ConsentStatusPath { get; set; } = "api/v3/consent/request/status";
    public string FetchConsentPath { get; set; } = "api/v3/consent/fetch";
    public string ConsentOnNotifyPath { get; set; } = "api/v3/hiu/consent/request/on-notify";
    public string HealthInformationRequestPath { get; set; } = "api/v3/health-information/cm/request";
    public string HealthInformationPushNotificationPath { get; set; } = "api/v3/health-information/notify";
}
