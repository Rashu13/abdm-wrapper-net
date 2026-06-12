using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AbdmWrapperNet.Models;

public class ErrorResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class DiscoverRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("patient")]
    public PatientDemographicDetails? Patient { get; set; }

    [JsonPropertyName("hipId")]
    public string HipId { get; set; } = string.Empty;
}

public class PatientDemographicDetails
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("yearOfBirth")]
    public string YearOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("verifiedIdentifiers")]
    public List<PatientVerifiedIdentifiers> VerifiedIdentifiers { get; set; } = new();

    [JsonPropertyName("unverifiedIdentifiers")]
    public List<PatientUnverifiedIdentifiers> UnverifiedIdentifiers { get; set; } = new();
}

public class PatientVerifiedIdentifiers
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class PatientUnverifiedIdentifiers
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class InitV3Response
{
    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("patient")]
    public List<PatientCareContextHIType> Patient { get; set; } = new();
}

public class PatientCareContextHIType
{
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; } = string.Empty;

    [JsonPropertyName("display")]
    public string Display { get; set; } = string.Empty;

    [JsonPropertyName("careContexts")]
    public List<CareContext> CareContexts { get; set; } = new();

    [JsonPropertyName("hiType")]
    public string HiType { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class ConfirmResponse
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("confirmation")]
    public Confirmation? Confirmation { get; set; }
}

public class Confirmation
{
    [JsonPropertyName("linkRefNumber")]
    public string LinkRefNumber { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}

public class StatusResponse
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("resp")]
    public RespRequest? Resp { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("acknowledgement")]
    public Acknowledgement? Acknowledgement { get; set; }
}

public class RespRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;
}

public class Acknowledgement
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class LinkOnAddCareContextsV3Response
{
    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }
}

public class OnGenerateTokenResponse
{
    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("linkToken")]
    public string LinkToken { get; set; } = string.Empty;

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }
}

public class HIPNotifyRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("notification")]
    public HIPNotification? Notification { get; set; }
}

public class HIPNotification
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentId")]
    public string ConsentId { get; set; } = string.Empty;

    [JsonPropertyName("consentDetail")]
    public ConsentDetail? ConsentDetail { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}

public class HIPHealthInformationRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("hiRequest")]
    public HealthInformationRequest? HiRequest { get; set; }
}

public class HealthInformationRequest
{
    [JsonPropertyName("consent")]
    public IdRequest? Consent { get; set; }

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }

    [JsonPropertyName("dataPushUrl")]
    public string DataPushUrl { get; set; } = string.Empty;

    [JsonPropertyName("keyMaterial")]
    public HealthInformationKeyMaterial? KeyMaterial { get; set; }
}

public class IdRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public class HealthInformationKeyMaterial
{
    [JsonPropertyName("cryptoAlg")]
    public string CryptoAlg { get; set; } = string.Empty;

    [JsonPropertyName("curve")]
    public string Curve { get; set; } = string.Empty;

    [JsonPropertyName("dhPublicKey")]
    public HealthInformationDhPublicKey? DhPublicKey { get; set; }

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;
}

public class HealthInformationDhPublicKey
{
    [JsonPropertyName("expiry")]
    public string Expiry { get; set; } = string.Empty;

    [JsonPropertyName("parameters")]
    public string Parameters { get; set; } = string.Empty;

    [JsonPropertyName("keyValue")]
    public string KeyValue { get; set; } = string.Empty;
}

public class ProfileShareV3Request
{
    [JsonPropertyName("intent")]
    public string Intent { get; set; } = string.Empty;

    [JsonPropertyName("metaData")]
    public ProfileShareMetaData? MetaData { get; set; }

    [JsonPropertyName("profile")]
    public ProfileShareProfile? Profile { get; set; }
}

public class ProfileShareMetaData
{
    [JsonPropertyName("hipId")]
    public string HipId { get; set; } = string.Empty;

    [JsonPropertyName("context")]
    public string Context { get; set; } = string.Empty;

    [JsonPropertyName("hprId")]
    public string HprId { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;
}

public class ProfileShareProfile
{
    [JsonPropertyName("patient")]
    public PatientV3Details? Patient { get; set; }
}

public class PatientV3Details
{
    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("abhaNumber")]
    public string AbhaNumber { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public PatientAddress? Address { get; set; }

    [JsonPropertyName("yearOfBirth")]
    public string YearOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("dayOfBirth")]
    public string DayOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("monthOfBirth")]
    public string MonthOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("identifiers")]
    public List<PatientVerifiedIdentifiers> Identifiers { get; set; } = new();
}

public class PatientAddress
{
    [JsonPropertyName("line")]
    public string Line { get; set; } = string.Empty;

    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("pincode")]
    public string Pincode { get; set; } = string.Empty;
}

public class GenericV3Response
{
    [JsonPropertyName("httpStatus")]
    public string HttpStatus { get; set; } = "OK";

    [JsonPropertyName("error")]
    public object? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class GatewayCallbackResponse
{
    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }
}

public class LinkRecordsV3Request
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("requesterId")]
    public string RequesterId { get; set; } = string.Empty;

    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("careContexts")]
    public List<CareContext> CareContexts { get; set; } = new();
}

public class HIPPatient
{
    [JsonPropertyName("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("dateOfBirth")]
    public string DateOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("patientReference")]
    public string PatientReference { get; set; } = string.Empty;

    [JsonPropertyName("patientDisplay")]
    public string PatientDisplay { get; set; } = string.Empty;

    [JsonPropertyName("patientMobile")]
    public string PatientMobile { get; set; } = string.Empty;

    [JsonPropertyName("careContexts")]
    public List<CareContext> CareContexts { get; set; } = new();

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("hipId")]
    public string HipId { get; set; } = string.Empty;
}

public class FacadeV3Response
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public object? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("clientRequestId")]
    public string? ClientRequestId { get; set; }

    [JsonPropertyName("errors")]
    public List<ErrorResponse>? Errors { get; set; }

    [JsonIgnore]
    public int HttpStatusCode { get; set; }
}

public class ErrorV3Response
{
    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }
}

public class RequestStatusV3Response
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public List<ErrorResponse>? Errors { get; set; }
}

public class HIPOnNotifyRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("acknowledgement")]
    public ConsentAcknowledgement? Acknowledgement { get; set; }

    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    [JsonPropertyName("resp")]
    public RespRequest? Resp { get; set; }

    [JsonPropertyName("response")]
    public RespRequest? Response { get; set; }
}

public class ConsentAcknowledgement
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("consentId")]
    public string ConsentId { get; set; } = string.Empty;
}

public class DeepLinkingRequest
{
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("notification")]
    public DeepLinkNotification? Notification { get; set; }
}

public class DeepLinkNotification
{
    [JsonPropertyName("phoneNo")]
    public string PhoneNo { get; set; } = string.Empty;

    [JsonPropertyName("hip")]
    public HIPDetails? Hip { get; set; }
}

public class HIPDetails
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}



