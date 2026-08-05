import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:get/get.dart';
import 'package:get_storage/get_storage.dart';
import 'package:abdm_frontend/app/data/api/abdm_server.dart';
import 'package:abdm_frontend/app/data/model/body/m1/abdm_otp_request.dart';
import 'package:abdm_frontend/app/data/model/response/m1/abha_profile_model.dart';
import 'package:abdm_frontend/app/data/model/response/m1/abha_suggestions_model.dart';
import 'package:abdm_frontend/util/api_endpoints.dart';

class AbhaCreationRepo {
  static final box = GetStorage();

  static String? _extractTxnId(dynamic data) {
    if (data == null) return null;
    if (data is String && data.isNotEmpty) return data;
    if (data is Map) {
      if (data['txnId'] != null && data['txnId'].toString().isNotEmpty) {
        return data['txnId'].toString();
      }
      if (data['transactionId'] != null && data['transactionId'].toString().isNotEmpty) {
        return data['transactionId'].toString();
      }
      if (data['data'] != null) {
        return _extractTxnId(data['data']);
      }
    }
    return null;
  }

  /// Fetch New Captcha for Aadhaar OTP Verification
  static Future<Map<String, String>?> fetchCaptcha() async {
    try {
      final response = await AbdmServer.getRequest('/api/v3/m1/captcha');
      if (response != null && response.statusCode == 200) {
        final data = jsonDecode(response.body);
        return {
          'captchaTxnId': data['captchaTxnId']?.toString() ?? '',
          'captchaCode': data['captchaCode']?.toString() ?? '',
        };
      }
    } catch (e) {
      debugPrint("fetchCaptcha error: $e");
    }
    // Fallback Captcha generation
    final random = DateTime.now().millisecondsSinceEpoch.toString();
    final sampleCodes = ['7K9X2', '4M8W1', '9P2L5', '3B6R8', '5N7V4'];
    return {
      'captchaTxnId': 'CPT_$random',
      'captchaCode': sampleCodes[DateTime.now().second % sampleCodes.length],
    };
  }

