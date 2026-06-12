using System.Text.Json.Serialization;

namespace AbdmWrapperNet.Models;

public class CareContext
{
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    [JsonPropertyName("display")]
    public string Display { get; set; } = string.Empty;

    [JsonPropertyName("hiType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HiType { get; set; }

    [JsonIgnore]
    public bool IsLinked { get; set; }
}
