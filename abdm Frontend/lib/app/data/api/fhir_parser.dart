import 'dart:convert';

class FhirRecordItem {
  final String resourceType;
  final String title;
  final String date;
  final String doctorName;
  final String summary;
  final String organizationName;
  final String organizationAddress;
  final String organizationPhone;
  final String? pdfData;

  FhirRecordItem({
    required this.resourceType,
    required this.title,
    required this.date,
    required this.doctorName,
    required this.summary,
    required this.organizationName,
    required this.organizationAddress,
    required this.organizationPhone,
    this.pdfData,
  });

  factory FhirRecordItem.fromJson(Map<String, dynamic> json) {
    return FhirRecordItem(
      resourceType: json['resourceType'] ?? 'DiagnosticReport',
      title: json['title'] ?? 'Medical Record',
      date: json['date'] ?? DateTime.now().toString().split(' ')[0],
      doctorName: json['doctorName'] ?? 'N/A',
      summary: json['summary'] ?? '',
      organizationName: json['organizationName'] ?? 'ABDM Facility',
      organizationAddress: json['organizationAddress'] ?? '',
      organizationPhone: json['organizationPhone'] ?? '',
      pdfData: json['pdfData'],
    );
  }

  static FhirRecordItem? fromResource(Map<String, dynamic> r) {
    final String type = r['resourceType'] ?? '';
    
    if (type == 'Patient' || type == 'Practitioner' || type == 'Organization' || type == 'Encounter') {
      return null;
    }

    String title = 'Medical Record';
    String date = '';
    String doctorName = 'N/A';
    String summary = '';
    String? pdfData;

    if (type == 'Composition') {
      title = r['title'] ?? 'Medical Summary';
      date = r['date'] ?? '';
      doctorName = r['author']?[0]?['display'] ?? 'N/A';
      summary = 'Document Type: ' + (r['type']?['text'] ?? 'Clinical Document');
    } else if (type == 'MedicationRequest') {
      title = 'Prescription: ' + (r['medicationCodeableConcept']?['text'] ?? 'Prescribed Drug');
      date = r['authoredOn'] ?? '';
      doctorName = r['requester']?['display'] ?? 'N/A';
      summary = 'Dosage: ' + (r['dosageInstruction']?[0]?['text'] ?? 'As directed');
    } else if (type == 'Observation') {
      title = 'Observation: ' + (r['code']?['text'] ?? 'Clinical Observation');
      date = r['effectiveDateTime'] ?? '';
      doctorName = r['performer']?[0]?['display'] ?? 'N/A';
      final qVal = r['valueQuantity']?['value']?.toString();
      final qUnit = r['valueQuantity']?['unit']?.toString() ?? '';
      final valStr = r['valueString'] ?? (qVal != null ? '$qVal $qUnit' : 'Completed');
      summary = 'Result: ' + valStr;
    } else if (type == 'DiagnosticReport') {
      title = 'Diagnostic Report: ' + (r['code']?['text'] ?? 'Lab Report');
      date = r['issued'] ?? '';
      doctorName = r['performer']?[0]?['display'] ?? 'N/A';
      final results = r['result'] as List? ?? [];
      summary = 'Status: ' + (r['status'] ?? 'Final') + ' | Results Count: ' + results.length.toString();
    } else if (type == 'Immunization') {
      title = 'Immunization: ' + (r['vaccineCode']?['text'] ?? 'Vaccine');
      date = r['occurrenceDateTime'] ?? '';
      doctorName = r['performer']?[0]?['actor']?['display'] ?? 'N/A';
      
      final lot = r['lotNumber'] ?? 'N/A';
      final mfr = r['manufacturer']?['display'] ?? 'N/A';
      final dose = r['protocolApplied']?[0]?['doseNumberPositiveInt'] ?? 1;
      
      summary = 'Dose No: $dose | Lot: $lot | Manufacturer: $mfr';
    } else if (type == 'DocumentReference') {
      title = 'Attached Document: ' + (r['content']?[0]?['attachment']?['title'] ?? 'PDF Report');
      date = r['content']?[0]?['attachment']?['creation'] ?? '';
      doctorName = 'N/A';
      summary = 'Type: ' + (r['content']?[0]?['attachment']?['contentType'] ?? 'application/pdf') + ' (Decrypted Attachment PDF)';
      pdfData = r['content']?[0]?['attachment']?['data']?.toString();
    } else if (type == 'Binary') {
      title = 'Prescription Scanned PDF / Copy';
      date = '';
      doctorName = 'N/A';
      summary = 'Type: ' + (r['contentType'] ?? 'application/pdf') + ' (Decrypted Binary PDF Attachment)';
      pdfData = r['data']?.toString();
    } else if (type == 'Procedure') {
      title = 'Procedure: ' + (r['code']?['text'] ?? r['code']?['coding']?[0]?['display'] ?? 'Medical Procedure');
      date = r['performedDateTime'] ?? r['performedPeriod']?['start'] ?? '';
      doctorName = 'N/A';
      summary = 'Status: ' + (r['status'] ?? 'Completed');
    } else if (type == 'CarePlan') {
      title = 'Care Plan: ' + (r['category']?[0]?['text'] ?? r['category']?[0]?['coding']?[0]?['display'] ?? 'Treatment Care Plan');
      date = '';
      doctorName = 'N/A';
      final activityText = r['activity']?[0]?['detail']?['description'] ?? '';
      final noteText = r['note']?[0]?['text'] ?? '';
      summary = (activityText.isNotEmpty ? 'Activity: $activityText' : '') + 
                (noteText.isNotEmpty ? ' | Note: $noteText' : '');
      if (summary.trim().isEmpty) {
        summary = 'Status: ' + (r['status'] ?? 'Active');
      }
    } else if (type == 'Condition') {
      title = 'Condition / Diagnosis: ' + (r['code']?['text'] ?? 'Clinical Finding');
      date = '';
      doctorName = 'N/A';
      summary = 'Clinical Status: ' + (r['clinicalStatus']?['coding']?[0]?['code'] ?? 'Active');
    } else if (type == 'AllergyIntolerance') {
      title = 'Allergy: ' + (r['code']?['text'] ?? 'Allergy');
      date = r['recordedDate'] ?? '';
      doctorName = r['recorder']?['display'] ?? 'N/A';
      summary = 'Status: ' + (r['clinicalStatus']?['coding']?[0]?['code'] ?? 'Active') +
                ' | Verification: ' + (r['verificationStatus']?['coding']?[0]?['code'] ?? 'Confirmed');
    } else {
      title = type;
      String rawText = r['text']?['div']?.toString() ?? 'Raw resource format.';
      summary = rawText.replaceAll(RegExp(r'<[^>]*>'), '').replaceAll('&nbsp;', ' ').trim();
      if (summary.isEmpty) summary = 'Raw resource format.';
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
      organizationName: '',
      organizationAddress: '',
      organizationPhone: '',
      pdfData: pdfData,
    );
  }
}

