import 'dart:convert';
import 'package:abdm_frontend/app/data/api/abdm_server.dart';
import 'package:abdm_frontend/app/data/model/response/m2/care_context_model.dart';
import 'package:abdm_frontend/util/api_endpoints.dart';

class HipCareContextRepo {
  static Future<List<CareContextModel>> getLinkedCareContexts({String requestId = "REQ_DEFAULT"}) async {
    final response = await AbdmServer.getRequest(ApiEndpoints.getLinkStatus(requestId));
    if (response != null && response.statusCode == 200) {
      final data = jsonDecode(response.body);
      if (data['contexts'] != null) {
        return (data['contexts'] as List)
            .map((e) => CareContextModel.fromJson(e))
            .toList();
      }
    }
    return [];
  }

  static Future<bool> linkCareContext(String abhaAddress, String visitRef) async {
    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.linkCareContext,
      body: {
        'patientAbha': abhaAddress,
        'careContexts': [{'referenceNumber': visitRef}]
      },
    );
    return response != null && (response.statusCode == 200 || response.statusCode == 201 || response.statusCode == 202);
  }
}
