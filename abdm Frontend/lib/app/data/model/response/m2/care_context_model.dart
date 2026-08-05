class CareContextModel {
  final String referenceNumber;
  final String display;
  final String type;
  final String date;

  CareContextModel({
    required this.referenceNumber,
    required this.display,
    required this.type,
    required this.date,
  });

  factory CareContextModel.fromJson(Map<String, dynamic> json) {
    return CareContextModel(
      referenceNumber: json['referenceNumber'] ?? 'OPD-10024',
      display: json['display'] ?? 'General Medicine OPD Consultation',
      type: json['type'] ?? 'OPD',
      date: json['date'] ?? '2026-08-03',
    );
  }
}