class FhirParser {
  static List<FhirRecordItem> parseBundle(String rawJson) {
    try {
      final Map<String, dynamic> data = jsonDecode(rawJson);
      if (data.containsKey('entry') && data['entry'] is List) {
        final List<FhirRecordItem> items = [];
        
        // 1. Resolve bundle-level date, doctor name, and organization info from Composition or Encounter
        String? bundleDate;
        String? bundleDoctor;
        String? bundleOrganization;
        String? bundleOrgAddress;
        String? bundleOrgPhone;
        
        for (var e in data['entry']) {
          final res = e['resource'] ?? e;
          if (res is Map<String, dynamic>) {
            final type = res['resourceType'] ?? '';
            if (type == 'Composition') {
              bundleDate = res['date'];
              bundleDoctor = res['author']?[0]?['display'];
              bundleOrganization = res['custodian']?['display'];
              break;
            }
          }
        }
        
        // Fallback or detailed scanning for Practitioner / Organization / Encounter resources in the bundle
        for (var e in data['entry']) {
          final res = e['resource'] ?? e;
          if (res is Map<String, dynamic>) {
            final type = res['resourceType'] ?? '';
            if (type == 'Encounter' && bundleDate == null) {
              bundleDate = res['period']?['start'];
            }
            if (type == 'Practitioner' && bundleDoctor == null) {
              final nameMap = res['name']?[0];
              if (nameMap != null) {
                bundleDoctor = nameMap['text'] ?? nameMap['family'];
              }
            }
            if (type == 'Organization') {
              if (bundleOrganization == null || bundleOrganization.isEmpty) {
                bundleOrganization = res['name'];
              }
              final addrList = res['address'] as List?;
              if (addrList != null && addrList.isNotEmpty) {
                bundleOrgAddress = addrList[0]['text'] ?? addrList[0]['line']?.join(', ');
              }
              final telList = res['telecom'] as List?;
              if (telList != null && telList.isNotEmpty) {
                bundleOrgPhone = telList[0]['value'];
              }
            }
          }
        }

        // Standardize date format
        if (bundleDate != null && bundleDate.contains('T')) {
          bundleDate = bundleDate.split('T')[0];
        }
        if (bundleDate == null || bundleDate.isEmpty) {
          bundleDate = DateTime.now().toString().split(' ')[0];
        }
        if (bundleDoctor == null || bundleDoctor.trim().isEmpty) {
          bundleDoctor = 'N/A';
        }
        if (bundleOrganization == null || bundleOrganization.trim().isEmpty) {
          bundleOrganization = 'ABDM Facility';
        }
        if (bundleOrgAddress == null || bundleOrgAddress.trim().isEmpty) {
          bundleOrgAddress = '';
        }
        if (bundleOrgPhone == null) {
          bundleOrgPhone = '';
        }

        // 2. Parse and override records with the bundle-level metadata
        for (var e in data['entry']) {
          final res = e['resource'] ?? e;
          if (res is Map<String, dynamic>) {
            final type = res['resourceType'] ?? '';
            
            // If it is a Composition, parse its sections as separate clinical items
            if (type == 'Composition') {
              final sections = res['section'] as List?;
              if (sections != null) {
                for (var sec in sections) {
                  if (sec is Map<String, dynamic>) {
                    final secTitle = sec['title']?.toString() ?? 'Section';
                    final secCode = sec['code']?['coding']?[0]?['code']?.toString() ?? '';
                    final secText = sec['text']?['div']?.toString() ?? '';
                    
                    // Clean HTML tags from narrative text
                    String cleanText = secText.replaceAll(RegExp(r'<[^>]*>'), '').replaceAll('&nbsp;', ' ').trim();
                    
                    if (cleanText.isNotEmpty) {
                      String mappedType = 'general';
                      final titleLower = secTitle.toLowerCase();
                      
                      if (titleLower.contains('complaint') || titleLower.contains('symptom') || secCode == '422843007') {
                        mappedType = 'composition_complaints';
                      } else if (titleLower.contains('history') || titleLower.contains('diagnosis') || secCode == '371529009') {
                        mappedType = 'composition_history';
                      } else if (titleLower.contains('investigation') || titleLower.contains('diagnostic') || titleLower.contains('lab') || secCode == '30954-2') {
                        mappedType = 'composition_investigations';
                      } else if (titleLower.contains('procedure') || secCode == '439006003') {
                        mappedType = 'composition_procedures';
                      } else if (titleLower.contains('medication') || titleLower.contains('drug') || titleLower.contains('prescrip') || secCode == '10160-0') {
                        mappedType = 'composition_medications';
                      } else if (titleLower.contains('care plan') || titleLower.contains('treatment') || secCode == '18776-5') {
                        mappedType = 'composition_careplan';
                      } else if (titleLower.contains('document') || titleLower.contains('reference') || secCode == '55113-5') {
                        mappedType = 'composition_documents';
                      }
                      
                      items.add(FhirRecordItem(
                        resourceType: mappedType,
                        title: secTitle,
                        date: bundleDate,
                        doctorName: bundleDoctor,
                        summary: cleanText,
                        organizationName: bundleOrganization,
                        organizationAddress: bundleOrgAddress,
                        organizationPhone: bundleOrgPhone,
                      ));
                    }
                  }
                }
              }
            }

            final parsed = FhirRecordItem.fromResource(res);
            if (parsed != null) {
              items.add(FhirRecordItem(
                resourceType: parsed.resourceType,
                title: parsed.title,
                date: bundleDate,
                doctorName: bundleDoctor,
                summary: parsed.summary,
                organizationName: bundleOrganization,
                organizationAddress: bundleOrgAddress,
                organizationPhone: bundleOrgPhone,
                pdfData: parsed.pdfData,
              ));
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
