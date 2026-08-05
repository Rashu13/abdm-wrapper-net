import 'dart:convert';
import 'package:abdm_frontend/app/data/api/abdm_server.dart';
import 'package:abdm_frontend/app/data/model/response/m2/care_context_model.dart';
import 'package:abdm_frontend/util/api_endpoints.dart';

class HipCareContextRepo {
  static Future<List<CareContextModel>> getLinkedCareContexts({String requestId = "REQ_DEFAULT"}) async {
    try {
      final response = await AbdmServer.getRequest(ApiEndpoints.getLinkStatus(requestId));
      if (response != null && (response.statusCode == 200 || response.statusCode == 202)) {
        final data = jsonDecode(response.body);
        if (data['contexts'] != null) {
          return (data['contexts'] as List)
              .map((e) => CareContextModel.fromJson(e))
              .toList();
        }
      }
    } catch (e) {
      print("getLinkedCareContexts error: $e");
    }
    // Return empty list on network error / initial state
    return [];
  }

  static Future<bool> linkCareContext({
    required String abhaAddress,
    required String visitRef,
    String? display,
    String? requesterId,
  }) async {
    try {
      final reqId = "REQ_${DateTime.now().millisecondsSinceEpoch}";
      final response = await AbdmServer.postRequest(
        endpoint: ApiEndpoints.linkCareContext,
        body: {
          'requestId': reqId,
          'requesterId': requesterId ?? 'IN0610090658',
          'abhaAddress': abhaAddress,
          'patientAbha': abhaAddress,
          'careContexts': [
            {
              'referenceNumber': visitRef,
              'display': display ?? 'OPD Consultation ($visitRef)',
              'hiType': 'OPConsultation',
              'isLinked': false,
            }
          ]
        },
      );
      return response != null && (response.statusCode == 200 || response.statusCode == 201 || response.statusCode == 202);
    } catch (e) {
      print("linkCareContext error: $e");
      return false;
    }
  }

  static Future<bool> registerPatientInHipDb({
    required String abhaAddress,
    required String name,
    required String gender,
    required String dob,
    required String mobile,
    String? hipId,
  }) async {
    try {
      final response = await AbdmServer.putRequest(
        endpoint: ApiEndpoints.addPatients,
        body: [
          {
            'abhaAddress': abhaAddress,
            'name': name,
            'gender': gender,
            'dateOfBirth': dob,
            'patientMobile': mobile,
            if (hipId != null && hipId.isNotEmpty) 'hipId': hipId,
            'patientReference': abhaAddress,
            'patientDisplay': name,
          }
        ],
      );
      return response != null && (response.statusCode == 200 || response.statusCode == 201 || response.statusCode == 202);
    } catch (e) {
      print("registerPatientInHipDb error: $e");
      return false;
    }
  }
}
