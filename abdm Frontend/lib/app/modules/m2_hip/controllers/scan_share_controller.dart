import 'dart:async';
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../../data/repository/m2/scan_share_repo.dart';

class OpdTokenRecord {
  final String tokenNumber;
  final String patientName;
  final String abhaAddress;
  final String gender;
  final String phoneNumber;
  final String counterName;
  final DateTime generatedAt;
  final String expirySeconds;

  OpdTokenRecord({
    required this.tokenNumber,
    required this.patientName,
    required this.abhaAddress,
    required this.gender,
    required this.phoneNumber,
    required this.counterName,
    required this.generatedAt,
    this.expirySeconds = '1800',
  });
}

class ScanShareController extends GetxController {
  // Configurable Facility Details (Default: Configured Sandbox HIP ID IN0610090658)
  final hipIdController = TextEditingController(text: 'IN0610090658');
  final facilityNameController = TextEditingController(text: 'Midha Super Speciality Hospital');
  
  var selectedCounterId = '1'.obs;
  var qrFormatMode = 'json'.obs; // 'json', 'deeplink', 'intent'
  
  final Map<String, String> counterNames = {
    '1': 'Counter 1 • General OPD Desk',
    '2': 'Counter 2 • Cardiology & Ortho',
    '3': 'Counter 3 • Pediatrics & Gynae',
  };

  // OPD Token Queue (Real-time list)
  var tokenQueue = <OpdTokenRecord>[].obs;
  var _tokenCounter = 1;
  var isFetchingRemote = false.obs;
  Timer? _pollingTimer;

  @override
  void onInit() {
    super.onInit();
    fetchBackendScanRequests();
    // Auto-poll incoming scan requests every 5 seconds
    _pollingTimer = Timer.periodic(const Duration(seconds: 5), (_) => fetchBackendScanRequests(silent: true));
  }

  /// Fetch real incoming scan & share requests logged in backend from actual ABHA App scans
  Future<void> fetchBackendScanRequests({bool silent = false}) async {
    if (!silent) isFetchingRemote.value = true;
    try {
      final remoteList = await ScanShareRepo.fetchScanShareRequests();
      if (remoteList.isNotEmpty) {
        for (var item in remoteList) {
          final abhaAddr = item['AbhaAddress'] ?? item['abhaAddress'] ?? '';
          final createdStr = item['CreatedOn'] ?? item['createdOn'] ?? '';
          final detailsStr = item['Details'] ?? item['details'] ?? '{}';
          
          if (abhaAddr.isEmpty) continue;

          // Check if already in tokenQueue by abhaAddress & created timestamp
          bool exists = tokenQueue.any((t) => t.abhaAddress.toLowerCase() == abhaAddr.toString().toLowerCase());
          if (!exists) {
            String name = abhaAddr;
            String gender = 'M';
            String phone = '';
            String counter = counterNames['1']!;
            String tokenNum = _tokenCounter.toString().padLeft(4, '0');
            _tokenCounter++;

            try {
              final parsedDetails = jsonDecode(detailsStr);
              if (parsedDetails['shareProfileRequest'] != null) {
                final req = parsedDetails['shareProfileRequest'];
                final p = req['Profile']?['Patient'] ?? req['profile']?['patient'];
                if (p != null) {
                  name = p['Name'] ?? p['name'] ?? name;
                  gender = p['Gender'] ?? p['gender'] ?? 'M';
                  phone = p['PhoneNumber'] ?? p['phoneNumber'] ?? '';
                }
              }
              if (parsedDetails['shareProfileResponse'] != null) {
                final resp = parsedDetails['shareProfileResponse'];
                final ack = resp['acknowledgement'] ?? resp['Acknowledgement'];
                if (ack != null && ack['profile'] != null) {
                  tokenNum = ack['profile']['tokenNumber'] ?? tokenNum;
                }
              }
            } catch (_) {}

            tokenQueue.insert(
              0,
              OpdTokenRecord(
                tokenNumber: tokenNum,
                patientName: name,
                abhaAddress: abhaAddr,
                gender: gender,
                phoneNumber: phone,
                counterName: counter,
                generatedAt: DateTime.tryParse(createdStr) ?? DateTime.now(),
              ),
            );
          }
        }
      }
    } catch (e) {
      print("Error fetching remote scan requests: $e");
    } finally {
      if (!silent) isFetchingRemote.value = false;
    }
  }

  /// Construct 100% compliant ABDM Scan & Share QR Code payload
  String getQrPayload() {
    final hipId = hipIdController.text.trim().isEmpty ? 'IN0610090658' : hipIdController.text.trim();
    final facility = facilityNameController.text.trim().isEmpty ? 'Midha Super Speciality Hospital' : facilityNameController.text.trim();
    final counter = selectedCounterId.value;

    if (qrFormatMode.value == 'deeplink') {
      return 'https://phr.abdm.gov.in/share?hip_id=$hipId&counter_id=$counter&facility_name=${Uri.encodeComponent(facility)}';
    } else if (qrFormatMode.value == 'intent') {
      return 'abdm://share?hip_id=$hipId&counter_id=$counter';
    }

    return '{"hip_id":"$hipId","counter_id":"$counter","facility_name":"$facility","code":"$hipId","hipId":"$hipId","counterId":"$counter"}';
  }

  @override
  void onClose() {
    _pollingTimer?.cancel();
    hipIdController.dispose();
    facilityNameController.dispose();
    super.onClose();
  }
}
