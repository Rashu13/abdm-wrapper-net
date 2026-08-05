using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbdmWrapperNet.Models;

[Table("health_data_records")]
public class HealthDataRecord
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("care_context_reference")]
    [MaxLength(100)]
    public string CareContextReference { get; set; } = string.Empty;

    [Required]
    [Column("abha_address")]
    [MaxLength(100)]
    public string AbhaAddress { get; set; } = string.Empty;

    [Required]
    [Column("record_type")]
    [MaxLength(50)]
    public string RecordType { get; set; } = "PrescriptionRecord"; // Or OPConsultRecord

    [Column("fhir_json_payload", TypeName = "text")]
    public string FhirJsonPayload { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
