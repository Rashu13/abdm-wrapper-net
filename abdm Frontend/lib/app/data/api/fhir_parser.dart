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

  static FhirRecordItem? fromResource(Map<String, dynamic> r) {
    final String type = r['resourceType'] ?? '';
    
    if (type == 'Patient' || type == 'Practitioner' || type == 'Organization' || type == 'Encounter') {
      return null;
    }

    String title = 'Medical Record';
    String date = '';
    String doctorName = 'Dr. Sonomed Specialist';
    String summary = 'Details of the record.';

    if (type == 'Composition') {
      title = r['title'] ?? 'Composition Header';
      date = r['date'] ?? '';
      doctorName = r['author']?[0]?['display'] ?? 'Dr. Sonomed Specialist';
      summary = 'Document Type: ' + (r['type']?['text'] ?? 'Clinical Document');
    } else if (type == 'MedicationRequest') {
      title = 'Prescription: ' + (r['medicationCodeableConcept']?['text'] ?? 'Medicine');
      date = r['authoredOn'] ?? '';
      doctorName = r['requester']?['display'] ?? 'Dr. Sonomed Specialist';
      summary = 'Dosage: ' + (r['dosageInstruction']?[0]?['text'] ?? 'As directed by physician');
    } else if (type == 'Observation') {
      title = 'Observation: ' + (r['code']?['text'] ?? 'Clinical Observation');
      date = r['effectiveDateTime'] ?? '';
      doctorName = r['performer']?[0]?['display'] ?? 'Dr. Sonomed Specialist';
      summary = 'Result: ' + (r['valueString'] ?? r['valueQuantity']?['value']?.toString() ?? 'Completed');
    } else if (type == 'Immunization') {
      title = 'Immunization: ' + (r['vaccineCode']?['text'] ?? 'Vaccine');
      date = r['occurrenceDateTime'] ?? '';
      doctorName = r['performer']?[0]?['actor']?['display'] ?? 'Vaccination Center';
      
      final lot = r['lotNumber'] ?? 'N/A';
      final mfr = r['manufacturer']?['display'] ?? 'N/A';
      final dose = r['protocolApplied']?[0]?['doseNumberPositiveInt'] ?? 1;
      
      summary = 'Dose No: $dose | Lot: $lot | Manufacturer: $mfr';
    } else if (type == 'DocumentReference') {
      title = 'Attached Document: ' + (r['content']?[0]?['attachment']?['title'] ?? 'PDF Report');
      date = r['content']?[0]?['attachment']?['creation'] ?? '';
      doctorName = 'ABDM Provider';
      summary = 'Type: ' + (r['content']?[0]?['attachment']?['contentType'] ?? 'application/pdf') + ' (Decrypted Attachment PDF)';
    } else if (type == 'Condition') {
      title = 'Condition / Diagnosis: ' + (r['code']?['text'] ?? 'Clinical Finding');
      date = '';
      doctorName = 'ABDM Provider';
      summary = 'Clinical Status: ' + (r['clinicalStatus']?['coding']?[0]?['code'] ?? 'Active');
    } else if (type == 'AllergyIntolerance') {
      title = 'Allergy: ' + (r['code']?['text'] ?? 'Allergy');
      date = r['recordedDate'] ?? '';
      doctorName = r['recorder']?['display'] ?? 'ABDM Provider';
      summary = 'Status: ' + (r['clinicalStatus']?['coding']?[0]?['code'] ?? 'Active') +
                ' | Verification: ' + (r['verificationStatus']?['coding']?[0]?['code'] ?? 'Confirmed');
    } else {
      title = type;
      summary = r['text']?['div']?.toString() ?? 'Raw resource format.';
    }

    // Clean up dates
    if (date.contains('T')) {
      date = date.split('T')[0];
    }
    if (date.isEmpty) {
      date = DateTime.now().toString().split(' ')[0];
    }

    return FhirRecordItem(
      resourceType: type,
      title: title,
      date: date,
      doctorName: doctorName,
      summary: summary,
    );
  }
}

class FhirParser {
  static List<FhirRecordItem> parseBundle(String rawJson) {
    try {
      final Map<String, dynamic> data = jsonDecode(rawJson);
      if (data.containsKey('entry') && data['entry'] is List) {
        final List<FhirRecordItem> items = [];
        for (var e in data['entry']) {
          final res = e['resource'] ?? e;
          if (res is Map<String, dynamic>) {
            final parsed = FhirRecordItem.fromResource(res);
            if (parsed != null) {
              items.add(parsed);
            }
          }
        }
        return items;
      }
    } catch (e) {
      print("FhirParser error: $e");
    }
    return [];
  }
}
