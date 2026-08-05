import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/data/api/abdm_server.dart';
import 'package:abdm_frontend/app/data/repository/m2/hip_care_context_repo.dart';
import 'package:abdm_frontend/util/api_endpoints.dart';

class PatientRegistryModel {
  final String abhaAddress;
  final String name;
  final String patientReference;
  final String patientDisplay;
  final String gender;
  final String dateOfBirth;
  final String mobile;
  final String hipId;
  final int careContextCount;

  PatientRegistryModel({
    required this.abhaAddress,
    required this.name,
    required this.patientReference,
    required this.patientDisplay,
    required this.gender,
    required this.dateOfBirth,
    required this.mobile,
    required this.hipId,
    required this.careContextCount,
  });

  factory PatientRegistryModel.fromJson(Map<String, dynamic> json) {
    return PatientRegistryModel(
      abhaAddress: json['abhaAddress'] ?? '',
      name: json['name'] ?? json['patientDisplay'] ?? 'Unknown',
      patientReference: json['patientReference'] ?? '',
      patientDisplay: json['patientDisplay'] ?? '',
      gender: json['gender'] ?? '',
      dateOfBirth: json['dateOfBirth'] ?? '',
      mobile: json['mobile'] ?? '',
      hipId: json['hipId'] ?? '',
      careContextCount: json['careContextCount'] ?? 0,
    );
  }
}

class PatientRegistryController extends GetxController {
  var patients = <PatientRegistryModel>[].obs;
  var isLoading = false.obs;
  var searchQuery = ''.obs;
  var errorMessage = ''.obs;

  List<PatientRegistryModel> get filtered {
    if (searchQuery.value.isEmpty) return patients;
    final q = searchQuery.value.toLowerCase();
    return patients.where((p) =>
        p.name.toLowerCase().contains(q) ||
        p.abhaAddress.toLowerCase().contains(q) ||
        p.mobile.contains(q)).toList();
  }

  @override
  void onInit() {
    super.onInit();
    fetchPatients();
  }

  Future<void> fetchPatients() async {
    isLoading.value = true;
    errorMessage.value = '';
    try {
      final response = await AbdmServer.getRequest(ApiEndpoints.getPatientList());
      if (response != null && response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        patients.value = data
            .map((e) => PatientRegistryModel.fromJson(e as Map<String, dynamic>))
            .toList();
      } else {
        errorMessage.value =
            'Failed to load patients (${response?.statusCode ?? 'no response'})';
      }
    } catch (e) {
      errorMessage.value = 'Error: $e';
    } finally {
      isLoading.value = false;
    }
  }

  var isLinkingMap = <String, bool>{}.obs;

  Future<void> linkCareContextForPatient({
    required String abhaAddress,
    required String visitRef,
    required String display,
    String? hiType,
    String? hipId,
  }) async {
    isLinkingMap[abhaAddress] = true;
    try {
      bool success = await HipCareContextRepo.linkCareContext(
        abhaAddress: abhaAddress,
        visitRef: visitRef,
        display: display,
        hiType: hiType ?? 'OPConsultation',
        requesterId: hipId,
      );

      if (success) {
        Get.snackbar(
          'Care Context Request Sent 🚀',
          'Link request for "$visitRef" submitted to ABDM Gateway. Patient notification initiated.',
          snackPosition: SnackPosition.TOP,
          backgroundColor: const Color(0xFF10B981),
          colorText: Colors.white,
          duration: const Duration(seconds: 4),
        );
        fetchPatients();
      } else {
        Get.snackbar(
          'Linking Failed',
          'Could not send care context link request for $abhaAddress.',
          snackPosition: SnackPosition.TOP,
          backgroundColor: const Color(0xFFEF4444),
          colorText: Colors.white,
        );
      }
    } catch (e) {
      Get.snackbar('Error', 'Linking failed: $e', snackPosition: SnackPosition.TOP);
    } finally {
      isLinkingMap[abhaAddress] = false;
    }
  }
}
