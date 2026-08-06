import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:qr_flutter/qr_flutter.dart';
import '../controllers/scan_share_controller.dart';
import '../controllers/care_context_controller.dart';
import '../controllers/patient_registry_controller.dart';
import '../../dashboard/controllers/dashboard_controller.dart';
import '../../m3_hiu/controllers/health_record_controller.dart';
import '../../../routes/app_paths.dart';
import '../../../../util/style.dart';
import '../../../../util/constants.dart';

class ScanShareView extends GetView<ScanShareController> {
  const ScanShareView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    // Ensure ScanShareController is initialized
    final controller = Get.put(ScanShareController());

    return Scaffold(
      backgroundColor: AppColor.background,
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Header Section (Project Glass Theme)
              _buildPageHeader(),
              const SizedBox(height: 24),

              // Main Hospital OPD Desk QR Card
              _buildHospitalQrCard(context, controller),
              const SizedBox(height: 24),

              // Live OPD Tokens Feed Table
              _buildLiveTokensTable(context, controller),
            ],
          ),
        ),
      ),
    );
  }

  // Header Component matching Project Glass Design Language
  Widget _buildPageHeader() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  gradient: AppColor.primaryGradient,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.qr_code_scanner,
                    color: Colors.white, size: 28),
              ),
              const SizedBox(width: 16),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(
                        "ABDM Scan & Share Engine",
                        style: fontBold.copyWith(fontSize: 18),
                      ),
                      const SizedBox(width: 10),
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 8, vertical: 3),
                        decoration: BoxDecoration(
                          color: AppColor.success.withOpacity(0.15),
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                              color: AppColor.success.withOpacity(0.4)),
                        ),
                        child: Text(
                          "Active Counter Desk",
                          style: fontSmall.copyWith(
                            color: AppColor.success,
                            fontWeight: FontWeight.bold,
                            fontSize: 10,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Text(
                    "Display OPD Counter QR code for patients to scan & generate instant OPD token",
                    style: fontSmall,
                  ),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }

  // Hospital Counter QR Card
  Widget _buildHospitalQrCard(
      BuildContext context, ScanShareController controller) {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  const Icon(Icons.storefront_outlined,
                      color: AppColor.accent, size: 22),
                  const SizedBox(width: 8),
                  Text(
                    "Hospital OPD Counter Desk",
                    style: fontBold.copyWith(fontSize: 16),
                  ),
                ],
              ),
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: AppColor.primary.withOpacity(0.15),
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(color: AppColor.primary.withOpacity(0.3)),
                ),
                child: Text(
                  "Registration Desk QR",
                  style: fontSmall.copyWith(
                    color: AppColor.accent,
                    fontWeight: FontWeight.bold,
                    fontSize: 11,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Divider(color: AppColor.border.withOpacity(0.5), height: 1),
          const SizedBox(height: 16),

          // Configurable Registered HIP ID & Counter
          Row(
            children: [
              Expanded(
                flex: 6,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("HIP ID (Registered ABDM ID):",
                        style: fontMedium.copyWith(fontSize: 11)),
                    const SizedBox(height: 4),
                    TextField(
                      controller: controller.hipIdController,
                      onChanged: (_) => controller.update(),
                      decoration: InputDecoration(
                        hintText: "e.g. IN0610090658",
                        isDense: true,
                        contentPadding: const EdgeInsets.symmetric(
                            horizontal: 10, vertical: 8),
                        fillColor: AppColor.surface,
                        filled: true,
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(8),
                          borderSide: BorderSide(color: AppColor.border),
                        ),
                        enabledBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(8),
                          borderSide: BorderSide(color: AppColor.border),
                        ),
                      ),
                      style: fontMedium.copyWith(fontSize: 12),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                flex: 6,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("Select OPD Counter Desk:",
                        style: fontMedium.copyWith(fontSize: 11)),
                    const SizedBox(height: 4),
                    Obx(() => Container(
                          padding: const EdgeInsets.symmetric(horizontal: 10),
                          decoration: BoxDecoration(
                            color: AppColor.surface,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: AppColor.border),
                          ),
                          child: DropdownButtonHideUnderline(
                            child: DropdownButton<String>(
                              value: controller.selectedCounterId.value,
                              isExpanded: true,
                              isDense: true,
                              items:
                                  controller.counterNames.entries.map((entry) {
                                return DropdownMenuItem<String>(
                                  value: entry.key,
                                  child: Text(entry.value,
                                      style: fontMedium.copyWith(fontSize: 12)),
                                );
                              }).toList(),
                              onChanged: (val) {
                                if (val != null)
                                  controller.selectedCounterId.value = val;
                              },
                            ),
                          ),
                        )),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),

          // QR Format Mode Segment Switcher
          Text("Select ABDM QR Format Mode:",
              style: fontMedium.copyWith(fontSize: 11)),
          const SizedBox(height: 6),
          Obx(() => Row(
                children: [
                  Expanded(
                    child: ChoiceChip(
                      label: Text("ABDM Universal JSON",
                          style: fontSmall.copyWith(fontSize: 11)),
                      selected: controller.qrFormatMode.value == 'json',
                      selectedColor: AppColor.primary.withOpacity(0.2),
                      onSelected: (selected) {
                        if (selected) controller.qrFormatMode.value = 'json';
                      },
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: ChoiceChip(
                      label: Text("Official DeepLink URL",
                          style: fontSmall.copyWith(fontSize: 11)),
                      selected: controller.qrFormatMode.value == 'deeplink',
                      selectedColor: AppColor.success.withOpacity(0.2),
                      onSelected: (selected) {
                        if (selected)
                          controller.qrFormatMode.value = 'deeplink';
                      },
                    ),
                  ),
                ],
              )),
          const SizedBox(height: 24),

          // Display Generated Scannable QR Code
          Center(
            child: Obx(() {
              final payload = controller.getQrPayload();
              return Container(
                padding: const EdgeInsets.all(20),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(
                      color: AppColor.accent.withOpacity(0.3), width: 2),
                  boxShadow: [
                    BoxShadow(
                      color: AppColor.accent.withOpacity(0.1),
                      blurRadius: 16,
                      spreadRadius: 2,
                    ),
                  ],
                ),
                child: Column(
                  children: [
                    QrImageView(
                      data: payload,
                      version: QrVersions.auto,
                      size: 220.0,
                      backgroundColor: Colors.white,
                    ),
                    const SizedBox(height: 12),
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 14, vertical: 6),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        "SCAN WITH ABHA / PHR APP",
                        style: fontBold.copyWith(
                            fontSize: 12,
                            color: AppColor.accent,
                            letterSpacing: 1.1),
                      ),
                    ),
                  ],
                ),
              );
            }),
          ),
          const SizedBox(height: 16),

          // QR Payload Raw Text Display
          Obx(() => Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: AppColor.border),
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        controller.getQrPayload(),
                        style: fontRegular.copyWith(
                            fontSize: 11,
                            color: AppColor.textSecondary,
                            fontFamily: 'monospace'),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ],
                ),
              )),
          const SizedBox(height: 12),
          Center(
            child: Text(
              "Displayed at OPD Registration Desk • Compatible with Official ABHA App, Aarogya Setu & Paytm",
              style: fontSmall,
              textAlign: TextAlign.center,
            ),
          ),
        ],
      ),
    );
  }

  // Live Generated OPD Tokens Queue Table with Quick M2 Care Context Link Button
  Widget _buildLiveTokensTable(
      BuildContext context, ScanShareController controller) {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  const Icon(Icons.confirmation_number_outlined,
                      color: AppColor.accent, size: 22),
                  const SizedBox(width: 8),
                  Text(
                    "Live OPD Registration Token Queue",
                    style: fontBold.copyWith(fontSize: 16),
                  ),
                ],
              ),
              Row(
                children: [
                  Obx(() => OutlinedButton.icon(
                        style: OutlinedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 12, vertical: 6),
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(20)),
                          side: const BorderSide(color: AppColor.success),
                        ),
                        onPressed: controller.isFetchingRemote.value
                            ? null
                            : () => controller.fetchBackendScanRequests(),
                        icon: controller.isFetchingRemote.value
                            ? const SizedBox(
                                width: 12,
                                height: 12,
                                child: CircularProgressIndicator(
                                    strokeWidth: 2, color: AppColor.success),
                              )
                            : const Icon(Icons.sync,
                                size: 14, color: AppColor.success),
                        label: Text(
                          controller.isFetchingRemote.value
                              ? "Syncing..."
                              : "Sync Real Scans",
                          style: fontSmall.copyWith(
                              color: AppColor.success,
                              fontWeight: FontWeight.bold,
                              fontSize: 11),
                        ),
                      )),
                  const SizedBox(width: 8),
                  Obx(() => Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: AppColor.accent.withOpacity(0.15),
                          borderRadius: BorderRadius.circular(20),
                          border: Border.all(
                              color: AppColor.accent.withOpacity(0.3)),
                        ),
                        child: Text(
                          "${controller.tokenQueue.length} Active Tokens Today",
                          style: fontSmall.copyWith(
                            color: AppColor.accent,
                            fontWeight: FontWeight.bold,
                            fontSize: 11,
                          ),
                        ),
                      )),
                ],
              ),
            ],
          ),
          const SizedBox(height: 16),
          Divider(color: AppColor.border.withOpacity(0.5), height: 1),
          const SizedBox(height: 12),
          Obx(() {
            if (controller.tokenQueue.isEmpty) {
              return Padding(
                padding: const EdgeInsets.all(32),
                child: Center(
                  child: Text(
                    "No OPD tokens generated yet. Scan QR & share profile from ABHA App to view tokens.",
                    style: fontSmall,
                  ),
                ),
              );
            }

            return ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.tokenQueue.length,
              separatorBuilder: (context, index) =>
                  Divider(color: AppColor.border.withOpacity(0.5), height: 1),
              itemBuilder: (context, index) {
                final token = controller.tokenQueue[index];
                return Container(
                  padding:
                      const EdgeInsets.symmetric(vertical: 12, horizontal: 8),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      // Token Badge & Patient Info
                      Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(
                                horizontal: 14, vertical: 8),
                            decoration: BoxDecoration(
                              gradient: AppColor.primaryGradient,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Text(
                              "#${token.tokenNumber}",
                              style: fontBold.copyWith(
                                  color: Colors.white, fontSize: 16),
                            ),
                          ),
                          const SizedBox(width: 16),
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Text(
                                    token.patientName,
                                    style: fontBold.copyWith(fontSize: 14),
                                  ),
                                  const SizedBox(width: 8),
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                        horizontal: 6, vertical: 2),
                                    decoration: BoxDecoration(
                                      color: AppColor.surface,
                                      borderRadius: BorderRadius.circular(4),
                                      border:
                                          Border.all(color: AppColor.border),
                                    ),
                                    child: Text(
                                      token.gender,
                                      style: fontSmall.copyWith(
                                          fontSize: 10,
                                          fontWeight: FontWeight.bold),
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 2),
                              Text(
                                "ABHA: ${token.abhaAddress} • Mobile: ${token.phoneNumber}",
                                style: fontSmall,
                              ),
                            ],
                          ),
                        ],
                      ),

                      // Actions & Counter Info
                      Row(
                        children: [
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 10, vertical: 4),
                                decoration: BoxDecoration(
                                  color: AppColor.success.withOpacity(0.15),
                                  borderRadius: BorderRadius.circular(8),
                                  border: Border.all(
                                      color: AppColor.success.withOpacity(0.3)),
                                ),
                                child: Text(
                                  token.counterName,
                                  style: fontSmall.copyWith(
                                    color: AppColor.success,
                                    fontWeight: FontWeight.bold,
                                    fontSize: 11,
                                  ),
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                "Valid for 30m • ${_formatTime(token.generatedAt)}",
                                style: fontSmall.copyWith(fontSize: 10),
                              ),
                            ],
                          ),
                          const SizedBox(width: 16),

                          // Direct M2 Care Context Link Button
                          ElevatedButton.icon(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: AppColor.primary,
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(8)),
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 12, vertical: 8),
                            ),
                            onPressed: () {
                              final recordCtrl =
                                  Get.isRegistered<HealthRecordController>()
                                      ? Get.find<HealthRecordController>()
                                      : Get.put(HealthRecordController());

                              // Map token to PatientRegistryModel
                              final p = PatientRegistryModel(
                                abhaAddress: token.abhaAddress,
                                name: token.patientName,
                                patientReference: "",
                                patientDisplay: token.patientName,
                                gender: token.gender,
                                dateOfBirth: "",
                                mobile: token.phoneNumber,
                                hipId: "",
                                careContextCount: 0,
                                abhaNumber: '',
                                pincode: '',
                              );

                              // Ensure the patient exists in HealthRecordController's patients list
                              bool exists = recordCtrl.patients.any((pat) =>
                                  pat.abhaAddress.toLowerCase() ==
                                  token.abhaAddress.toLowerCase());
                              if (!exists) {
                                recordCtrl.patients.add(p);
                              }

                              // Set selected patient
                              recordCtrl.selectedPatient.value =
                                  recordCtrl.patients.firstWhere((pat) =>
                                      pat.abhaAddress.toLowerCase() ==
                                      token.abhaAddress.toLowerCase());

                              // Refresh saved records
                              recordCtrl
                                  .fetchSavedHealthRecords(token.abhaAddress);

                              // Switch dashboard tab to EMR Health Studio (Index 6)
                              Get.find<DashboardController>().changeTab(6);
                            },
                            icon: const Icon(Icons.add_link,
                                color: Colors.white, size: 14),
                            label: Text(
                              "Link Care Context (M2)",
                              style: fontMedium.copyWith(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 11),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                );
              },
            );
          }),
        ],
      ),
    );
  }

  // Quick Modal Dialog to Link M2 Care Context directly for Scanned Patient
  void _showLinkContextModal(BuildContext context, OpdTokenRecord token) {
    final visitRefController = TextEditingController(
        text: 'OPD-VISIT-${DateTime.now().millisecondsSinceEpoch % 10000}');
    final careContextController = Get.isRegistered<CareContextController>()
        ? Get.find<CareContextController>()
        : Get.put(CareContextController());

    showDialog(
      context: context,
      builder: (context) => Dialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Container(
          width: 480,
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(8),
                        decoration: BoxDecoration(
                          gradient: AppColor.primaryGradient,
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Icon(Icons.link,
                            color: Colors.white, size: 18),
                      ),
                      const SizedBox(width: 10),
                      Text("Link Care Context (Milestone M2)",
                          style: fontBold.copyWith(fontSize: 16)),
                    ],
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, size: 18),
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              const Divider(height: 1),
              const SizedBox(height: 16),

              // Patient Details Summary Box
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColor.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("Token #${token.tokenNumber} • ${token.patientName}",
                        style: fontBold.copyWith(fontSize: 14)),
                    const SizedBox(height: 4),
                    Text("ABHA Address: ${token.abhaAddress}",
                        style: fontSmall.copyWith(
                            color: AppColor.accent,
                            fontWeight: FontWeight.bold)),
                  ],
                ),
              ),
              const SizedBox(height: 16),

              // Visit Reference Input
              Text("Enter OPD Visit / Medical Record Reference:",
                  style: fontMedium.copyWith(fontSize: 12)),
              const SizedBox(height: 6),
              TextField(
                controller: visitRefController,
                decoration: InputDecoration(
                  hintText: "e.g. OPD-VISIT-1002",
                  isDense: true,
                  contentPadding:
                      const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8)),
                ),
                style: fontMedium.copyWith(fontSize: 13),
              ),
              const SizedBox(height: 24),

              // Modal Actions
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  OutlinedButton(
                    onPressed: () => Navigator.of(context).pop(),
                    child: Text("Cancel",
                        style: fontMedium.copyWith(fontSize: 12)),
                  ),
                  const SizedBox(width: 12),
                  ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColor.primary,
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8)),
                      padding: const EdgeInsets.symmetric(
                          horizontal: 16, vertical: 10),
                    ),
                    onPressed: () async {
                      Navigator.of(context).pop();
                      await careContextController.linkNewCareContext(
                          token.abhaAddress, visitRefController.text.trim());
                      Get.toNamed(Routes.M2_DISCOVERY);
                    },
                    icon: const Icon(Icons.check_circle_outline,
                        color: Colors.white, size: 16),
                    label: Text("Link Context & View Records",
                        style: fontBold.copyWith(
                            color: Colors.white, fontSize: 12)),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _formatTime(DateTime dt) {
    return "${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}:${dt.second.toString().padLeft(2, '0')}";
  }
}
