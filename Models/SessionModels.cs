using System.Text.Json.Serialization;

namespace AbdmWrapperNet.Models;

public class CreateSessionRequest
{
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("clientSecret")]
    public string ClientSecret { get; set; } = string.Empty;

    [JsonPropertyName("grantType")]
    public string GrantType { get; set; } = "client_credentials";
}

public class CreateSessionResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refreshExpiresIn")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = string.Empty;
}
