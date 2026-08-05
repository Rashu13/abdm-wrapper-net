import 'dart:convert';

class FhirRecordItem {
  final String resourceType;
  final String title;
  final String date;
  final String doctorName;
  final String summary;

  FhirRecordItem({
    required this.resourceType,
    required this.title,
    required this.date,
    required this.doctorName,
    required this.summary,
  });

  factory FhirRecordItem.fromJson(Map<String, dynamic> json) {
    return FhirRecordItem(
      resourceType: json['resourceType'] ?? 'DiagnosticReport',
      title: json['title'] ?? 'Medical Record',
      date: json['date'] ?? DateTime.now().toString().split(' ')[0],
      doctorName: json['doctorName'] ?? 'Dr. Sharma (OPD)',
      summary: json['summary'] ?? 'OPD Consultation & Prescription details.',
    );
  }
}

class FhirParser {
  static List<FhirRecordItem> parseBundle(String rawJson) {
    try {
      final Map<String, dynamic> data = jsonDecode(rawJson);
      if (data.containsKey('entry') && data['entry'] is List) {
        return (data['entry'] as List)
            .map((e) => FhirRecordItem.fromJson(e['resource'] ?? e))
            .toList();
      }
    } catch (e) {
      // Catch JSON decode or format errors
    }
    return [];
  }
}
