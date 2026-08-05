using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AbdmWrapperNet.Models;

public class AbhaConsentLog
{
    [Key]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("loginId")]
    public string LoginId { get; set; } = string.Empty;

    [BsonElement("consentTimestamp")]
    public DateTime ConsentTimestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("operatorName")]
    public string OperatorName { get; set; } = string.Empty;

    [BsonElement("beneficiaryName")]
    public string BeneficiaryName { get; set; } = string.Empty;

    [BsonElement("chk1")]
    public bool Chk1 { get; set; }

    [BsonElement("chk2")]
    public bool Chk2 { get; set; }

    [BsonElement("chk3")]
    public bool Chk3 { get; set; }

    [BsonElement("chk4")]
    public bool Chk4 { get; set; }

    [BsonElement("chk5")]
    public bool Chk5 { get; set; }

    [BsonElement("chk6")]
    public bool Chk6 { get; set; }

    [BsonElement("chk7")]
    public bool Chk7 { get; set; }

    [BsonElement("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;
}
