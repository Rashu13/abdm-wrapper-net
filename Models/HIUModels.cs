using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AbdmWrapperNet.Models;

// ─── Consent Init Request ───────────────────────────────────────────────────

public class InitConsentRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("consent")]
    public ConsentBody? Consent { get; set; }
}

public class ConsentBody
{
    [JsonPropertyName("purpose")]
    public ConsentPurpose? Purpose { get; set; }

    [JsonPropertyName("patient")]
    public ConsentPatientRef? Patient { get; set; }

    [JsonPropertyName("hiu")]
    public ConsentEntityRef? Hiu { get; set; }

    [JsonPropertyName("hip")]
    public ConsentEntityRef? Hip { get; set; }

    [JsonPropertyName("requester")]
    public ConsentRequester? Requester { get; set; }

    [JsonPropertyName("hiTypes")]
    public List<string> HiTypes { get; set; } = new();

    [JsonPropertyName("permission")]
    public ConsentPermission? Permission { get; set; }
}

public class ConsentPurpose
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("refUri")]
    public string RefUri { get; set; } = string.Empty;
}

public class ConsentPatientRef
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public class ConsentEntityRef
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class ConsentPermission
{
    [JsonPropertyName("accessMode")]
    public string AccessMode { get; set; } = string.Empty;

    // DateRange, DataEraseAt, Frequency reuse MongoModels.cs types
    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }

    [JsonPropertyName("dataEraseAt")]
    public string DataEraseAt { get; set; } = string.Empty;

    [JsonPropertyName("frequency")]
    public ConsentFrequency? Frequency { get; set; }
}

// DateRange class is defined in MongoModels.cs and reused here

public class ConsentFrequency
{
    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("repeats")]
    public int Repeats { get; set; }
}

// ─── HIU Consent Gateway Callbacks ──────────────────────────────────────────

public class ConsentOnInitV3Request
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("consentRequest")]
    public ConsentRequestId? ConsentRequest { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class ConsentRequestId
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public class HIUConsentOnStatusV3Request
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("consentRequest")]
    public ConsentStatusDetail? ConsentRequest { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class ConsentStatusDetail
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentArtefacts")]
    public List<ConsentArtefact> ConsentArtefacts { get; set; } = new();
}

public class ConsentArtefact
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("lastUpdated")]
    public string LastUpdated { get; set; } = string.Empty;

    [JsonPropertyName("hipId")]
    public string HipId { get; set; } = string.Empty;

    [JsonPropertyName("careContextReference")]
    public List<string> CareContextReference { get; set; } = new();
}

public class NotifyHIURequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("notification")]
    public HIUNotification? Notification { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class HIUNotification
{
    [JsonPropertyName("consentRequestId")]
    public string ConsentRequestId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentArtefacts")]
    public List<ConsentArtefact> ConsentArtefacts { get; set; } = new();
}

public class OnFetchV3Request
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("consent")]
    public FetchedConsent? Consent { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class FetchedConsent
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentDetail")]
    public ConsentDetail? ConsentDetail { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

// ConsentDetail is defined in MongoModels.cs - reused here
// CareContextRef is defined in MongoModels.cs as ConsentCareContexts

// ─── HIU Health Information ──────────────────────────────────────────────────

public class HIUClientHealthInformationRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("consentId")]
    public string ConsentId { get; set; } = string.Empty;

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }
}

public class HIUHealthInformationOnRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("hiRequest")]
    public HIUHiRequest? HiRequest { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class HIUHiRequest
{
    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("sessionStatus")]
    public string SessionStatus { get; set; } = string.Empty;
}

// Encrypted FHIR health information pushed by HIP
public class HealthInformationPushRequest
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }

    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("entries")]
    public List<HealthInformationEntry> Entries { get; set; } = new();

    [JsonPropertyName("keyMaterial")]
    public HealthInformationKeyMaterial? KeyMaterial { get; set; }
}

public class HealthInformationEntry
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("media")]
    public string Media { get; set; } = string.Empty;

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = string.Empty;

    [JsonPropertyName("careContextReference")]
    public string CareContextReference { get; set; } = string.Empty;
}

// HIU Consent on-notify acknowledgement sent back to gateway
public class ConsentOnNotifyV3Request
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("acknowledgement")]
    public List<ConsentAcknowledgementItem> Acknowledgement { get; set; } = new();

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class ConsentAcknowledgementItem
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentId")]
    public string ConsentId { get; set; } = string.Empty;
}

// ─── HIU Consent Status Response ─────────────────────────────────────────────

public class ConsentStatusV3Response
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentDetails")]
    public FacadeConsentDetails? ConsentDetails { get; set; }

    [JsonPropertyName("errors")]
    public List<ErrorV3Response>? Errors { get; set; }

    [JsonIgnore]
    public int HttpStatusCode { get; set; } = 200;
}

public class FacadeConsentDetails
{
    [JsonPropertyName("grantedOn")]
    public string GrantedOn { get; set; } = string.Empty;

    [JsonPropertyName("deniedOn")]
    public string DeniedOn { get; set; } = string.Empty;

    [JsonPropertyName("dataEraseAt")]
    public string DataEraseAt { get; set; } = string.Empty;

    [JsonPropertyName("hiTypes")]
    public List<string> HiTypes { get; set; } = new();

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }

    [JsonPropertyName("consent")]
    public List<ConsentStatus> Consent { get; set; } = new();
}

public class ConsentStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentArtefacts")]
    public List<ConsentArtefact> ConsentArtefacts { get; set; } = new();
}

// ─── HIU Health Info Response (from facade status endpoint) ──────────────────

public class HealthInformationV3Response
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("decryptedHealthInformation")]
    public List<DecryptedEntry>? DecryptedHealthInformation { get; set; }

    [JsonPropertyName("errors")]
    public List<ErrorV3Response>? Errors { get; set; }

    [JsonIgnore]
    public int HttpStatusCode { get; set; } = 200;
}

public class DecryptedEntry
{
    [JsonPropertyName("careContextReference")]
    public string CareContextReference { get; set; } = string.Empty;

    [JsonPropertyName("fhirBundle")]
    public object? FhirBundle { get; set; }
}

// ─── Consent Request mapping (to track consentRequestId ↔ gatewayRequestId) ──

public class ConsentRequestMapping
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("consentRequestId")]
    public string ConsentRequestId { get; set; } = string.Empty;

    [JsonPropertyName("gatewayRequestId")]
    public string GatewayRequestId { get; set; } = string.Empty;
}