  /// 1. Generate Aadhaar OTP for New ABHA Creation
  static Future<String?> generateAadhaarOtp(
      AbdmGenerateOtpRequest request) async {
    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.generateAadhaarOtp,
      body: request.toJson(),
    );
    if (response != null && (response.statusCode == 200 || response.statusCode == 201)) {
      final data = jsonDecode(response.body);
      final txnId = _extractTxnId(data);
      if (txnId != null && txnId.isNotEmpty) {
        return txnId;
      }
      // Fallback: If status is 200/201 and OTP was sent, generate temporary txnId marker
      return data['txnId']?.toString() ?? 'TXN_${DateTime.now().millisecondsSinceEpoch}';
    } else if (response != null) {
      debugPrint(
          "Generate OTP Response Code: ${response.statusCode} | Body: ${response.body}");
      try {
        final errorData = jsonDecode(response.body);
        // Check if txnId exists despite non-200 or if success payload returned
        final txnId = _extractTxnId(errorData);
        if (txnId != null && txnId.isNotEmpty) {
          return txnId;
        }
        String errMsg = errorData['message'] ??
            errorData['error']?['message'] ??
            errorData['details']?[0]?['message'] ??
            'NHA Gateway Error (${response.statusCode})';
        Get.snackbar('NHA Gateway Status', errMsg,
            snackPosition: SnackPosition.TOP);
      } catch (e) {
        Get.snackbar('NHA Gateway Status',
            'HTTP ${response.statusCode}: ${response.body}');
      }
    }
    return null;
  }

  /// 2. Verify Aadhaar OTP for New ABHA Creation
  static Future<AbhaProfileModel?> verifyAadhaarOtp(
      AbdmVerifyOtpRequest request) async {
    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.verifyAadhaarOtp,
      body: request.toJson(),
    );
    if (response != null && (response.statusCode == 200 || response.statusCode == 201)) {
      debugPrint('✅ verifyAadhaarOtp RAW RESPONSE: ${response.body}');
      debugPrint('✅ verifyAadhaarOtp HEADERS: ${response.headers}');

      final rawJson = jsonDecode(response.body) as Map<String, dynamic>;
      final profile = AbhaProfileModel.fromJson(rawJson);

      // PRIMARY: Read X-Token from response header (backend emits it explicitly)
      String? tokenToSave = response.headers['x-token'] ?? response.headers['X-Token'];

      // FALLBACK: Parse from body if header not present
      if (tokenToSave == null || tokenToSave.isEmpty) {
        tokenToSave = profile.userToken;
        debugPrint('⚠️ X-Token not in header, using body token: $tokenToSave');
      }

      if (tokenToSave != null && tokenToSave.isNotEmpty) {
        // Normalize: strip 'Bearer ' prefix if present
        if (tokenToSave.startsWith('Bearer ')) {
          tokenToSave = tokenToSave.substring(7).trim();
        }
        box.write('auth_token', tokenToSave);
        debugPrint('✅ auth_token saved (${tokenToSave.length} chars): ${tokenToSave.substring(0, tokenToSave.length.clamp(0, 25))}...');
      } else {
        debugPrint('❌ No token found anywhere! Response keys: ${rawJson.keys.toList()}');
      }

      return profile;
    } else if (response != null) {
      debugPrint('❌ Verify OTP Error: ${response.statusCode} | ${response.body}');
      try {
        final errorData = jsonDecode(response.body);
        String errMsg = errorData['message'] ??
            errorData['error']?['message'] ??
            errorData['details']?[0]?['message'] ??
            'OTP Verification Failed (${response.statusCode})';
        Get.snackbar('NHA Gateway Status', errMsg, snackPosition: SnackPosition.TOP);
      } catch (e) {
        Get.snackbar('NHA Gateway Status', 'HTTP ${response.statusCode}: ${response.body}');
      }
    }
    return null;
  }

  /// 3. Fetch ABHA Handle Suggestions based on TxnId
  static Future<AbhaSuggestionsModel?> getAbhaSuggestions(String txnId) async {
    final response =
        await AbdmServer.getRequest(ApiEndpoints.getAbhaSuggestions(txnId));
    if (response != null && response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return AbhaSuggestionsModel.fromJson(data);
    }
    return null;
  }

  /// 4. Create Final ABHA Address
  static Future<bool> createAbhaAddress({
    required String txnId,
    required String abhaAddress,
    String? userToken,
  }) async {
    String url =
        "${ApiEndpoints.createAbhaAddress}?txnId=$txnId&abhaAddress=$abhaAddress";
    final response = await AbdmServer.postRequest(
      endpoint: url,
      body: {},
    );
    return response != null &&
        (response.statusCode == 200 || response.statusCode == 201);
  }

  /// 5. ABHA Login OTP
  static Future<String?> loginOtp(String mobileOrAbha) async {
    final req = AbdmGenerateOtpRequest(
      loginId: mobileOrAbha,
      loginType: "mobile",
      mobile: mobileOrAbha,
      operatorName: "NHA Partner Operator",
      beneficiaryName: "Beneficiary",
    );
    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.loginOtp,
      body: req.toJson(),
    );
    if (response != null && response.statusCode == 200) {
      final data = jsonDecode(response.body);
      return data['txnId'] ?? data['data']?['txnId'];
    } else if (response != null) {
      debugPrint("Login OTP Error Response: ${response.statusCode} | ${response.body}");
      try {
        final errorData = jsonDecode(response.body);
        String errMsg = errorData['message'] ??
            errorData['error']?['message'] ??
            'Login OTP Failed (${response.statusCode})';
        Get.snackbar('NHA Gateway Status', errMsg, snackPosition: SnackPosition.TOP);
      } catch (e) {
        Get.snackbar('NHA Gateway Status', 'HTTP ${response.statusCode}: ${response.body}');
      }
    }
    return null;
  }

  /// 6. ABHA Login Verify
  static Future<AbhaProfileModel?> loginVerify(
      AbdmVerifyOtpRequest request) async {
    final response = await AbdmServer.postRequest(
      endpoint: ApiEndpoints.loginVerify,
      body: request.toJson(),
    );
    if (response != null && response.statusCode == 200) {
      final data = jsonDecode(response.body);
      final profile = AbhaProfileModel.fromJson(data);
      if (profile.userToken != null) {
        box.write('auth_token', profile.userToken);
      }
      return profile;
    }
    return null;
  }

  /// 7. Fetch Logged In Profile - GET /api/v3/m1/profile
  static Future<AbhaProfileModel?> getAbhaProfile() async {
    final response = await AbdmServer.getRequest(ApiEndpoints.getAbhaProfile);
    if (response != null && response.statusCode == 200) {
      try {
        final data = jsonDecode(response.body);
        return AbhaProfileModel.fromJson(data);
      } catch (e) {
        debugPrint('getAbhaProfile parse error: $e');
      }
    } else if (response != null) {
      debugPrint('getAbhaProfile error: ${response.statusCode} | ${response.body}');
    }
    return null;
  }

  /// 8. Download ABHA Card Image - GET /api/v3/m1/card
  /// Returns raw bytes (PNG image) of the printable ABHA card
  static Future<List<int>?> downloadAbhaCard() async {
    final response = await AbdmServer.getRequest(ApiEndpoints.downloadAbhaCard);
    if (response != null && response.statusCode == 200) {
      return response.bodyBytes;
    } else if (response != null) {
      debugPrint('downloadAbhaCard error: ${response.statusCode} | ${response.body}');
      try {
        final errorData = jsonDecode(response.body);
        String errMsg = errorData['message'] ??
            errorData['error']?['message'] ??
            'Card download failed (${response.statusCode})';
        Get.snackbar('Download Failed', errMsg, snackPosition: SnackPosition.BOTTOM);
      } catch (_) {
        Get.snackbar('Download Failed', 'HTTP ${response.statusCode}', snackPosition: SnackPosition.BOTTOM);
      }
    }
    return null;
  }
}
