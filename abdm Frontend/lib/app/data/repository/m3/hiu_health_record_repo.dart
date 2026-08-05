import 'dart:convert';
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
  final String purpose;
  final String fromDate;
  final String toDate;

  HiuConsentRequestModel({
    required this.id,
    required this.clientRequestId,
    required this.status,
    required this.createdAt,
    required this.abhaAddress,
    required this.hiTypes,
    required this.purpose,
    required this.fromDate,
    required this.toDate,
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

    String abha = reqDetails['patientAbhaAddress'] ??
        reqDetails['patientAbha'] ??
        reqDetails['consent']?['patient']?['id'] ??
        'user_40893@sbx';

    List<String> hi = [];
    if (reqDetails['hiTypes'] is List) {
      hi = List<String>.from(reqDetails['hiTypes']);
    } else if (reqDetails['consent']?['hiTypes'] is List) {
      hi = List<String>.from(reqDetails['consent']['hiTypes']);
    } else {
      hi = ['Prescription', 'OPConsultation', 'DiagnosticReport'];
    }

    String purp = reqDetails['purposeCode'] ??
        reqDetails['consent']?['purpose']?['code'] ??
        'CAREMGT';

    return HiuConsentRequestModel(
      id: json['id']?.toString() ?? '',
      clientRequestId: json['clientRequestId']?.toString() ?? '',
      status: (json['status']?.toString() ?? 'REQUESTED').toUpperCase(),
      createdAt: json['createdAt']?.toString() ?? DateTime.now().toIso8601String(),
      abhaAddress: abha,
      hiTypes: hi,
      purpose: purp,
      fromDate: reqDetails['dateFrom'] ?? reqDetails['consent']?['permission']?['dateRange']?['from'] ?? '',
      toDate: reqDetails['dateTo'] ?? reqDetails['consent']?['permission']?['dateRange']?['to'] ?? '',
    );
  }
}

class HiuHealthRecordRepo {
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
      final reqId = "REQ_${DateTime.now().millisecondsSinceEpoch}";
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
            "text": "Care Management - Sonomed Portal",
            "code": purposeCode
          },
          "patient": {"id": abhaAddress},
          "hiu": {"id": hiuId ?? "IN0610090658"},
          "requester": {
            "name": "Dr. Sonomed Specialist",
            "identifier": {
              "type": "REGNO",
              "value": "NMC-998811",
              "system": "https://www.nmc.org.in"
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
      return FhirParser.parseBundle(response.body);
    }
    return [];
  }
}
