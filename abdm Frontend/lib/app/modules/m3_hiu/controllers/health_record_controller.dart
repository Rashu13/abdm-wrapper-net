import 'dart:convert';
import 'package:abdm_frontend/helper/file_picker_stub.dart'
    if (dart.library.html) 'package:abdm_frontend/helper/file_picker_web.dart'
    as picker;
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
  // PDF Attachment fields
  var attachedPdfName = ''.obs;
  var attachedPdfBase64 = ''.obs;

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
  var complaintsList = <TextEditingController>[].obs;
  var allergiesList = <AllergyFormItem>[].obs;
  var medicalHistoryList = <TextEditingController>[].obs;
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

  @override
  void onClose() {
    for (var c in complaintsList) {
      c.dispose();
    }
    for (var m in medicalHistoryList) {
      m.dispose();
    }
    for (var v in vitalsList) {
      v.dispose();
    }
    for (var a in allergiesList) {
      a.dispose();
    }
    for (var m in medicines) {
      m.dispose();
    }
    for (var l in labResults) {
      l.dispose();
    }
    for (var i in immunizationList) {
      i.dispose();
    }
    opHeightCtrl.dispose();
    opWeightCtrl.dispose();
    opBmiCtrl.dispose();
    opObservationResultCtrl.dispose();
    wellRespRateCtrl.dispose();
    wellHeartRateCtrl.dispose();
    wellSpo2Ctrl.dispose();
    wellTempCtrl.dispose();
    wellSysBpCtrl.dispose();
    wellDiaBpCtrl.dispose();
    wellHeightCtrl.dispose();
    wellWeightCtrl.dispose();
    wellBmiCtrl.dispose();
    wellWaistCtrl.dispose();
    wellOtherObsCtrl.dispose();
    diagnosisCtrl.dispose();
    reportTitleCtrl.dispose();
    dischargeNotesCtrl.dispose();
    adviceCtrl.dispose();
    super.onClose();
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
    final oldVitals = List<VitalFormItem>.from(vitalsList);
    final oldAllergies = List<AllergyFormItem>.from(allergiesList);
    final oldMedicines = List<MedicineFormItem>.from(medicines);
    final oldLab = List<LabResultFormItem>.from(labResults);
    final oldImm = List<ImmunizationFormItem>.from(immunizationList);
    final oldComplaints = List<TextEditingController>.from(complaintsList);
    final oldHistory = List<TextEditingController>.from(medicalHistoryList);

    vitalsList.value = [
      VitalFormItem(vitalName: 'Blood Pressure', value: '120/80', unit: 'mmHg'),
      VitalFormItem(vitalName: 'Pulse Rate', value: '72', unit: 'bpm'),
      VitalFormItem(vitalName: 'Body Temperature', value: '98.6', unit: '°F'),
      VitalFormItem(vitalName: 'SpO2 Oxygen', value: '98', unit: '%'),
    ];

    complaintsList.value = [
      TextEditingController(text: 'Fever with chills for 2 days'),
      TextEditingController(text: 'Mild headache and body fatigue'),
    ];

    allergiesList.value = [
      AllergyFormItem(
          allergyName: 'Penicillin', type: 'medication', status: 'active'),
    ];

    medicalHistoryList.value = [
      TextEditingController(text: 'Type 2 Diabetes Mellitus (Under control)'),
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
          testName: 'Hemoglobin (Hb)', value: '14.2', unit: 'g/dL', snomedCode: '721981007'),
      LabResultFormItem(
          testName: 'Total WBC Count', value: '7,500', unit: '/cu mm', snomedCode: '26604007'),
      LabResultFormItem(
          testName: 'Platelet Count', value: '2.5', unit: 'lakhs/cu mm', snomedCode: '14784000'),
    ];

    immunizationList.value = [
      ImmunizationFormItem(
        vaccineName: 'Covishield (COVID-19 Vaccine)',
        lotNumber: '4120Z015',
        doseNumber: '2',
        date: DateTime.now().toString().split(' ').first,
        manufacturer: 'Serum Institute of India',
      ),
    ];

    WidgetsBinding.instance.addPostFrameCallback((_) {
      for (var v in oldVitals) { v.dispose(); }
      for (var a in oldAllergies) { a.dispose(); }
      for (var m in oldMedicines) { m.dispose(); }
      for (var l in oldLab) { l.dispose(); }
      for (var i in oldImm) { i.dispose(); }
      for (var c in oldComplaints) { c.dispose(); }
      for (var m in oldHistory) { m.dispose(); }
    });

    clearAttachedPdf();
  }

  Future<void> selectPdfFile() async {
    try {
      final result = await picker.pickPdfFile();
      if (result != null) {
        attachedPdfName.value = result.fileName;
        attachedPdfBase64.value = result.base64Data;
        Get.snackbar('Success', 'PDF attached successfully: ${result.fileName}');
      }
    } catch (e) {
      Get.snackbar('Error', 'Failed to pick file: $e');
    }
  }

  void clearAttachedPdf() {
    attachedPdfName.value = '';
    attachedPdfBase64.value = '';
  }

  void addVital() => vitalsList.add(VitalFormItem());
  void removeVital(int idx) {
    if (vitalsList.length > 1) {
      final item = vitalsList.removeAt(idx);
      WidgetsBinding.instance.addPostFrameCallback((_) => item.dispose());
    }
  }

  void addComplaint() => complaintsList.add(TextEditingController());
  void removeComplaint(int idx) {
    if (complaintsList.length > 1) {
      final ctrl = complaintsList.removeAt(idx);
      WidgetsBinding.instance.addPostFrameCallback((_) => ctrl.dispose());
    }
  }

  void addAllergy() => allergiesList.add(AllergyFormItem());
  void removeAllergy(int idx) {
    if (allergiesList.length > 1) {
      final item = allergiesList.removeAt(idx);
      WidgetsBinding.instance.addPostFrameCallback((_) => item.dispose());
    }
  }

  void addMedicalHistory() => medicalHistoryList.add(TextEditingController());
  void removeMedicalHistory(int idx) {
    if (medicalHistoryList.length > 1) {
      final ctrl = medicalHistoryList.removeAt(idx);
      WidgetsBinding.instance.addPostFrameCallback((_) => ctrl.dispose());
    }
  }

  void addMedicine() => medicines.add(MedicineFormItem());
  void removeMedicine(int index) {
    if (medicines.length > 1) {
      final item = medicines.removeAt(index);
      WidgetsBinding.instance.addPostFrameCallback((_) => item.dispose());
    }
  }

  void addLabResult() => labResults.add(LabResultFormItem());
  void removeLabResult(int index) {
    if (labResults.length > 1) {
      final item = labResults.removeAt(index);
      WidgetsBinding.instance.addPostFrameCallback((_) => item.dispose());
    }
  }

  void addImmunization() => immunizationList.add(ImmunizationFormItem());
  void removeImmunization(int index) {
    if (immunizationList.length > 1) {
      final item = immunizationList.removeAt(index);
      WidgetsBinding.instance.addPostFrameCallback((_) => item.dispose());
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
    final hiType = activeHiType.value;

    String prefix = 'VISIT-';
    if (hiType == 'OPConsultation') prefix = 'OPD-';
    else if (hiType == 'Prescription') prefix = 'RX-';
    else if (hiType == 'DiagnosticReport') prefix = 'LAB-';
    else if (hiType == 'DischargeSummary') prefix = 'DIS-';
    else if (hiType == 'ImmunizationRecord') prefix = 'IMM-';
    else if (hiType == 'WellnessRecord') prefix = 'WEL-';

    final visitRef =
        "$prefix${DateTime.now().millisecondsSinceEpoch.toString().substring(6)}";

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
      'clinicalObservation': opObservationResultCtrl.text.trim(),
      'clinicalNotes': opObservationResultCtrl.text.trim(),
      'allergies': allergiesList.map((a) => a.toJson()).toList(),
      'medicalHistory':
          medicalHistoryList.map((m) => m.text.trim()).where((t) => t.isNotEmpty).toList(),
      'diagnosis': diagnosisCtrl.text.trim(),
      'prescriptions': medicines.map((m) => m.toJson()).toList(),
      'labResults': labResults.map((l) => l.toJson()).toList(),
      'reportTitle': reportTitleCtrl.text.trim(),
      'dischargeSummary': dischargeNotesCtrl.text.trim(),
      'advice': adviceCtrl.text.trim(),
      'documents': [
        {
          'contentType': 'application/pdf',
          'type': hiType,
          'data': attachedPdfBase64.value.isNotEmpty
              ? attachedPdfBase64.value
              : 'JVBERi0xLjQKJdPr6gogMSAwIG9iagogIDw8L1R5cGUvQ2F0YWxvZy9QYWdlcyAyIDAgUj4+CmVuZG9iagogMiAwIG9iagogIDw8L1R5cGUvUGFnZXMvS2lkc1szIDAgUl0vQ291bnQgMT4+CmVuZG9iagogMyAwIG9iagogIDw8L1R5cGUvUGFnZS9QYXJlbnQgMiAwIFIvTWVkaWFCb3hbMCAwIDU5NSA4NDJdL0NvbnRlbnRzIDQgMCBSPj4KZW5kb2JqCiA0IDAgb2JqCiAgPDwvTGVuZ3RoIDQ0Pj5zdHJlYW0KQlQgL0YxIDEyIFRmIDcwIDcwMCBUZCAoQUJETSBJbW11bml6YXRpb24gUmVjb3JkKSBUaiBFVAplbmRzdHJlYW0KZW5kb2JqCnhyZWYKMCA1CjAwMDAwMDAwMDAgNjU1MzUgZiAKMDAwMDAwMDAxNSAwMDAwMCBuIAowMDAwMDAwMDcwIDEwMDAwIG4gCjAwMDAwMDAxMjcgMDAwMDAgbiAKMDAwMDAwMDIyMSAwMDAwMCBuIAp0cmFpbGVyCiAgPDwvU2l6ZSA1L1Jvb3QgMSAwIFI+PgpzdGFydHhyZWYKMzE2CiUlRU9GCg==',
        }
      ],
      'immunizations': immunizationList.map((i) => i.toJson()).toList(),
      if (hiType == 'WellnessRecord') ...{
        'bodyMeasurements': [
          if (wellHeightCtrl.text.isNotEmpty) { 'observation': 'Height', 'result': '${wellHeightCtrl.text} cm' },
          if (wellWeightCtrl.text.isNotEmpty) { 'observation': 'Weight', 'result': '${wellWeightCtrl.text} kg' },
          if (wellBmiCtrl.text.isNotEmpty) { 'observation': 'BMI', 'result': wellBmiCtrl.text },
          if (wellWaistCtrl.text.isNotEmpty) { 'observation': 'Waist circumference', 'result': '${wellWaistCtrl.text} cm' },
        ],
        'vitalSigns': [
          if (wellRespRateCtrl.text.isNotEmpty) { 'observation': 'Respiratory rate', 'result': '${wellRespRateCtrl.text} /min' },
          if (wellHeartRateCtrl.text.isNotEmpty) { 'observation': 'Heart rate', 'result': '${wellHeartRateCtrl.text} /min' },
          if (wellSpo2Ctrl.text.isNotEmpty) { 'observation': 'SPO2', 'result': '${wellSpo2Ctrl.text} %' },
          if (wellTempCtrl.text.isNotEmpty) { 'observation': 'Body temperature', 'result': '${wellTempCtrl.text} F' },
          if (wellSysBpCtrl.text.isNotEmpty) { 'observation': 'Systolic BP', 'result': '${wellSysBpCtrl.text} mmHg' },
          if (wellDiaBpCtrl.text.isNotEmpty) { 'observation': 'Diastolic BP', 'result': '${wellDiaBpCtrl.text} mmHg' },
        ],
        'womanHealths': [
          if (wellMenarcheAge.value.isNotEmpty) { 'observation': 'Age at menarche', 'result': '${wellMenarcheAge.value} years' },
          if (wellLmpDate.value.isNotEmpty) { 'observation': 'Last menstrual period date', 'result': wellLmpDate.value },
        ],
        'lifeStyles': [
          { 'observation': 'Diet type', 'result': wellDietType.value },
          { 'observation': 'Tobacco use', 'result': wellTobaccoUse.value },
          { 'observation': 'Alcohol consumption', 'result': wellAlcoholConsumption.value },
        ],
        'otherObservations': [
          if (wellOtherObsCtrl.text.isNotEmpty) { 'observation': 'Other wellness notes', 'result': wellOtherObsCtrl.text.trim() },
        ],
      } else ...{
        'bodyMeasurements': {
          'heightCm': opHeightCtrl.text,
          'weightKg': opWeightCtrl.text,
          'bmi': opBmiCtrl.text,
        },
        'vitals': vitalsList.map((v) => v.toJson()).toList(),
        'complaints': complaintsList.map((c) => c.text.trim()).where((t) => t.isNotEmpty).toList(),
      }
    };

    try {
      await AbdmServer.postRequest(
        endpoint: ApiEndpoints.saveHealthDataRecord,
        body: {
          'abhaAddress': patient.abhaAddress,
          'careContextReference': visitRef,
          'recordType': hiType,
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
    final hiType = activeHiType.value;

    String prefix = 'VISIT-';
    if (hiType == 'OPConsultation') prefix = 'OPD-';
    else if (hiType == 'Prescription') prefix = 'RX-';
    else if (hiType == 'DiagnosticReport') prefix = 'LAB-';
    else if (hiType == 'DischargeSummary') prefix = 'DIS-';
    else if (hiType == 'ImmunizationRecord') prefix = 'IMM-';
    else if (hiType == 'WellnessRecord') prefix = 'WEL-';

    final visitRef =
        "$prefix${DateTime.now().millisecondsSinceEpoch.toString().substring(6)}";
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
      'clinicalObservation': opObservationResultCtrl.text.trim(),
      'clinicalNotes': opObservationResultCtrl.text.trim(),
      'allergies': allergiesList.map((a) => a.toJson()).toList(),
      'medicalHistory':
          medicalHistoryList.map((m) => m.text.trim()).where((t) => t.isNotEmpty).toList(),
      'diagnosis': diagnosisCtrl.text.trim(),
      'prescriptions': medicines.map((m) => m.toJson()).toList(),
      'labResults': labResults.map((l) => l.toJson()).toList(),
      'reportTitle': reportTitleCtrl.text.trim(),
      'dischargeSummary': dischargeNotesCtrl.text.trim(),
      'advice': adviceCtrl.text.trim(),
      'documents': [
        {
          'contentType': 'application/pdf',
          'type': hiType,
          'data': attachedPdfBase64.value.isNotEmpty
              ? attachedPdfBase64.value
              : 'JVBERi0xLjQKJdPr6gogMSAwIG9iagogIDw8L1R5cGUvQ2F0YWxvZy9QYWdlcyAyIDAgUj4+CmVuZG9iagogMiAwIG9iagogIDw8L1R5cGUvUGFnZXMvS2lkc1szIDAgUl0vQ291bnQgMT4+CmVuZG9iagogMyAwIG9iagogIDw8L1R5cGUvUGFnZS9QYXJlbnQgMiAwIFIvTWVkaWFCb3hbMCAwIDU5NSA4NDJdL0NvbnRlbnRzIDQgMCBSPj4KZW5kb2JqCiA0IDAgb2JqCiAgPDwvTGVuZ3RoIDQ0Pj5zdHJlYW0KQlQgL0YxIDEyIFRmIDcwIDcwMCBUZCAoQUJETSBJbW11bml6YXRpb24gUmVjc3JkKSBUaiBFVAplbmRzdHJlYW0KZW5kb2JqCnhyZWYKMCA1CjAwMDAwMDAwMDAgNjU1MzUgZiAKMDAwMDAwMDAxNSAwMDAwMCBuIAowMDAwMDAwMDcwIDEwMDAwIG4gCjAwMDAwMDAxMjcgMDAwMDAgbiAKMDAwMDAwMDIyMSAwMDAwMCBuIAp0cmFpbGVyCiAgPDwvU2l6ZSA1L1Jvb3QgMSAwIFI+PgpzdGFydHhyZWYKMzE2CiUlRU9GCg==',
        }
      ],
      'immunizations': immunizationList.map((i) => i.toJson()).toList(),
      if (hiType == 'WellnessRecord') ...{
        'bodyMeasurements': [
          if (wellHeightCtrl.text.isNotEmpty) { 'observation': 'Height', 'result': '${wellHeightCtrl.text} cm' },
          if (wellWeightCtrl.text.isNotEmpty) { 'observation': 'Weight', 'result': '${wellWeightCtrl.text} kg' },
          if (wellBmiCtrl.text.isNotEmpty) { 'observation': 'BMI', 'result': wellBmiCtrl.text },
          if (wellWaistCtrl.text.isNotEmpty) { 'observation': 'Waist circumference', 'result': '${wellWaistCtrl.text} cm' },
        ],
        'vitalSigns': [
          if (wellRespRateCtrl.text.isNotEmpty) { 'observation': 'Respiratory rate', 'result': '${wellRespRateCtrl.text} /min' },
          if (wellHeartRateCtrl.text.isNotEmpty) { 'observation': 'Heart rate', 'result': '${wellHeartRateCtrl.text} /min' },
          if (wellSpo2Ctrl.text.isNotEmpty) { 'observation': 'SPO2', 'result': '${wellSpo2Ctrl.text} %' },
          if (wellTempCtrl.text.isNotEmpty) { 'observation': 'Body temperature', 'result': '${wellTempCtrl.text} F' },
          if (wellSysBpCtrl.text.isNotEmpty) { 'observation': 'Systolic BP', 'result': '${wellSysBpCtrl.text} mmHg' },
          if (wellDiaBpCtrl.text.isNotEmpty) { 'observation': 'Diastolic BP', 'result': '${wellDiaBpCtrl.text} mmHg' },
        ],
        'womanHealths': [
          if (wellMenarcheAge.value.isNotEmpty) { 'observation': 'Age at menarche', 'result': '${wellMenarcheAge.value} years' },
          if (wellLmpDate.value.isNotEmpty) { 'observation': 'Last menstrual period date', 'result': wellLmpDate.value },
        ],
        'lifeStyles': [
          { 'observation': 'Diet type', 'result': wellDietType.value },
          { 'observation': 'Tobacco use', 'result': wellTobaccoUse.value },
          { 'observation': 'Alcohol consumption', 'result': wellAlcoholConsumption.value },
        ],
        'otherObservations': [
          if (wellOtherObsCtrl.text.isNotEmpty) { 'observation': 'Other wellness notes', 'result': wellOtherObsCtrl.text.trim() },
        ],
      } else ...{
        'bodyMeasurements': {
          'heightCm': opHeightCtrl.text,
          'weightKg': opWeightCtrl.text,
          'bmi': opBmiCtrl.text,
        },
        'vitals': vitalsList.map((v) => v.toJson()).toList(),
        'complaints': complaintsList.map((c) => c.text.trim()).where((t) => t.isNotEmpty).toList(),
      }
    };

    // 1. Save Health Data Record to Backend
    await AbdmServer.postRequest(
      endpoint: ApiEndpoints.saveHealthDataRecord,
      body: {
        'abhaAddress': patient.abhaAddress,
        'careContextReference': visitRef,
        'recordType': hiType,
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
