import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/health_record_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class EmrHealthRecordsPage extends GetView<HealthRecordController> {
  const EmrHealthRecordsPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    Get.put(HealthRecordController());

    return Scaffold(
      backgroundColor: AppColor.background,
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Top Header Banner
            Container(
              padding: const EdgeInsets.all(20),
              decoration: glassDecoration(),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [const Color(0xFF10B981), const Color(0xFF059669)],
                      ),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(Icons.medical_services_rounded,
                        color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Sonomed EMR Studio — Health Record Generator & Care Context Creator",
                            style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Select HI Type (Prescription, Diagnostic, OPD Note, Discharge Summary) to build FHIR bundles and link to ABDM Gateway.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // Patient Selector Bar
            Container(
              padding: const EdgeInsets.all(16),
              decoration: glassDecoration(),
              child: Row(
                children: [
                  const Icon(Icons.person_pin_rounded, color: Color(0xFF10B981), size: 24),
                  const SizedBox(width: 12),
                  Text("Target Patient: ", style: fontBold.copyWith(fontSize: 14)),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Obx(() {
                      if (controller.isLoadingPatients.value) {
                        return const Text("Loading registered patients...");
                      }
                      if (controller.patients.isEmpty) {
                        return const Text("No patients registered yet. Register patient in M1/M2 first.",
                            style: TextStyle(color: Colors.redAccent));
                      }
                      return Container(
                        padding: const EdgeInsets.symmetric(horizontal: 14),
                        decoration: BoxDecoration(
                          color: AppColor.background,
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: AppColor.border),
                        ),
                        child: DropdownButtonHideUnderline(
                          child: DropdownButton<String>(
                            value: controller.selectedPatient.value?.abhaAddress,
                            isExpanded: true,
                            dropdownColor: AppColor.surface,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 14),
                            items: controller.patients.map((p) {
                              return DropdownMenuItem<String>(
                                value: p.abhaAddress,
                                child: Text('${p.name} (${p.abhaAddress}) — Mobile: ${p.mobile}'),
                              );
                            }).toList(),
                            onChanged: (val) {
                              if (val != null) {
                                controller.selectedPatient.value =
                                    controller.patients.firstWhere((p) => p.abhaAddress == val);
                              }
                            },
                          ),
                        ),
                      );
                    }),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // HI Types Horizontal Pill Bar
            Obx(() => Container(
                  padding: const EdgeInsets.all(6),
                  decoration: BoxDecoration(
                    color: AppColor.surface,
                    borderRadius: BorderRadius.circular(14),
                    border: Border.all(color: AppColor.border.withOpacity(0.5)),
                  ),
                  child: SingleChildScrollView(
                    scrollDirection: Axis.horizontal,
                    child: Row(
                      children: controller.hiTypeList.map((type) {
                        bool isSelected = controller.activeHiType.value == type;
                        return Padding(
                          padding: const EdgeInsets.only(right: 6),
                          child: ChoiceChip(
                            label: Text(type),
                            selected: isSelected,
                            onSelected: (_) => controller.activeHiType.value = type,
                            selectedColor: const Color(0xFF10B981),
                            labelStyle: TextStyle(
                              color: isSelected ? Colors.white : AppColor.textPrimary,
                              fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                              fontSize: 13,
                            ),
                            backgroundColor: AppColor.background,
                            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10),
                              side: BorderSide(
                                color: isSelected ? const Color(0xFF10B981) : AppColor.border,
                              ),
                            ),
                          ),
                        );
                      }).toList(),
                    ),
                  ),
                )),
            const SizedBox(height: 20),

            // Form Content based on Active HI Type
            Obx(() {
              final type = controller.activeHiType.value;
              switch (type) {
                case 'Prescription':
                  return _buildPrescriptionForm();
                case 'DiagnosticReport':
                  return _buildDiagnosticForm();
                case 'OPConsultation':
                  return _buildOpConsultationForm();
                case 'DischargeSummary':
                  return _buildDischargeSummaryForm();
                case 'ImmunizationRecord':
                  return _buildImmunizationForm();
                default:
                  return _buildPrescriptionForm();
              }
            }),

            const SizedBox(height: 24),

            // Submit Button: Generate FHIR & Link Care Context
            Obx(() => Container(
                  width: double.infinity,
                  height: 52,
                  child: ElevatedButton.icon(
                    onPressed: controller.isSavingHealthRecord.value
                        ? null
                        : controller.generateAndLinkCareContext,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF10B981),
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      elevation: 4,
                    ),
                    icon: controller.isSavingHealthRecord.value
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                          )
                        : const Icon(Icons.cloud_upload_rounded, size: 22),
                    label: Text(
                      controller.isSavingHealthRecord.value
                          ? 'Generating FHIR Bundle & Linking to ABDM Gateway...'
                          : 'Generate FHIR & Link Care Context to ABDM Gateway 🚀',
                      style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                    ),
                  ),
                )),
          ],
        ),
      ),
    );
  }

  // ─── Prescription Form ───────────────────────────────────────────────────
  Widget _buildPrescriptionForm() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Prescription Medicines (Rx)', style: fontBold.copyWith(fontSize: 16)),
              TextButton.icon(
                onPressed: controller.addMedicine,
                icon: const Icon(Icons.add_circle_outline_rounded, size: 18, color: Color(0xFF10B981)),
                label: const Text('Add Medicine',
                    style: TextStyle(color: Color(0xFF10B981), fontWeight: FontWeight.bold)),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Obx(() => ListView.separated(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: controller.medicines.length,
                separatorBuilder: (_, __) => const SizedBox(height: 12),
                itemBuilder: (context, idx) {
                  final med = controller.medicines[idx];
                  return Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: Column(
                      children: [
                        Row(
                          children: [
                            Expanded(
                              flex: 3,
                              child: TextField(
                                controller: TextEditingController(text: med.drugName)
                                  ..selection = TextSelection.collapsed(offset: med.drugName.length),
                                onChanged: (v) => med.drugName = v,
                                style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                                decoration: InputDecoration(
                                  labelText: 'Drug Name (e.g. Dolo 650 mg, Pan 40 mg)',
                                  labelStyle: TextStyle(color: AppColor.textSecondary, fontSize: 12),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                                ),
                              ),
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              flex: 2,
                              child: TextField(
                                controller: TextEditingController(text: med.dosagePattern)
                                  ..selection = TextSelection.collapsed(offset: med.dosagePattern.length),
                                onChanged: (v) => med.dosagePattern = v,
                                style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                                decoration: InputDecoration(
                                  labelText: 'Dosage Pattern (1-0-1)',
                                  labelStyle: TextStyle(color: AppColor.textSecondary, fontSize: 12),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                                ),
                              ),
                            ),
                            const SizedBox(width: 6),
                            IconButton(
                              icon: const Icon(Icons.delete_outline_rounded, color: Colors.redAccent, size: 20),
                              onPressed: () => controller.removeMedicine(idx),
                            ),
                          ],
                        ),
                        const SizedBox(height: 10),
                        Row(
                          children: [
                            Expanded(
                              child: TextField(
                                controller: TextEditingController(text: med.route)
                                  ..selection = TextSelection.collapsed(offset: med.route.length),
                                onChanged: (v) => med.route = v,
                                style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                                decoration: InputDecoration(
                                  labelText: 'Route (Oral / IV / Topical)',
                                  labelStyle: TextStyle(color: AppColor.textSecondary, fontSize: 12),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                                ),
                              ),
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              child: TextField(
                                controller: TextEditingController(text: med.reason)
                                  ..selection = TextSelection.collapsed(offset: med.reason.length),
                                onChanged: (v) => med.reason = v,
                                style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                                decoration: InputDecoration(
                                  labelText: 'Reason / Indication',
                                  labelStyle: TextStyle(color: AppColor.textSecondary, fontSize: 12),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  );
                },
              )),
          const SizedBox(height: 16),
          Text('Advice & Doctor Instructions', style: fontBold.copyWith(fontSize: 14)),
          const SizedBox(height: 6),
          TextField(
            controller: controller.adviceCtrl,
            maxLines: 2,
            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
            decoration: InputDecoration(
              filled: true,
              fillColor: AppColor.background,
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
        ],
      ),
    );
  }

  // ─── Diagnostic Report Form ──────────────────────────────────────────────
  Widget _buildDiagnosticForm() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Report Title & Investigation', style: fontBold.copyWith(fontSize: 16)),
          const SizedBox(height: 6),
          TextField(
            controller: controller.reportTitleCtrl,
            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
            decoration: InputDecoration(
              filled: true,
              fillColor: AppColor.background,
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
          const SizedBox(height: 18),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Lab Test Observations & Results', style: fontBold.copyWith(fontSize: 14)),
              TextButton.icon(
                onPressed: controller.addLabResult,
                icon: const Icon(Icons.add_circle_outline_rounded, size: 18, color: Color(0xFF10B981)),
                label: const Text('Add Test Result',
                    style: TextStyle(color: Color(0xFF10B981), fontWeight: FontWeight.bold)),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Obx(() => ListView.separated(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: controller.labResults.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, idx) {
                  final lab = controller.labResults[idx];
                  return Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          flex: 3,
                          child: TextField(
                            controller: TextEditingController(text: lab.testName)
                              ..selection = TextSelection.collapsed(offset: lab.testName.length),
                            onChanged: (v) => lab.testName = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Test Name (e.g. Hemoglobin)',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          flex: 2,
                          child: TextField(
                            controller: TextEditingController(text: lab.value)
                              ..selection = TextSelection.collapsed(offset: lab.value.length),
                            onChanged: (v) => lab.value = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Observed Value',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          flex: 1,
                          child: TextField(
                            controller: TextEditingController(text: lab.unit)
                              ..selection = TextSelection.collapsed(offset: lab.unit.length),
                            onChanged: (v) => lab.unit = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Unit (g/dL)',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                        IconButton(
                          icon: const Icon(Icons.delete_outline_rounded, color: Colors.redAccent, size: 20),
                          onPressed: () => controller.removeLabResult(idx),
                        ),
                      ],
                    ),
                  );
                },
              )),
        ],
      ),
    );
  }

  // ─── OP Consultation Form ────────────────────────────────────────────────
  Widget _buildOpConsultationForm() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Chief Complaints & Symptoms', style: fontBold.copyWith(fontSize: 14)),
          const SizedBox(height: 6),
          TextField(
            controller: controller.complaintsCtrl,
            maxLines: 2,
            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
            decoration: InputDecoration(
              filled: true,
              fillColor: AppColor.background,
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
          const SizedBox(height: 16),
          Text('Vital Signs', style: fontBold.copyWith(fontSize: 14)),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: controller.bpCtrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    labelText: 'Blood Pressure',
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: TextField(
                  controller: controller.pulseCtrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    labelText: 'Pulse Rate',
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: TextField(
                  controller: controller.tempCtrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    labelText: 'Body Temp',
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: TextField(
                  controller: controller.spo2Ctrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    labelText: 'SpO2 Oxygen',
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Text('Clinical Diagnosis', style: fontBold.copyWith(fontSize: 14)),
          const SizedBox(height: 6),
          TextField(
            controller: controller.diagnosisCtrl,
            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
            decoration: InputDecoration(
              filled: true,
              fillColor: AppColor.background,
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
          const SizedBox(height: 16),
          _buildPrescriptionForm(),
        ],
      ),
    );
  }

  // ─── Discharge Summary Form ──────────────────────────────────────────────
  Widget _buildDischargeSummaryForm() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Discharge Summary & Hospitalization Notes', style: fontBold.copyWith(fontSize: 16)),
          const SizedBox(height: 12),
          TextField(
            controller: controller.dischargeNotesCtrl,
            maxLines: 4,
            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
            decoration: InputDecoration(
              filled: true,
              fillColor: AppColor.background,
              border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              contentPadding: const EdgeInsets.all(12),
            ),
          ),
          const SizedBox(height: 16),
          _buildOpConsultationForm(),
        ],
      ),
    );
  }

  // ─── Immunization Form ───────────────────────────────────────────────────
  Widget _buildImmunizationForm() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Vaccination & Immunization Record', style: fontBold.copyWith(fontSize: 16)),
          const SizedBox(height: 12),
          Obx(() => ListView.separated(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: controller.immunizationList.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, idx) {
                  final item = controller.immunizationList[idx];
                  return Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          flex: 3,
                          child: TextField(
                            controller: TextEditingController(text: item.vaccineName)
                              ..selection = TextSelection.collapsed(offset: item.vaccineName.length),
                            onChanged: (v) => item.vaccineName = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Vaccine Name (Covishield, Hepatitis B)',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          flex: 2,
                          child: TextField(
                            controller: TextEditingController(text: item.lotNumber)
                              ..selection = TextSelection.collapsed(offset: item.lotNumber.length),
                            onChanged: (v) => item.lotNumber = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Lot / Batch No.',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          flex: 1,
                          child: TextField(
                            controller: TextEditingController(text: item.doseNumber)
                              ..selection = TextSelection.collapsed(offset: item.doseNumber.length),
                            onChanged: (v) => item.doseNumber = v,
                            style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                            decoration: InputDecoration(
                              labelText: 'Dose No.',
                              border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                              contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                            ),
                          ),
                        ),
                      ],
                    ),
                  );
                },
              )),
        ],
      ),
    );
  }
}
