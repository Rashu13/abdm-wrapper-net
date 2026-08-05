import 'dart:convert';
import 'package:abdm_frontend/app/data/api/abdm_server.dart';
import 'package:abdm_frontend/util/api_endpoints.dart';

class ScanShareRepo {
  static Future<Map<String, dynamic>?> shareProfile({
    required String abhaAddress,
    required String name,
    required String gender,
    required int yearOfBirth,
    required String phoneNumber,
    required String hipId,
    required String counterId,
  }) async {
    final body = {
      'profile': {
        'patient': {
          'abhaAddress': abhaAddress,
          'name': name,
          'gender': gender,
          'yearOfBirth': yearOfBirth,
          'phoneNumber': phoneNumber,
        }
      },
      'metaData': {
        'context': counterId,
        'hipId': hipId,
      }
    };

    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.profileShare,
      body: body,
      customHeaders: {
        'X-HIP-ID': hipId,
      },
    );

    if (response != null && (response.statusCode == 200 || response.statusCode == 202)) {
      try {
        return jsonDecode(response.body) as Map<String, dynamic>;
      } catch (_) {
        return {'status': 'SUCCESS', 'message': 'Profile shared successfully'};
      }
    }
    return null;
  }

  static Future<List<dynamic>> fetchScanShareRequests() async {
    final response = await AbdmServer.getRequest(ApiEndpoints.getScanShareRequests);
    if (response != null && response.statusCode == 200) {
      try {
        return jsonDecode(response.body) as List<dynamic>;
      } catch (e) {
        print("Error parsing scan share requests: $e");
      }
    }
    return [];
  }
}
