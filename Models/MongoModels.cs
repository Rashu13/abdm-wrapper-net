using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AbdmWrapperNet.Models;

public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("gender")]
    public string Gender { get; set; } = string.Empty;

    [BsonElement("dateOfBirth")]
    public string DateOfBirth { get; set; } = string.Empty;

    [BsonElement("patientReference")]
    public string PatientReference { get; set; } = string.Empty;

    [BsonElement("patientDisplay")]
    public string PatientDisplay { get; set; } = string.Empty;

    [BsonElement("patientMobile")]
    public string PatientMobile { get; set; } = string.Empty;

    [BsonElement("careContexts")]
    public List<CareContext> CareContexts { get; set; } = new();

    [BsonElement("consents")]
    public List<Consent> Consents { get; set; } = new();

    [BsonElement("hipId")]
    public string HipId { get; set; } = string.Empty;
}

public class Consent
{
    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("lastUpdatedOn")]
    public string LastUpdatedOn { get; set; } = string.Empty;

    [BsonElement("grantedOn")]
    public string GrantedOn { get; set; } = string.Empty;

    [BsonElement("revokedOn")]
    public string RevokedOn { get; set; } = string.Empty;

    [BsonElement("consentDetail")]
    public ConsentDetail? ConsentDetail { get; set; }

    [BsonElement("signature")]
    public string Signature { get; set; } = string.Empty;
}

public class ConsentDetail
{
    [BsonElement("schemaVersion")]
    public string SchemaVersion { get; set; } = string.Empty;

    [BsonElement("consentId")]
    public string ConsentId { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [BsonElement("patient")]
    public OnFetchConsentPatient? Patient { get; set; }

    [BsonElement("careContexts")]
    public List<ConsentCareContexts> CareContexts { get; set; } = new();

    [BsonElement("purpose")]
    public Purpose? Purpose { get; set; }

    [BsonElement("hip")]
    public ConsentHIP? Hip { get; set; }

    [BsonElement("hiu")]
    public ConsentHIU? Hiu { get; set; }

    [BsonElement("consentManager")]
    public OnFetchConsentManager? ConsentManager { get; set; }

    [BsonElement("requester")]
    public ConsentRequester? Requester { get; set; }

    [BsonElement("hiTypes")]
    public List<string> HiTypes { get; set; } = new();

    [BsonElement("permission")]
    public Permission? Permission { get; set; }

    [BsonElement("signature")]
    public string Signature { get; set; } = string.Empty;
}

public class OnFetchConsentPatient
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;
}

public class ConsentCareContexts
{
    [BsonElement("patientReference")]
    public string PatientReference { get; set; } = string.Empty;

    [BsonElement("careContextReference")]
    public string CareContextReference { get; set; } = string.Empty;
}

public class Purpose
{
    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    [BsonElement("code")]
    public string Code { get; set; } = string.Empty;

    [BsonElement("refUri")]
    public string RefUri { get; set; } = string.Empty;
}

public class ConsentHIP
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;
}

public class ConsentHIU
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;
}

public class OnFetchConsentManager
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;
}

public class ConsentRequester
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("identifier")]
    public ConsentRequesterIdentifier? Identifier { get; set; }
}

public class ConsentRequesterIdentifier
{
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("value")]
    public string Value { get; set; } = string.Empty;

    [BsonElement("system")]
    public string System { get; set; } = string.Empty;
}

public class Permission
{
    [BsonElement("accessMode")]
    public string AccessMode { get; set; } = string.Empty;

    [BsonElement("dateRange")]
    public DateRange? DateRange { get; set; }

    [BsonElement("dataEraseAt")]
    public string DataEraseAt { get; set; } = string.Empty;

    [BsonElement("frequency")]
    public Frequency? Frequency { get; set; }
}

public class DateRange
{
    [BsonElement("from")]
    public string From { get; set; } = string.Empty;

    [BsonElement("to")]
    public string To { get; set; } = string.Empty;
}

public class Frequency
{
    [BsonElement("unit")]
    public string Unit { get; set; } = string.Empty;

    [BsonElement("value")]
    public int Value { get; set; }

    [BsonElement("repeats")]
    public int Repeats { get; set; }
}

public class RequestLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clientRequestId")]
    public string ClientRequestId { get; set; } = string.Empty;

    [BsonElement("gatewayRequestId")]
    public string GatewayRequestId { get; set; } = string.Empty;

    [BsonElement("linkTokenRequestId")]
    public string LinkTokenRequestId { get; set; } = string.Empty;

    [BsonElement("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [BsonElement("linkRefNumber")]
    public string LinkRefNumber { get; set; } = string.Empty;

    [BsonElement("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("error")]
    public object? Error { get; set; }

    [BsonElement("module")]
    public string Module { get; set; } = string.Empty;

    [BsonElement("otp")]
    public string Otp { get; set; } = string.Empty;

    [BsonElement("requestDetails")]
    public BsonDocument RequestDetails { get; set; } = new();

    [BsonElement("responseDetails")]
    public BsonDocument ResponseDetails { get; set; } = new();

    [BsonElement("consentId")]
    public string ConsentId { get; set; } = string.Empty;

    [BsonElement("entityType")]
    public string EntityType { get; set; } = string.Empty;

    [BsonElement("createdOn")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [BsonElement("hipId")]
    public string HipId { get; set; } = string.Empty;
}

public class LinkToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [BsonElement("linkToken")]
    public string LinkTokenVal { get; set; } = string.Empty;

    [BsonElement("expiry")]
    public string Expiry { get; set; } = string.Empty;

    [BsonElement("hipId")]
    public string HipId { get; set; } = string.Empty;

    [BsonElement("linkTokenRequestId")]
    public string LinkTokenRequestId { get; set; } = string.Empty;
}

public class ConsentPatient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("consentId")]
    public string ConsentId { get; set; } = string.Empty;

    [BsonElement("abhaAddress")]
    public string AbhaAddress { get; set; } = string.Empty;

    [BsonElement("entityType")]
    public string EntityType { get; set; } = string.Empty;

    [BsonElement("hipId")]
    public string HipId { get; set; } = string.Empty;
}
