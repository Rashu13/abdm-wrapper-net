import 'dart:convert';
import 'package:get_storage/get_storage.dart';
import 'package:http/http.dart' as http;

class AbdmServer {
  static final box = GetStorage();

  static Map<String, String> _getHeaders([Map<String, String>? customHeaders]) {
    String? token = box.read('auth_token');
    final headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'X-CM-ID': 'sbx',
      if (token != null && token.isNotEmpty) 'Authorization': 'Bearer $token',
      if (token != null && token.isNotEmpty) 'X-Token': token,
    };
    if (customHeaders != null) {
      headers.addAll(customHeaders);
    }
    return headers;
  }

  static Future<http.Response?> getRequest([
    String? endpointPositional,
    String? endpoint,
    Map<String, String>? customHeaders,
  ]) async {
    final String url = (endpointPositional != null && endpointPositional.isNotEmpty)
        ? endpointPositional
        : (endpoint ?? '');
    try {
      final response = await http.get(
        Uri.parse(url),
        headers: _getHeaders(customHeaders),
      );
      return response;
    } catch (e) {
      print('GET Error ($url): $e');
      return null;
    }
  }

  static Future<http.Response?> postRequest({
    required String endpoint,
    required Map<String, dynamic> body,
    Map<String, String>? customHeaders,
  }) async {
    try {
      final response = await http.post(
        Uri.parse(endpoint),
        headers: _getHeaders(customHeaders),
        body: jsonEncode(body),
      );
      return response;
    } catch (e) {
      print('POST Error ($endpoint): $endpoint - $e');
      return null;
    }
  }

  static Future<http.Response?> putRequest({
    required String endpoint,
    required dynamic body,
    Map<String, String>? customHeaders,
  }) async {
    try {
      final response = await http.put(
        Uri.parse(endpoint),
        headers: _getHeaders(customHeaders),
        body: jsonEncode(body),
      );
      return response;
    } catch (e) {
      print('PUT Error ($endpoint): $endpoint - $e');
      return null;
    }
  }
}
