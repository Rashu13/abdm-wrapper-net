import 'dart:convert';
import 'dart:math';
import 'package:abdm_frontend/util/api_endpoints.dart';
import '../../api/abdm_server.dart';
import '../../api/fhir_parser.dart';
import '../../model/response/m3/consent_model.dart';

class HiuConsentRequestModel {
  final String id;
  final String clientRequestId;
  final String status;
  final String createdAt;
  final String abhaAddress;
  final List<String> hiTypes;
  final List<String> grantedHiTypes;
  final String purpose;
  final String fromDate;
  final String toDate;
  final String consentId;

  HiuConsentRequestModel({
    required this.id,
    required this.clientRequestId,
    required this.status,
    required this.createdAt,
    required this.abhaAddress,
    required this.hiTypes,
    required this.grantedHiTypes,
    required this.purpose,
    required this.fromDate,
    required this.toDate,
    required this.consentId,
  });

  factory HiuConsentRequestModel.fromJson(Map<String, dynamic> json) {
    Map<String, dynamic> reqDetails = {};
    if (json['requestDetails'] != null) {
      if (json['requestDetails'] is Map<String, dynamic>) {
        reqDetails = json['requestDetails'];
      } else if (json['requestDetails'] is String) {
        try {
          reqDetails = jsonDecode(json['requestDetails']);
        } catch (_) {}
      }
    }

    final consentReq = reqDetails['consentRequest'] ?? reqDetails['ConsentRequest'];
    
    // Resolve ABHA Address safely
    String abha = '';
    if (reqDetails['patientAbhaAddress'] != null) {
      abha = reqDetails['patientAbhaAddress'].toString();
    } else if (reqDetails['patientAbha'] != null) {
      abha = reqDetails['patientAbha'].toString();
    } else if (reqDetails['consent'] is Map && reqDetails['consent']['patient'] is Map) {
      abha = (reqDetails['consent']['patient']['id'] ?? '').toString();
    } else if (consentReq is Map && consentReq['consent'] is Map && consentReq['consent']['patient'] is Map) {
      abha = (consentReq['consent']['patient']['id'] ?? '').toString();
    }
    if (abha.isEmpty) abha = 'user_40893@sbx';

    // Resolve HI Types safely
    List<String> hi = [];
    if (reqDetails['hiTypes'] is List) {
      hi = List<String>.from(reqDetails['hiTypes']);
    } else if (reqDetails['consent'] is Map && reqDetails['consent']['hiTypes'] is List) {
      hi = List<String>.from(reqDetails['consent']['hiTypes']);
    } else if (consentReq is Map && consentReq['consent'] is Map && consentReq['consent']['hiTypes'] is List) {
      hi = List<String>.from(consentReq['consent']['hiTypes']);
    } else {
      hi = ['Prescription', 'OPConsultation', 'DiagnosticReport'];
    }

    // Resolve Response details safely (grantedHi)
    List<String> grantedHi = [];
    String resolvedStatus = (json['status']?.toString() ?? 'REQUESTED').toUpperCase();

    if (json['responseDetails'] != null) {
      Map<String, dynamic> respDetails = {};
      if (json['responseDetails'] is Map<String, dynamic>) {
        respDetails = json['responseDetails'];
      } else if (json['responseDetails'] is String) {
        try {
          respDetails = jsonDecode(json['responseDetails']);
        } catch (_) {}
      }

      // Check if we have the notify notification callback status
      final notifyResponse = respDetails['CONSENT_ON_NOTIFY_RESPONSE'];
      if (notifyResponse is Map && notifyResponse['notification'] is Map) {
        final notificationBlock = notifyResponse['notification'];
        final String? cbStatus = notificationBlock['status']?.toString();
        if (cbStatus != null && cbStatus.isNotEmpty) {
          resolvedStatus = cbStatus.toUpperCase();
        }
      }

      for (var entry in respDetails.entries) {
        if (entry.value is Map) {
          final valMap = entry.value as Map;
          final consentBlock = valMap['consent'] ?? valMap['Consent'];
          if (consentBlock is Map) {
            final detailBlock = consentBlock['consentDetail'] ?? consentBlock['ConsentDetail'];
            if (detailBlock is Map) {
              final hiTypesBlock = detailBlock['hiTypes'] ?? detailBlock['HiTypes'];
              if (hiTypesBlock is List) {
                grantedHi = List<String>.from(hiTypesBlock);
                break;
              }
            }
          }
        }
      }
    }

    // Resolve Purpose Code safely
    String purp = 'CAREMGT';
    if (reqDetails['purposeCode'] != null) {
      purp = reqDetails['purposeCode'].toString();
    } else if (reqDetails['consent'] is Map && reqDetails['consent']['purpose'] is Map) {
      purp = (reqDetails['consent']['purpose']['code'] ?? 'CAREMGT').toString();
    } else if (consentReq is Map && consentReq['consent'] is Map && consentReq['consent']['purpose'] is Map) {
      purp = (consentReq['consent']['purpose']['code'] ?? 'CAREMGT').toString();
    }

    // Resolve Date Range safely
    String from = reqDetails['dateFrom'] ?? '';
    String to = reqDetails['dateTo'] ?? '';
    if (consentReq is Map) {
      final consentBlock = consentReq['consent'] ?? consentReq['Consent'];
      if (consentBlock is Map) {
        final permBlock = consentBlock['permission'] ?? consentBlock['Permission'];
        if (permBlock is Map) {
          final rangeBlock = permBlock['dateRange'] ?? permBlock['DateRange'];
          if (rangeBlock is Map) {
            from = (rangeBlock['from'] ?? rangeBlock['From'] ?? '').toString();
            to = (rangeBlock['to'] ?? rangeBlock['To'] ?? '').toString();
          }
        }
      }
    }

    return HiuConsentRequestModel(
      id: json['id']?.toString() ?? '',
      clientRequestId: json['clientRequestId']?.toString() ?? '',
      status: resolvedStatus,
      createdAt: json['createdAt']?.toString() ?? DateTime.now().toIso8601String(),
      abhaAddress: abha,
      hiTypes: hi,
      grantedHiTypes: grantedHi,
      purpose: purp,
      fromDate: from,
      toDate: to,
      consentId: json['consentId']?.toString() ?? '',
    );
  }
}

