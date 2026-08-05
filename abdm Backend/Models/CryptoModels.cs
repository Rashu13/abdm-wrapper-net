using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AbdmWrapperNet.Models;

public class EncryptionKeys
{
    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
}

public class HealthInformationBundle
{
    [JsonPropertyName("careContextReference")]
    public string CareContextReference { get; set; } = string.Empty;

    [JsonPropertyName("bundleContent")]
    public string BundleContent { get; set; } = string.Empty;
}

public class EncryptionResponse
{
    [JsonPropertyName("healthInformationBundles")]
    public List<HealthInformationBundle> HealthInformationBundles { get; set; } = new();

    [JsonPropertyName("keyToShare")]
    public string KeyToShare { get; set; } = string.Empty;

    [JsonPropertyName("senderNonce")]
    public string SenderNonce { get; set; } = string.Empty;
}
