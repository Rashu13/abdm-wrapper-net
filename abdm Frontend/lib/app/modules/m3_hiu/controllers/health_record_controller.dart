import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../../data/api/fhir_parser.dart';
import '../../../data/model/response/m3/consent_model.dart';
import '../../../data/repository/m3/hiu_health_record_repo.dart';

class HealthRecordController extends GetxController {
  var isLoadingConsents = false.obs;
  var isSubmittingConsent = false.obs;
  var isFetchingRecords = false.obs;
  var errorMessage = ''.obs;

  var consents = <ConsentModel>[].obs;
  var consentRequests = <HiuConsentRequestModel>[].obs;
  var fhirRecords = <FhirRecordItem>[].obs;
  var selectedConsentId = ''.obs;

  // Consent Form State
  var abhaAddressInput = 'user_40893@sbx'.obs;
  var selectedPurposeCode = 'CAREMGT'.obs;
  var selectedHiTypes = <String>[
    'Prescription',
    'DiagnosticReport',
    'OPConsultation',
    'DischargeSummary',
  ].obs;

  final List<Map<String, String>> purposeCodeList = [
    {'code': 'CAREMGT', 'label': 'Care Management'},
    {'code': 'BTG', 'label': 'Break the Glass (Emergency)'},
    {'code': 'PUBHLTH', 'label': 'Public Health'},
    {'code': 'HPAYMT', 'label': 'Healthcare Payment'},
    {'code': 'DSRCH', 'label': 'Disease Research'},
    {'code': 'PATRQT', 'label': 'Patient Request'},
  ];

  final List<String> availableHiTypes = [
    'Prescription',
    'DiagnosticReport',
    'OPConsultation',
    'DischargeSummary',
    'ImmunizationRecord',
    'WellnessRecord',
    'Invoice',
  ];

  @override
  void onInit() {
    super.onInit();
    fetchConsentRequests();
  }

  void toggleHiType(String hiType) {
    if (selectedHiTypes.contains(hiType)) {
      if (selectedHiTypes.length > 1) {
        selectedHiTypes.remove(hiType);
      }
    } else {
      selectedHiTypes.add(hiType);
    }
  }

  Future<void> fetchConsentRequests() async {
    isLoadingConsents.value = true;
    errorMessage.value = '';
    try {
      var requests = await HiuHealthRecordRepo.getAllConsentRequests();
      consentRequests.value = requests;

      var list = await HiuHealthRecordRepo.getConsents();
      consents.value = list;
    } catch (e) {
      errorMessage.value = 'Error loading consent requests: $e';
    } finally {
      isLoadingConsents.value = false;
    }
  }

  Future<void> submitConsentRequest({
    required String abhaAddress,
    required String fromDate,
    required String toDate,
    required String eraseAt,
  }) async {
    isSubmittingConsent.value = true;
    bool success = await HiuHealthRecordRepo.initiateConsentRequest(
      abhaAddress: abhaAddress.trim(),
      purposeCode: selectedPurposeCode.value,
      hiTypes: selectedHiTypes.toList(),
      fromDate: fromDate,
      toDate: toDate,
      eraseAt: eraseAt,
    );
    isSubmittingConsent.value = false;

    if (success) {
      Get.snackbar(
        'Consent Requested 🚀',
        'Consent request submitted to ABDM Gateway for $abhaAddress. Patient notification sent.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFF10B981),
        colorText: Colors.white,
        duration: const Duration(seconds: 4),
      );
      fetchConsentRequests();
    } else {
      Get.snackbar(
        'Request Failed',
        'Could not submit consent request. Check ABDM Gateway connection.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFFEF4444),
        colorText: Colors.white,
      );
    }
  }

  Future<void> fetchAndDecryptRecords(String consentId) async {
    selectedConsentId.value = consentId;
    isFetchingRecords.value = true;
    var records = await HiuHealthRecordRepo.fetchDecryptedRecords(consentId);
    fhirRecords.value = records;
    isFetchingRecords.value = false;
  }
}