class HiuHealthRecordRepo {
  static String _generateUuidV4() {
    final random = Random.secure();
    final bytes = List<int>.generate(16, (_) => random.nextInt(256));
    bytes[6] = (bytes[6] & 0x0F) | 0x40; // set version to 4
    bytes[8] = (bytes[8] & 0x3F) | 0x80; // set variant to 10
    
    final buffer = StringBuffer();
    for (int i = 0; i < 16; i++) {
      if (i == 4 || i == 6 || i == 8 || i == 10) {
        buffer.write('-');
      }
      buffer.write(bytes[i].toRadixString(16).padLeft(2, '0'));
    }
    return buffer.toString();
  }
  /// Fetch all HIU consent requests logged in backend database
  static Future<List<HiuConsentRequestModel>> getAllConsentRequests() async {
    try {
      final response = await AbdmServer.getRequest(ApiEndpoints.getConsentList);
      if (response != null && response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        return data
            .map((e) => HiuConsentRequestModel.fromJson(e as Map<String, dynamic>))
            .toList();
      }
    } catch (e) {
      print("getAllConsentRequests error: $e");
    }
    return [];
  }

  /// Initiate a new consent request to ABDM Gateway
  static Future<bool> initiateConsentRequest({
    required String abhaAddress,
    required String purposeCode,
    required List<String> hiTypes,
    required String fromDate,
    required String toDate,
    required String eraseAt,
    String? hiuId,
  }) async {
    try {
      final reqId = _generateUuidV4();
      final body = {
        "requestId": reqId,
        "timestamp": DateTime.now().toUtc().toIso8601String(),
        "patientAbhaAddress": abhaAddress,
        "purposeCode": purposeCode,
        "hiTypes": hiTypes,
        "dateFrom": fromDate,
        "dateTo": toDate,
        "eraseAt": eraseAt,
        "consent": {
          "purpose": {
            "text": "Care Management",
            "code": purposeCode,
            "refUri": "https://dev.abdm.gov.in"
          },
          "patient": {"id": abhaAddress},
          "hiu": {
            "id": hiuId ?? "IN0610090658",
            "name": "MIDHA HOSPITAL"
          },
          "requester": {
            "name": "Dr. Midha",
            "identifier": {
              "type": "REGNO",
              "value": "MCI-12345",
              "system": "https://mciindia.org"
            }
          },
          "hiTypes": hiTypes,
          "permission": {
            "accessMode": "VIEW",
            "dateRange": {"from": fromDate, "to": toDate},
            "dataEraseAt": eraseAt,
            "frequency": {
              "unit": "HOUR",
              "value": 1,
              "repeats": 0
            }
          }
        }
      };

      final response = await AbdmServer.postRequest(
        endpoint: ApiEndpoints.createConsentRequest,
        body: body,
      );

      return response != null &&
          (response.statusCode == 200 ||
              response.statusCode == 201 ||
              response.statusCode == 202);
    } catch (e) {
      print("initiateConsentRequest error: $e");
      return false;
    }
  }

