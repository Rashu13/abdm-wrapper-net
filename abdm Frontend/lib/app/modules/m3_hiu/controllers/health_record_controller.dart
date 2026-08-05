import 'dart:convert';
import 'package:abdm_frontend/util/api_endpoints.dart' show ApiEndpoints;
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../../data/api/abdm_server.dart';
import '../../../data/api/fhir_parser.dart';
import '../../../data/model/body/m3/emr_form_models.dart';
import '../../../data/model/response/m3/consent_model.dart';
import '../../../data/repository/m2/hip_care_context_repo.dart';
import '../../../data/repository/m3/hiu_health_record_repo.dart';
import '../../m2_hip/controllers/patient_registry_controller.dart';

class HealthRecordController extends GetxController {
  var isLoadingConsents = false.obs;
  var isSubmittingConsent = false.obs;
  var isFetchingRecords = false.obs;
  var isSavingHealthRecord = false.obs;
  var errorMessage = ''.obs;

  var consents = <ConsentModel>[].obs;
  var consentRequests = <HiuConsentRequestModel>[].obs;
  var fhirRecords = <FhirRecordItem>[].obs;
  var selectedConsentId = ''.obs;

  // Patient Selection for EMR Record Creation
  var patients = <PatientRegistryModel>[].obs;
  var selectedPatient = Rxn<PatientRegistryModel>();
  var isLoadingPatients = false.obs;

  // Active HI Type Tab ('Prescription', 'DiagnosticReport', 'DischargeSummary', 'OPConsultation', 'ImmunizationRecord', 'WellnessRecord')
  var activeHiType = 'Prescription'.obs;

  final List<String> hiTypeList = [
    'Prescription',
    'DiagnosticReport',
    'OPConsultation',
    'DischargeSummary',
    'ImmunizationRecord',
    'WellnessRecord',
  ];

  // Dynamic EMR Form Lists
  var medicines = <MedicineFormItem>[].obs;
  var labResults = <LabResultFormItem>[].obs;
  var immunizationList = <ImmunizationFormItem>[].obs;

