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

class SavedRecordModel {
  final String visitRef;
  final String patientName;
  final String abhaAddress;
  final String hiType;
  final String createdTime;
  final Map<String, dynamic> fhirPayload;
  bool isLinked;

  SavedRecordModel({
    required this.visitRef,
    required this.patientName,
    required this.abhaAddress,
    required this.hiType,
    required this.createdTime,
    required this.fhirPayload,
    this.isLinked = false,
  });
}

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
  var savedLocalRecords = <SavedRecordModel>[].obs;

  // Active HI Type Tab ('Prescription', 'DiagnosticReport', 'DischargeSummary', 'OPConsultation', 'ImmunizationRecord', 'WellnessRecord')
  var activeHiType = 'OPConsultation'.obs;

  final List<String> hiTypeList = [
    'OPConsultation',
    'Prescription',
    'DiagnosticReport',
    'DischargeSummary',
    'ImmunizationRecord',
    'WellnessRecord',
  ];

  // OP Consultation State (Matching React HealthRecordsPage.tsx)
  var encounterType = 'Outpatient'.obs;
  final opHeightCtrl = TextEditingController(text: '170');
  final opWeightCtrl = TextEditingController(text: '68');
  final opBmiCtrl = TextEditingController(text: '23.5');

  var vitalsList = <VitalFormItem>[].obs;
  var complaintsList = <String>[].obs;
  var allergiesList = <AllergyFormItem>[].obs;
  var medicalHistoryList = <String>[].obs;
  final opObservationResultCtrl = TextEditingController(
      text:
          'General examination normal. Chest clear, S1 S2 heard. Abdomen soft.');

  // Wellness Record State
  final wellRespRateCtrl = TextEditingController(text: '16');
  final wellHeartRateCtrl = TextEditingController(text: '72');
  final wellSpo2Ctrl = TextEditingController(text: '98');
  final wellTempCtrl = TextEditingController(text: '98.6');
  final wellSysBpCtrl = TextEditingController(text: '120');
  final wellDiaBpCtrl = TextEditingController(text: '80');
  final wellHeightCtrl = TextEditingController(text: '170');
  final wellWeightCtrl = TextEditingController(text: '68');
  final wellBmiCtrl = TextEditingController(text: '23.5');
  final wellWaistCtrl = TextEditingController(text: '80');
  var wellMenarcheAge = '13'.obs;
  var wellLmpDate = ''.obs;
  var wellDietType = 'veg'.obs;
  var wellTobaccoUse = 'no'.obs;
  var wellAlcoholConsumption = 'no'.obs;
  final wellOtherObsCtrl = TextEditingController(
      text: 'Routine health checkup. Patient is fit and healthy.');

  // Dynamic EMR Form Lists
  var medicines = <MedicineFormItem>[].obs;
  var labResults = <LabResultFormItem>[].obs;
  var immunizationList = <ImmunizationFormItem>[].obs;

  // Form Fields
  final diagnosisCtrl = TextEditingController(text: 'Acute Viral Pyrexia');
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

    ever(selectedPatient, (p) {
      if (p != null) {
        fetchSavedHealthRecords(p.abhaAddress);
      }
    });
  }

  void calculateBmi() {
    try {
      final h = double.tryParse(opHeightCtrl.text) ?? 0;
      final w = double.tryParse(opWeightCtrl.text) ?? 0;
      if (h > 0 && w > 0) {
        final hm = h / 100.0;
        opBmiCtrl.text = (w / (hm * hm)).toStringAsFixed(1);
      }
    } catch (_) {}
  }

  void calculateWellBmi() {
    try {
      final h = double.tryParse(wellHeightCtrl.text) ?? 0;
      final w = double.tryParse(wellWeightCtrl.text) ?? 0;
      if (h > 0 && w > 0) {
        final hm = h / 100.0;
        wellBmiCtrl.text = (w / (hm * hm)).toStringAsFixed(1);
      }
    } catch (_) {}
  }

  void resetFormLists() {
    vitalsList.value = [
      VitalFormItem(vitalName: 'Blood Pressure', value: '120/80', unit: 'mmHg'),
      VitalFormItem(vitalName: 'Pulse Rate', value: '72', unit: 'bpm'),
      VitalFormItem(vitalName: 'Body Temperature', value: '98.6', unit: '°F'),
      VitalFormItem(vitalName: 'SpO2 Oxygen', value: '98', unit: '%'),
    ];

    complaintsList.value = [
      'Fever with chills for 2 days',
      'Mild headache and body fatigue',
    ];

    allergiesList.value = [
      AllergyFormItem(
          allergyName: 'Penicillin', type: 'medication', status: 'active'),
    ];

    medicalHistoryList.value = [
      'Type 2 Diabetes Mellitus (Under control)',
    ];

    medicines.value = [
      MedicineFormItem(
        drugName: 'Dolo 650 mg',
        dosagePattern: 'Morning & Night (1-0-1)',
        route: 'Oral',
        method: 'After Food',
        reason: 'Fever & Pain relief',
        snomedCode: '322236009',
      ),
      MedicineFormItem(
        drugName: 'Pan 40 mg',
        dosagePattern: 'Morning Only (1-0-0)',
        route: 'Oral',
        method: 'Before Food',
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

  void addVital() => vitalsList.add(VitalFormItem());
  void removeVital(int idx) {
    if (vitalsList.length > 1) vitalsList.removeAt(idx);
  }

  void addComplaint() => complaintsList.add('');
  void removeComplaint(int idx) {
    if (complaintsList.length > 1) complaintsList.removeAt(idx);
  }

  void addAllergy() => allergiesList.add(AllergyFormItem());
  void removeAllergy(int idx) {
    if (allergiesList.length > 1) allergiesList.removeAt(idx);
  }

  void addMedicalHistory() => medicalHistoryList.add('');
  void removeMedicalHistory(int idx) {
    if (medicalHistoryList.length > 1) medicalHistoryList.removeAt(idx);
  }

  void addMedicine() => medicines.add(MedicineFormItem());
  void removeMedicine(int index) {
    if (medicines.length > 1) medicines.removeAt(index);
  }

  void addLabResult() => labResults.add(LabResultFormItem());
  void removeLabResult(int index) {
    if (labResults.length > 1) labResults.removeAt(index);
  }

  void addImmunization() => immunizationList.add(ImmunizationFormItem());
  void removeImmunization(int index) {
    if (immunizationList.length > 1) immunizationList.removeAt(index);
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
        if (patients.isNotEmpty) {
          selectedPatient.value = patients.first;
          fetchSavedHealthRecords(patients.first.abhaAddress);
        }
      }
    } catch (e) {
      debugPrint('fetchPatients error: $e');
    } finally {
      isLoadingPatients.value = false;
    }
  }

  /// Fetches previously saved health data records from Backend DB for the selected patient
  Future<void> fetchSavedHealthRecords(String abhaAddress) async {
    if (abhaAddress.isEmpty) return;
    try {
      final endpoint =
          "${ApiEndpoints.saveHealthDataRecord}?abhaAddress=$abhaAddress";
      final response = await AbdmServer.getRequest(endpoint);
      if (response != null && response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        savedLocalRecords.value = data.map((e) {
          final map = e as Map<String, dynamic>;
          final ref = map['careContextReference'] ??
              map['careContextRef'] ??
              map['visitRef'] ??
              'VISIT-EX';
          final hiType =
              map['recordType'] ?? map['hiType'] ?? 'OPConsultation';
          final isLinked =
              map['isLinked'] == true || map['status'] == 'LINKED';
          final rawDate =
              map['createdAt'] ?? map['createdOn'] ?? map['authoredOn'];
          String createdOn = 'Saved Record';
          if (rawDate != null) {
            try {
              final dt = DateTime.parse(rawDate.toString()).toLocal();
              createdOn =
                  "${dt.day}/${dt.month}/${dt.year} ${dt.hour}:${dt.minute.toString().padLeft(2, '0')}";
            } catch (_) {
              createdOn = rawDate.toString();
            }
          }

          Map<String, dynamic> fhir = {};
          if (map['fhirJsonPayload'] != null) {
            try {
              fhir = jsonDecode(map['fhirJsonPayload']) as Map<String, dynamic>;
            } catch (_) {}
          }

          return SavedRecordModel(
            visitRef: ref.toString(),
            patientName: selectedPatient.value?.name ?? 'Patient',
            abhaAddress: abhaAddress,
            hiType: hiType.toString(),
            createdTime: createdOn,
            fhirPayload: fhir,
            isLinked: isLinked,
          );
        }).toList();
      }
    } catch (e) {
      debugPrint('fetchSavedHealthRecords error: $e');
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

  /// Saves FHIR Record locally without linking to ABDM Gateway
  Future<void> saveRecordLocally() async {
    final patient = selectedPatient.value;
    if (patient == null || patient.abhaAddress.isEmpty) {
      Get.snackbar('Error', 'Please select a registered patient first.');
      return;
    }

    isSavingHealthRecord.value = true;
    final visitRef =
        "VISIT-${DateTime.now().millisecondsSinceEpoch.toString().substring(6)}";
    final hiType = activeHiType.value;

    final fhirPayload = {
      'careContextReference': visitRef,
      'authoredOn': DateTime.now().toUtc().toIso8601String(),
      'recordType': hiType,
      'encounterType': encounterType.value,
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
      'bodyMeasurements': {
        'heightCm': opHeightCtrl.text,
        'weightKg': opWeightCtrl.text,
        'bmi': opBmiCtrl.text,
      },
      'vitals': vitalsList.map((v) => v.toJson()).toList(),
      'complaints': complaintsList.where((c) => c.trim().isNotEmpty).toList(),
      'clinicalObservation': opObservationResultCtrl.text.trim(),
      'allergies': allergiesList.map((a) => a.toJson()).toList(),
      'medicalHistory':
          medicalHistoryList.where((m) => m.trim().isNotEmpty).toList(),
      'diagnosis': diagnosisCtrl.text.trim(),
      'prescriptions': medicines.map((m) => m.toJson()).toList(),
      'labResults': labResults.map((l) => l.toJson()).toList(),
      'reportTitle': reportTitleCtrl.text.trim(),
      'dischargeSummary': dischargeNotesCtrl.text.trim(),
      'advice': adviceCtrl.text.trim(),
    };

    try {
      await AbdmServer.postRequest(
        endpoint: ApiEndpoints.saveHealthDataRecord,
        body: {
          'abhaAddress': patient.abhaAddress,
          'careContextReference': visitRef,
          'fhirJsonPayload': jsonEncode(fhirPayload),
        },
      );

      savedLocalRecords.insert(
        0,
        SavedRecordModel(
          visitRef: visitRef,
          patientName: patient.name,
          abhaAddress: patient.abhaAddress,
          hiType: hiType,
          createdTime:
              "${DateTime.now().day}/${DateTime.now().month}/${DateTime.now().year} ${DateTime.now().hour}:${DateTime.now().minute.toString().padLeft(2, '0')}",
          fhirPayload: fhirPayload,
          isLinked: false,
        ),
      );

      isSavingHealthRecord.value = false;

      Get.snackbar(
        'Record Saved Locally 💾',
        'EMR Record "$visitRef" ($hiType) saved for ${patient.name}. You can link it to ABDM anytime.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFF475569),
        colorText: Colors.white,
        duration: const Duration(seconds: 4),
      );
    } catch (e) {
      isSavingHealthRecord.value = false;
      Get.snackbar('Error', 'Failed to save record: $e',
          backgroundColor: Colors.redAccent, colorText: Colors.white);
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

    final fhirPayload = {
      'careContextReference': visitRef,
      'authoredOn': DateTime.now().toUtc().toIso8601String(),
      'recordType': hiType,
      'encounterType': encounterType.value,
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
      'bodyMeasurements': {
        'heightCm': opHeightCtrl.text,
        'weightKg': opWeightCtrl.text,
        'bmi': opBmiCtrl.text,
      },
      'vitals': vitalsList.map((v) => v.toJson()).toList(),
      'complaints': complaintsList.where((c) => c.trim().isNotEmpty).toList(),
      'clinicalObservation': opObservationResultCtrl.text.trim(),
      'allergies': allergiesList.map((a) => a.toJson()).toList(),
      'medicalHistory':
          medicalHistoryList.where((m) => m.trim().isNotEmpty).toList(),
      'diagnosis': diagnosisCtrl.text.trim(),
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

    savedLocalRecords.insert(
      0,
      SavedRecordModel(
        visitRef: visitRef,
        patientName: patient.name,
        abhaAddress: patient.abhaAddress,
        hiType: hiType,
        createdTime:
            "${DateTime.now().day}/${DateTime.now().month}/${DateTime.now().year} ${DateTime.now().hour}:${DateTime.now().minute.toString().padLeft(2, '0')}",
        fhirPayload: fhirPayload,
        isLinked: success,
      ),
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
        'Linking Request Sent',
        'Care context link request for "$visitRef" queued on ABDM Gateway.',
        snackPosition: SnackPosition.TOP,
        backgroundColor: const Color(0xFFF59E0B),
        colorText: Colors.white,
      );
    }
  }

  /// Links an existing saved local record to ABDM Gateway
  Future<void> linkSingleRecordToAbdm(SavedRecordModel record) async {
    isSavingHealthRecord.value = true;
    try {
      bool success = await HipCareContextRepo.linkCareContext(
        abhaAddress: record.abhaAddress,
        visitRef: record.visitRef,
        display: "${record.hiType} Record - ${record.createdTime}",
        hiType: record.hiType,
        requesterId: 'IN0610090658',
      );
      isSavingHealthRecord.value = false;

      if (success) {
        record.isLinked = true;
        savedLocalRecords.refresh();
        Get.snackbar(
          'Care Context Linked 🎉',
          'Record "${record.visitRef}" (${record.hiType}) for ${record.patientName} linked to ABDM Gateway.',
          snackPosition: SnackPosition.TOP,
          backgroundColor: const Color(0xFF10B981),
          colorText: Colors.white,
          duration: const Duration(seconds: 4),
        );
      } else {
        Get.snackbar(
          'Linking Failed',
          'Could not link record "${record.visitRef}" to ABDM Gateway. Check backend.',
          snackPosition: SnackPosition.TOP,
          backgroundColor: const Color(0xFFEF4444),
          colorText: Colors.white,
        );
      }
    } catch (e) {
      isSavingHealthRecord.value = false;
      Get.snackbar('Error', 'Failed to link: $e',
          backgroundColor: Colors.redAccent, colorText: Colors.white);
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