  /// Legacy consent status fetch
  static Future<List<ConsentModel>> getConsents({String requestId = "REQ_DEFAULT"}) async {
    final response = await AbdmServer.getRequest(ApiEndpoints.getConsentStatus(requestId));
    if (response != null && response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['consents'] != null) {
        return (data['consents'] as List)
            .map((e) => ConsentModel.fromJson(e))
            .toList();
      }
    }
    return [];
  }

  /// Fetch and parse decrypted FHIR health records
  static Future<List<FhirRecordItem>> fetchDecryptedRecords(String requestId) async {
    final response = await AbdmServer.getRequest(
      ApiEndpoints.getHealthInformationStatus(requestId),
    );
    if (response != null && response.statusCode == 200) {
      try {
        final Map<String, dynamic> bodyJson = jsonDecode(response.body);
        final List<FhirRecordItem> allRecords = [];
        
        final decryptedList = bodyJson['decryptedHealthInformation'] ?? bodyJson['DecryptedHealthInformation'];
        if (decryptedList is List) {
          for (var item in decryptedList) {
            final fhirBundle = item['fhirBundle'] ?? item['FhirBundle'];
            if (fhirBundle != null) {
              final bundleStr = jsonEncode(fhirBundle);
              final parsedItems = FhirParser.parseBundle(bundleStr);
              allRecords.addAll(parsedItems);
            }
          }
        }
        return allRecords;
      } catch (e) {
        print("fetchDecryptedRecords parsing error: $e");
      }
    }
    return [];
  }

  /// Initiate health information request to ABDM gateway
  static Future<String?> fetchHealthInformation({
    required String consentId,
    required String fromDate,
    required String toDate,
  }) async {
    try {
      final requestId = _generateUuidV4();
      final body = {
        "requestId": requestId,
        "consentId": consentId,
        "dateRange": {
          "from": fromDate,
          "to": toDate,
        }
      };

      final response = await AbdmServer.postRequest(
        endpoint: ApiEndpoints.fetchEncryptedHealthRecord,
        body: body,
      );

      if (response != null &&
          (response.statusCode == 200 ||
              response.statusCode == 201 ||
              response.statusCode == 202)) {
        return requestId;
      }
    } catch (e) {
      print("fetchHealthInformation error: $e");
    }
    return null;
  }
}
