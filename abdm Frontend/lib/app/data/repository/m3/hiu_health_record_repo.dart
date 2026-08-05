import 'dart:convert';
import 'package:abdm_frontend/util/api_endpoints.dart';

import '../../api/abdm_server.dart';
import '../../api/fhir_parser.dart';
import '../../model/response/m3/consent_model.dart';

class HiuHealthRecordRepo {
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
