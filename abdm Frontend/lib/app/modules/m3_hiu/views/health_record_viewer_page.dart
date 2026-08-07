import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:abdm_frontend/app/modules/m3_hiu/controllers/health_record_controller.dart';
import 'package:abdm_frontend/util/constants.dart';
import 'package:abdm_frontend/util/style.dart';
import 'package:abdm_frontend/app/data/api/fhir_parser.dart';
import 'package:abdm_frontend/app/data/repository/m3/hiu_health_record_repo.dart';
import 'package:abdm_frontend/helper/pdf_helper_stub.dart'
    if (dart.library.html) 'package:abdm_frontend/helper/pdf_helper_web.dart'
    as pdf_helper;

class HealthRecordViewerPage extends GetView<HealthRecordController> {
  const HealthRecordViewerPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        title: const Text('ABDM Decrypted Records'),
        backgroundColor: AppColor.surface,
        foregroundColor: AppColor.textPrimary,
        elevation: 0.5,
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20.0, vertical: 16.0),
        child: Obx(() {
          if (controller.isFetchingRecords.value) {
            return Center(
              child: Container(
                padding: const EdgeInsets.all(32),
                decoration: glassDecoration(),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const CircularProgressIndicator(color: AppColor.accent),
                    const SizedBox(height: 20),
                    Text('Decrypting FHIR Data Payload (ECDH Key Exchange)...',
                        style: fontMedium),
                    const SizedBox(height: 6),
                    Text('Connecting to /v3/health-information/fetch-records',
                        style: fontSmall),
                  ],
                ),
              ),
            );
          }

          if (controller.fhirRecords.isEmpty) {
            return Center(
              child: Container(
                padding: const EdgeInsets.all(30),
                decoration: glassDecoration(),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.inventory_2_outlined,
                        size: 60, color: AppColor.textSecondary),
                    const SizedBox(height: 16),
                    Text("No FHIR Records Delivered", style: fontBold),
                    const SizedBox(height: 6),
                    Text(
                        "Once HIP hospital transfers encrypted bundle, records will be rendered here.",
                        style: fontSmall),
                  ],
                ),
              ),
            );
          }

          // Group records by Date and Doctor/Facility name
          final Map<String, List<FhirRecordItem>> groupedRecords = {};
          for (var record in controller.fhirRecords) {
            final doc = record.doctorName.trim().isEmpty
                ? 'Unknown Provider'
                : record.doctorName;
            final key = '${record.date}_$doc';
            if (!groupedRecords.containsKey(key)) {
              groupedRecords[key] = [];
            }
            groupedRecords[key]!.add(record);
          }

          final keys = groupedRecords.keys.toList();
          keys.sort((a, b) {
            final dateAStr = a.split('_').first;
            final dateBStr = b.split('_').first;
            try {
              final dateA = DateTime.parse(dateAStr);
              final dateB = DateTime.parse(dateBStr);
              return dateB.compareTo(dateA); // Descending (latest first)
            } catch (_) {
              return dateBStr.compareTo(dateAStr);
            }
          });

          return ListView.builder(
            itemCount: keys.length,
            itemBuilder: (context, index) {
              final key = keys[index];
              final parts = key.split('_');
              final date = parts.first;
              final doctorName = parts.sublist(1).join('_');
              final records = groupedRecords[key]!;

              return _buildLetterheadPrescription(
                  context, date, doctorName, records);
            },
          );
        }),
      ),
    );
  }

  Widget _buildLetterheadPrescription(BuildContext context, String date,
      String doctorName, List<FhirRecordItem> records) {
    // Look up the active consent request to find the actual Patient ABHA Address
    final activeRequest = controller.consentRequests.firstWhere(
      (req) =>
          req.consentId == controller.selectedConsentId.value ||
          req.clientRequestId == controller.selectedConsentId.value,
      orElse: () => HiuConsentRequestModel(
        id: '',
        clientRequestId: '',
        status: '',
        createdAt: '',
        abhaAddress: '',
        hiTypes: [],
        grantedHiTypes: [],
        purpose: '',
        fromDate: '',
        toDate: '',
        consentId: '',
      ),
    );
    final patientAbha = activeRequest.abhaAddress;

    final mainRecord = records.firstWhere(
      (r) =>
          r.organizationName.isNotEmpty &&
          r.organizationName != 'ABDM Facility',
      orElse: () => records.first,
    );
    final orgName = mainRecord.organizationName.isNotEmpty
        ? mainRecord.organizationName
        : 'ABDM Facility';
    final orgAddress = mainRecord.organizationAddress.isNotEmpty
        ? mainRecord.organizationAddress
        : '';
    final orgPhone = mainRecord.organizationPhone.isNotEmpty
        ? ' | Tel: ${mainRecord.organizationPhone}'
        : '';

    // Categorize records in this encounter
    final List<FhirRecordItem> vitals = [];
    final List<FhirRecordItem> prescriptions = [];
    final List<FhirRecordItem> conditions = [];
    final List<FhirRecordItem> diagnostics = [];
    final List<FhirRecordItem> immunizations = [];
    final List<FhirRecordItem> documents = [];
    final List<FhirRecordItem> procedures = [];
    final List<FhirRecordItem> carePlans = [];
    final List<FhirRecordItem> general = [];

    for (var r in records) {
      final type = r.resourceType.toLowerCase();
      if (type == 'observation' || type == 'composition_investigations') {
        vitals.add(r);
      } else if (type == 'medicationrequest' || type == 'composition_medications') {
        prescriptions.add(r);
      } else if (type == 'condition' || type == 'allergyintolerance' || type == 'familymemberhistory' || type == 'composition_history') {
        conditions.add(r);
      } else if (type == 'diagnosticreport') {
        diagnostics.add(r);
      } else if (type == 'immunization') {
        immunizations.add(r);
      } else if (type == 'documentreference' || type == 'binary' || type == 'composition_documents') {
        documents.add(r);
      } else if (type == 'procedure' || type == 'composition_procedures') {
        procedures.add(r);
      } else if (type == 'careplan' || type == 'composition_careplan') {
        carePlans.add(r);
      } else if (type == 'composition_complaints') {
        general.add(r);
      } else {
        general.add(r);
      }
    }

    return Container(
      margin: const EdgeInsets.only(bottom: 28),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(
            4), // Slight rounded to feel like a paper sheet
        border: Border.all(
            color: const Color(0xFF94A3B8), width: 1.5), // Slate border
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.12),
            blurRadius: 16,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Stack(
        children: [
          // Background Medical Watermark Emblem
          Positioned.fill(
            child: Center(
              child: Opacity(
                opacity: 0.035,
                child: Icon(
                  Icons.healing_outlined,
                  size: 240,
                  color: const Color(0xFF0F4C81),
                ),
              ),
            ),
          ),

          // Prescription content
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // 1. Hospital Top Accent Bar
              Container(
                height: 5,
                color: const Color(0xFF0F4C81),
              ),

              // 2. Letterhead Hospital & Clinic Header
              Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  children: [
                    Text(
                      orgName.toUpperCase(),
                      style: fontBold.copyWith(
                        fontSize: 18,
                        color: const Color(0xFF0F4C81),
                        letterSpacing: 0.75,
                        fontWeight: FontWeight.w800,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      "Integrated Digital Health Facility (ABDM Certified EHR)",
                      style: fontRegular.copyWith(
                        fontSize: 10,
                        color: const Color(0xFF64748B),
                        fontWeight: FontWeight.w500,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    Text(
                      "$orgAddress$orgPhone",
                      style: fontSmall.copyWith(
                        fontSize: 9.5,
                        color: const Color(0xFF64748B),
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 12),

                    // Double border accent under header
                    Container(
                      height: 3,
                      decoration: const BoxDecoration(
                        border: Border(
                          top: BorderSide(color: Color(0xFF0F4C81), width: 1.5),
                          bottom:
                              BorderSide(color: Color(0xFF10B981), width: 0.75),
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              // 3. Form Patient Info Table Layout
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24.0),
                child: Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    border: Border.all(color: const Color(0xFFCBD5E1)),
                    borderRadius: BorderRadius.circular(2),
                  ),
                  child: Column(
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Row(
                              children: [
                                Text("Patient ABHA: ",
                                    style: fontBold.copyWith(
                                        fontSize: 11,
                                        color: const Color(0xFF475569))),
                                Expanded(
                                  child: Text(
                                    patientAbha,
                                    style: fontRegular.copyWith(
                                        fontSize: 11,
                                        color: const Color(0xFF1E293B)),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Row(
                            children: [
                              Text("Date: ",
                                  style: fontBold.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF475569))),
                              Text(date,
                                  style: fontRegular.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF1E293B))),
                            ],
                          ),
                        ],
                      ),
                      const SizedBox(height: 6),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Row(
                            children: [
                              Text("Doctor: ",
                                  style: fontBold.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF475569))),
                              Text(doctorName,
                                  style: fontRegular.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF1E293B))),
                            ],
                          ),
                          Row(
                            children: [
                              Text("Age / Sex: ",
                                  style: fontBold.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF475569))),
                              Text("Adult / Male",
                                  style: fontRegular.copyWith(
                                      fontSize: 11,
                                      color: const Color(0xFF1E293B))),
                            ],
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),

              const SizedBox(height: 16),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24.0),
                child: _buildDashedLine(),
              ),
              const SizedBox(height: 16),

              // 4. Clinical Sections (Dynamic Adaptive Layout)
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // A. Chief Complaints
                    if (general.isNotEmpty) ...[
                      _buildSectionTitle("Chief Complaints"),
                      const SizedBox(height: 8),
                      ...general.map((g) => _buildBulletPoint(g.summary)),
                      const SizedBox(height: 20),
                    ],

                    // B. Medical History
                    if (conditions.isNotEmpty) ...[
                      _buildSectionTitle("Medical History"),
                      const SizedBox(height: 8),
                      ...conditions.map((c) {
                        final cleanTitle = c.title
                            .replaceAll('Condition / Diagnosis: ', '')
                            .replaceAll('Allergy: ', '');
                        return _buildBulletPoint("$cleanTitle (${c.summary})");
                      }),
                      const SizedBox(height: 20),
                    ],

                    // C. Investigations
                    if (vitals.isNotEmpty || diagnostics.isNotEmpty) ...[
                      _buildSectionTitle("Investigations"),
                      const SizedBox(height: 8),
                      if (vitals.isNotEmpty) ...[
                        Text("Vitals / Observations:", style: fontBold.copyWith(fontSize: 12, color: AppColor.textSecondary)),
                        const SizedBox(height: 4),
                        ...vitals.map((v) {
                          final label = v.title.replaceAll('Observation: ', '');
                          final val = v.summary.replaceAll('Result: ', '');
                          return Padding(
                            padding: const EdgeInsets.only(left: 12.0, bottom: 4.0),
                            child: Row(
                              children: [
                                Text("• $label: ", style: fontBold.copyWith(fontSize: 11.5)),
                                Text(val, style: fontRegular.copyWith(fontSize: 11.5)),
                              ],
                            ),
                          );
                        }),
                        const SizedBox(height: 8),
                      ],
                      if (diagnostics.isNotEmpty) ...[
                        Text("Diagnostic Lab Reports:", style: fontBold.copyWith(fontSize: 12, color: AppColor.textSecondary)),
                        const SizedBox(height: 4),
                        ...diagnostics.map((d) {
                          final testName = d.title.replaceAll('Diagnostic Report: ', '');
                          return Padding(
                            padding: const EdgeInsets.only(left: 12.0, bottom: 6.0),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text("• $testName", style: fontBold.copyWith(fontSize: 11.5)),
                                Text(d.summary, style: fontRegular.copyWith(fontSize: 11, color: AppColor.textSecondary)),
                              ],
                            ),
                          );
                        }),
                      ],
                      const SizedBox(height: 20),
                    ],

                    // D. Procedures
                    if (procedures.isNotEmpty) ...[
                      _buildSectionTitle("Procedures"),
                      const SizedBox(height: 8),
                      ...procedures.map((p) {
                        final cleanProc = p.title.replaceAll('Procedure: ', '');
                        return _buildBulletPoint("$cleanProc | ${p.summary}");
                      }),
                      const SizedBox(height: 20),
                    ],

                    // E. Medications
                    if (prescriptions.isNotEmpty) ...[
                      _buildSectionTitle("Medications"),
                      const SizedBox(height: 8),
                      ...prescriptions.map((p) {
                        final cleanMed = p.title.replaceAll('Prescription: ', '');
                        final dosage = p.summary.replaceAll('Dosage: ', '');
                        return _buildBulletPoint("$cleanMed - $dosage");
                      }),
                      const SizedBox(height: 20),
                    ],

                    // F. Care Plan
                    if (carePlans.isNotEmpty) ...[
                      _buildSectionTitle("Care Plan"),
                      const SizedBox(height: 8),
                      ...carePlans.map((c) {
                        final cleanPlan = c.title.replaceAll('Care Plan: ', '');
                        return _buildBulletPoint("$cleanPlan: ${c.summary}");
                      }),
                      const SizedBox(height: 20),
                    ],

                    // G. Document Reference / Attached Records
                    if (documents.isNotEmpty) ...[
                      _buildSectionTitle("Document Reference / Attached Records"),
                      const SizedBox(height: 8),
                      ...documents.map((d) {
                        return Padding(
                          padding: const EdgeInsets.only(bottom: 8.0, left: 4.0),
                          child: Row(
                            children: [
                              const Icon(Icons.picture_as_pdf_outlined,
                                  color: Colors.redAccent, size: 18),
                              const SizedBox(width: 8),
                              Expanded(
                                child: Text(
                                  d.title,
                                  style: fontRegular.copyWith(
                                      fontSize: 11.5,
                                      color: const Color(0xFF1E293B)),
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ),
                              const SizedBox(width: 8),
                              InkWell(
                                onTap: () {
                                  if (d.pdfData != null && d.pdfData!.isNotEmpty) {
                                    pdf_helper.openPdfFromBase64(d.pdfData!, d.title);
                                  } else {
                                    Get.snackbar(
                                      'No Document Data',
                                      'This document does not contain PDF binary data.',
                                      snackPosition: SnackPosition.BOTTOM,
                                      backgroundColor: Colors.redAccent,
                                      colorText: Colors.white,
                                    );
                                  }
                                },
                                child: const Text(
                                  "View PDF",
                                  style: TextStyle(
                                    fontSize: 11,
                                    fontWeight: FontWeight.bold,
                                    color: Color(0xFF0284C7),
                                    decoration: TextDecoration.underline,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        );
                      }),
                    ],
                  ],
                ),
              ),

              const SizedBox(height: 24),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24.0),
                child: _buildDashedLine(),
              ),
              const SizedBox(height: 20),

              // 5. Letterpad Footer (Verification Stamp & Signature Pad)
              // Padding(
              //   padding: const EdgeInsets.symmetric(
              //       horizontal: 24.0, vertical: 16.0),
              //   child: Row(
              //     mainAxisAlignment: MainAxisAlignment.spaceBetween,
              //     crossAxisAlignment: CrossAxisAlignment.end,
              //     children: [
              //       // Verified stamp
              //       Container(
              //         padding: const EdgeInsets.symmetric(
              //             horizontal: 8, vertical: 4),
              //         decoration: BoxDecoration(
              //           color: Colors.transparent,
              //           borderRadius: BorderRadius.circular(4),
              //           border: Border.all(
              //               color: const Color(0xFF34A853), width: 1.5),
              //         ),
              //         // child: Row(
              //         //   mainAxisSize: MainAxisSize.min,
              //         //   children: [
              //         //     const Icon(Icons.verified_user_rounded, color: Color(0xFF34A853), size: 12),
              //         //     const SizedBox(width: 4),
              //         //     Text(
              //         //       "ABDM SECURED EHR",
              //         //       style: fontBold.copyWith(
              //         //         fontSize: 8.5,
              //         //         color: const Color(0xFF137333),
              //         //         letterSpacing: 0.5,
              //         //       ),
              //         //     ),
              //         //   ],
              //         // ),
              //       ),

              //       // Signature line
              //       Column(
              //         crossAxisAlignment: CrossAxisAlignment.center,
              //         children: [
              //           Text(
              //             doctorName,
              //             style: GoogleFonts.caveat(
              //               fontSize: 18,
              //               fontWeight: FontWeight.bold,
              //               color: const Color(0xFF0F4C81),
              //             ),
              //           ),
              //           const SizedBox(height: 1),
              //           Container(
              //             width: 120,
              //             height: 1,
              //             color: const Color(0xFF64748B),
              //           ),
              //           const SizedBox(height: 2),
              //           Text(
              //             "Authorized Signature / Doctor",
              //             style: fontSmall.copyWith(
              //               fontSize: 9,
              //               fontWeight: FontWeight.w600,
              //               color: const Color(0xFF64748B),
              //             ),
              //           ),
              //         ],
              //       ),
              //     ],
              //   ),
              // ),

              // Very bottom notice
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title.toUpperCase(),
          style: fontBold.copyWith(
            fontSize: 10.5,
            color: const Color(0xFF475569),
            letterSpacing: 0.5,
          ),
        ),
        const SizedBox(height: 2),
        Container(
          width: 40,
          height: 1.5,
          color: const Color(0xFF0F4C81),
        ),
      ],
    );
  }

  Widget _buildDashedLine() {
    return Row(
      children: List.generate(
        40,
        (index) => Expanded(
          child: Container(
            color:
                index % 2 == 0 ? Colors.transparent : const Color(0xFFCBD5E1),
            height: 1.5,
          ),
        ),
      ),
    );
  }

  Widget _buildBulletPoint(String text) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6.0, left: 4.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text("• ", style: fontBold.copyWith(fontSize: 13, color: const Color(0xFF0F4C81))),
          Expanded(
            child: Text(
              text,
              style: fontRegular.copyWith(
                  fontSize: 12,
                  color: const Color(0xFF334155)),
            ),
          ),
        ],
      ),
    );
  }
}