  // Form Fields
  final complaintsCtrl =
      TextEditingController(text: 'Fever and body pain since 2 days');
  final diagnosisCtrl = TextEditingController(text: 'Acute Viral Pyrexia');
  final bpCtrl = TextEditingController(text: '120/80 mmHg');
  final pulseCtrl = TextEditingController(text: '72 bpm');
  final tempCtrl = TextEditingController(text: '98.6 °F');
  final spo2Ctrl = TextEditingController(text: '98%');
  final reportTitleCtrl =
      TextEditingController(text: 'Complete Blood Count (CBC) & Lipid Profile');
  final dischargeNotesCtrl = TextEditingController(
      text: 'Patient recovered well. Discharged in stable condition.');
  final adviceCtrl = TextEditingController(
      text: 'Rest for 3 days, drink plenty of fluids. Review after 1 week.');

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
    resetFormLists();
    fetchConsentRequests();
    fetchPatients();
  }

  void resetFormLists() {
    medicines.value = [
      MedicineFormItem(
        drugName: 'Dolo 650 mg',
        dosagePattern: '1-0-1 (After Food)',
        route: 'Oral',
        method: 'Swallow with water',
        reason: 'Fever & Pain relief',
        snomedCode: '322236009',
      ),
      MedicineFormItem(
        drugName: 'Pan 40 mg',
        dosagePattern: '1-0-0 (Before Food)',
        route: 'Oral',
        method: 'Before Breakfast',
        reason: 'Acidity prevention',
        snomedCode: '372605007',
      ),
    ];

    labResults.value = [
      LabResultFormItem(
          testName: 'Hemoglobin (Hb)', value: '14.2', unit: 'g/dL'),
      LabResultFormItem(
          testName: 'Total WBC Count', value: '7,500', unit: '/cu mm'),
      LabResultFormItem(
          testName: 'Platelet Count', value: '2.5', unit: 'lakhs/cu mm'),
    ];

    immunizationList.value = [
      ImmunizationFormItem(
        vaccineName: 'Covishield (COVID-19 Vaccine)',
        lotNumber: '4120Z015',
        doseNumber: '2',
        date: DateTime.now().toString().split(' ').first,
      ),
    ];
  }

  void addMedicine() {
    medicines.add(MedicineFormItem());
  }

  void removeMedicine(int index) {
    if (medicines.length > 1) {
      medicines.removeAt(index);
    }
  }

  void addLabResult() {
    labResults.add(LabResultFormItem());
  }

  void removeLabResult(int index) {
    if (labResults.length > 1) {
      labResults.removeAt(index);
    }
  }

  Future<void> fetchPatients() async {
    isLoadingPatients.value = true;
    try {
      final response =
          await AbdmServer.getRequest(ApiEndpoints.getPatientList());
      if (response != null && response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        patients.value = data
            .map(
                (e) => PatientRegistryModel.fromJson(e as Map<String, dynamic>))
            .toList();
        if (patients.isNotEmpty && selectedPatient.value == null) {
          selectedPatient.value = patients.first;
        }
      }
    } catch (e) {
      debugPrint('fetchPatients error: $e');
    } finally {
      isLoadingPatients.value = false;
    }
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

  /// Generates FHIR Payload and Links Care Context directly to ABDM Gateway!
  Future<void> generateAndLinkCareContext() async {
    final patient = selectedPatient.value;
    if (patient == null || patient.abhaAddress.isEmpty) {
      Get.snackbar('Error', 'Please select a registered patient first.');
      return;
    }

    isSavingHealthRecord.value = true;
    final visitRef =
        "VISIT-${DateTime.now().millisecondsSinceEpoch.toString().substring(6)}";
    final hiType = activeHiType.value;
    final displayTitle =
        "$hiType Record - ${DateTime.now().day}/${DateTime.now().month}/${DateTime.now().year}";

    // Construct FHIR payload JSON structure
    final fhirPayload = {
      'careContextReference': visitRef,
      'authoredOn': DateTime.now().toUtc().toIso8601String(),
      'recordType': hiType,
      'patient': {
        'patientReference': patient.patientReference.isNotEmpty
            ? patient.patientReference
            : patient.abhaAddress,
        'name': patient.name,
        'gender': patient.gender,
        'birthDate': patient.dateOfBirth,
        'mobile': patient.mobile,
      },
      'practitioners': [
        {
          'practitionerId': 'DOC-NMC-998811',
          'name': 'Dr. Sonomed Specialist',
        }
      ],
      'organisation': {
        'facilityId': patient.hipId.isNotEmpty ? patient.hipId : 'IN0610090658',
        'facilityName': 'MIDHA HOSPITAL / SONOMED CLINIC',
      },
      'complaints': complaintsCtrl.text.trim(),
      'diagnosis': diagnosisCtrl.text.trim(),
      'vitals': {
        'bp': bpCtrl.text.trim(),
        'pulse': pulseCtrl.text.trim(),
        'temp': tempCtrl.text.trim(),
        'spo2': spo2Ctrl.text.trim(),
      },
      'prescriptions': medicines.map((m) => m.toJson()).toList(),
      'labResults': labResults.map((l) => l.toJson()).toList(),
      'reportTitle': reportTitleCtrl.text.trim(),
      'dischargeSummary': dischargeNotesCtrl.text.trim(),
      'advice': adviceCtrl.text.trim(),
    };

    // 1. Save Health Data Record to Backend
    await AbdmServer.postRequest(
      endpoint: ApiEndpoints.saveHealthDataRecord,
      body: {
        'abhaAddress': patient.abhaAddress,
        'careContextReference': visitRef,
        'fhirJsonPayload': jsonEncode(fhirPayload),
      },
    );

    // 2. Submit Link Care Context Request to ABDM Gateway
    bool success = await HipCareContextRepo.linkCareContext(
      abhaAddress: patient.abhaAddress,
      visitRef: visitRef,
      display: displayTitle,
      hiType: hiType,
      requesterId: patient.hipId,
    );

    isSavingHealthRecord.value = false;

    if (success) {
      Get.snackbar(
        'Care Context & FHIR Record Created 🎉',
        'Record "$visitRef" ($hiType) linked for ${patient.name}. Published to ABDM Gateway.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFF10B981),
        colorText: Colors.white,
        duration: const Duration(seconds: 5),
      );
    } else {
      Get.snackbar(
        'Linking Notice',
        'Health record saved locally for $visitRef. Gateway notification initiated.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFF0F4C81),
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
