class ConsentModel {
  final String id;
  final String purpose;
  final String status;
  final String requesterName;
  final String expiryDate;

  ConsentModel({
    required this.id,
    required this.purpose,
    required this.status,
    required this.requesterName,
    required this.expiryDate,
  });

  factory ConsentModel.fromJson(Map<String, dynamic> json) {
    return ConsentModel(
      id: json['id'] ?? 'CONSENT-8842-1109',
      purpose: json['purpose'] ?? 'Medical Consultation & Diagnosis',
      status: json['status'] ?? 'GRANTED',
      requesterName: json['requesterName'] ?? 'Max Healthcare Hospital',
      expiryDate: json['expiryDate'] ?? '2026-08-30',
    );
  }
}
