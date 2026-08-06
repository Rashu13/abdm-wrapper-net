import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/routes/app_paths.dart' show Routes;
import '../controllers/health_record_controller.dart';
import '../../../data/repository/m3/hiu_health_record_repo.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class ConsentRequestView extends GetView<HealthRecordController> {
  const ConsentRequestView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header Banner
            Container(
              padding: const EdgeInsets.all(20),
              decoration: glassDecoration(),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [const Color(0xFF8B5CF6), const Color(0xFF6366F1)],
                      ),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(Icons.folder_shared_outlined,
                        color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Milestone M3: HIU Consents & Encrypted FHIR Data",
                            style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Initiate patient consent requests, track status (Requested / Granted / Expired), and view FHIR records.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton.icon(
                    onPressed: () => _showInitiateConsentDialog(context),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF8B5CF6),
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                    ),
                    icon: const Icon(Icons.add_circle_outline_rounded, size: 18),
                    label: const Text("New Consent Request",
                        style: TextStyle(fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // Toolbar with Refresh
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Obx(() => Text(
                      'Consent Requests (${controller.consentRequests.length})',
                      style: fontBold.copyWith(fontSize: 16),
                    )),
                Obx(() => controller.isLoadingConsents.value
                    ? const SizedBox(
                        width: 24,
                        height: 24,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Color(0xFF8B5CF6)))
                    : IconButton(
                        icon: const Icon(Icons.refresh_rounded),
                        color: AppColor.textSecondary,
                        onPressed: controller.fetchConsentRequests,
                      )),
              ],
            ),
            const SizedBox(height: 12),

            // Consents List Section
            Expanded(
              child: Obx(() {
                if (controller.isLoadingConsents.value && controller.consentRequests.isEmpty) {
                  return const Center(
                      child: CircularProgressIndicator(color: Color(0xFF8B5CF6)));
                }

                if (controller.consentRequests.isEmpty) {
                  return Center(
                    child: Container(
                      padding: const EdgeInsets.all(30),
                      decoration: glassDecoration(),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(Icons.privacy_tip_outlined,
                              size: 60, color: AppColor.textSecondary),
                          const SizedBox(height: 16),
                          Text("No Consent Requests Found", style: fontBold),
                          const SizedBox(height: 6),
                          Text(
                            "Click 'New Consent Request' above to request health records from patient.",
                            style: fontSmall,
                            textAlign: TextAlign.center,
                          ),
                        ],
                      ),
                    ),
                  );
                }

                return ListView.builder(
                  itemCount: controller.consentRequests.length,
                  itemBuilder: (context, index) {
                    final item = controller.consentRequests[index];
                    return _buildConsentCard(item);
                  },
                );
              }),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildConsentCard(HiuConsentRequestModel item) {
    bool isGranted = item.status == 'GRANTED' || item.status.contains('GRANT');
    bool isRequested = item.status.contains('REQUEST') || item.status.contains('INIT');
    bool isExpired = item.status.contains('EXPIR');

    final statusColor = isGranted
        ? const Color(0xFF10B981)
        : isRequested
            ? const Color(0xFFF59E0B)
            : isExpired
                ? const Color(0xFF6B7280)
                : const Color(0xFFEF4444);

    return Container(
      margin: const EdgeInsets.only(bottom: 14),
      padding: const EdgeInsets.all(18),
      decoration: glassDecoration(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withOpacity(0.12),
                      borderRadius: BorderRadius.circular(8),
                      border: Border.all(color: statusColor.withOpacity(0.3)),
                    ),
                    child: Text(
                      item.status,
                      style: TextStyle(
                        color: statusColor,
                        fontWeight: FontWeight.bold,
                        fontSize: 12,
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Text(
                    'Purpose: ${item.purpose}',
                    style: TextStyle(
                      color: AppColor.textSecondary,
                      fontSize: 13,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
              Text(
                item.createdAt.split('T').first,
                style: TextStyle(color: AppColor.textSecondary, fontSize: 12),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              const Icon(Icons.fingerprint_rounded, size: 16, color: AppColor.textSecondary),
              const SizedBox(width: 6),
              Text(
                'ABHA Address: ',
                style: TextStyle(color: AppColor.textSecondary, fontSize: 13),
              ),
              Text(
                item.abhaAddress,
                style: TextStyle(
                    color: AppColor.textPrimary, fontWeight: FontWeight.w600, fontSize: 13),
              ),
            ],
          ),
          const SizedBox(height: 10),
          // HI Types chips
          Wrap(
            spacing: 6,
            runSpacing: 6,
            children: item.hiTypes.map((hi) {
              return Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(6),
                  border: Border.all(color: AppColor.border),
                ),
                child: Text(
                  hi,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 11),
                ),
              );
            }).toList(),
          ),
          const SizedBox(height: 14),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              if (item.fromDate.isNotEmpty)
                Text(
                  'Period: ${item.fromDate.split("T").first} → ${item.toDate.split("T").first}',
                  style: TextStyle(color: AppColor.textSecondary, fontSize: 12),
                )
              else
                const SizedBox.shrink(),
              ElevatedButton.icon(
                onPressed: () {
                  controller.fetchAndDecryptRecords(item);
                  Get.toNamed(Routes.M3_HEALTH_RECORDS);
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: isGranted ? const Color(0xFF10B981) : AppColor.surface,
                  foregroundColor: isGranted ? Colors.white : AppColor.textPrimary,
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                  side: isGranted ? BorderSide.none : BorderSide(color: AppColor.border),
                ),
                icon: Icon(Icons.medical_information_rounded,
                    size: 16, color: isGranted ? Colors.white : AppColor.textSecondary),
                label: Text(
                  isGranted ? 'View FHIR Records' : 'View Records Status',
                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  void _showInitiateConsentDialog(BuildContext context) {
    final abhaCtrl = TextEditingController(text: controller.abhaAddressInput.value);
    final now = DateTime.now();
    final oneMonthAgo = now.subtract(const Duration(days: 30));
    final oneYearLater = now.add(const Duration(days: 365));

    final fromDateStr = oneMonthAgo.toIso8601String();
    final toDateStr = now.toIso8601String();
    final eraseAtStr = oneYearLater.toIso8601String();

    Get.dialog(
      Dialog(
        backgroundColor: AppColor.surface,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Container(
          width: 520,
          padding: const EdgeInsets.all(24),
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: const Color(0xFF8B5CF6).withOpacity(0.15),
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: const Icon(Icons.add_circle_outline_rounded,
                          color: Color(0xFF8B5CF6), size: 20),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('Initiate Consent Request (M3 HIU)',
                              style: TextStyle(
                                  color: AppColor.textPrimary,
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold)),
                          Text('Request health records consent from patient via ABDM Gateway',
                              style: TextStyle(color: AppColor.textSecondary, fontSize: 12)),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),

                // Select from Registered Patients dropdown helper
                if (controller.patients.isNotEmpty) ...[
                  Text('Select from Registered Patients',
                      style: TextStyle(
                          color: AppColor.textPrimary, fontSize: 13, fontWeight: FontWeight.w600)),
                  const SizedBox(height: 6),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: DropdownButtonHideUnderline(
                      child: DropdownButton<String>(
                        hint: const Text('Choose a patient...'),
                        isExpanded: true,
                        dropdownColor: AppColor.surface,
                        style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                        items: controller.patients.map((p) {
                          final detailsLabel = "${p.name} (${p.abhaAddress})";
                          return DropdownMenuItem<String>(
                            value: p.abhaAddress,
                            child: Text(detailsLabel),
                          );
                        }).toList(),
                        onChanged: (val) {
                          if (val != null) {
                            abhaCtrl.text = val;
                          }
                        },
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),
                ],

                // ABHA Address Input
                Text('Patient ABHA Address',
                    style: TextStyle(
                        color: AppColor.textPrimary, fontSize: 13, fontWeight: FontWeight.w600)),
                const SizedBox(height: 6),
                TextField(
                  controller: abhaCtrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    hintText: 'e.g. user_40893@sbx',
                    hintStyle: TextStyle(color: AppColor.textSecondary),
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: BorderSide(color: AppColor.border),
                    ),
                    contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                  ),
                ),
                const SizedBox(height: 16),

                // Purpose Code Selection
                Text('Purpose of Request',
                    style: TextStyle(
                        color: AppColor.textPrimary, fontSize: 13, fontWeight: FontWeight.w600)),
                const SizedBox(height: 6),
                Obx(() => Container(
                      padding: const EdgeInsets.symmetric(horizontal: 14),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColor.border),
                      ),
                      child: DropdownButtonHideUnderline(
                        child: DropdownButton<String>(
                          value: controller.selectedPurposeCode.value,
                          isExpanded: true,
                          dropdownColor: AppColor.surface,
                          style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                          items: controller.purposeCodeList.map((p) {
                            return DropdownMenuItem<String>(
                              value: p['code'],
                              child: Text('${p['label']} (${p['code']})'),
                            );
                          }).toList(),
                          onChanged: (val) {
                            if (val != null) controller.selectedPurposeCode.value = val;
                          },
                        ),
                      ),
                    )),
                const SizedBox(height: 16),

                // HI Types Multi-Select Checkboxes
                Text('Health Information (HI) Types Requested',
                    style: TextStyle(
                        color: AppColor.textPrimary, fontSize: 13, fontWeight: FontWeight.w600)),
                const SizedBox(height: 8),
                Obx(() => Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: controller.availableHiTypes.map((hi) {
                        bool isSelected = controller.selectedHiTypes.contains(hi);
                        return FilterChip(
                          label: Text(hi),
                          selected: isSelected,
                          onSelected: (_) => controller.toggleHiType(hi),
                          selectedColor: const Color(0xFF8B5CF6).withOpacity(0.2),
                          checkmarkColor: const Color(0xFF8B5CF6),
                          labelStyle: TextStyle(
                            color: isSelected ? const Color(0xFF8B5CF6) : AppColor.textPrimary,
                            fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                            fontSize: 12,
                          ),
                          backgroundColor: AppColor.background,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                            side: BorderSide(
                              color: isSelected ? const Color(0xFF8B5CF6) : AppColor.border,
                            ),
                          ),
                        );
                      }).toList(),
                    )),
                const SizedBox(height: 24),

                // Submit Buttons
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    TextButton(
                      onPressed: () => Get.back(),
                      child: Text('Cancel', style: TextStyle(color: AppColor.textSecondary)),
                    ),
                    const SizedBox(width: 12),
                    Obx(() => controller.isSubmittingConsent.value
                        ? const CircularProgressIndicator(color: Color(0xFF8B5CF6))
                        : ElevatedButton.icon(
                            onPressed: () {
                              final abha = abhaCtrl.text.trim();
                              if (abha.isEmpty) {
                                Get.snackbar('Error', 'ABHA Address cannot be empty.');
                                return;
                              }
                              Get.back();
                              controller.submitConsentRequest(
                                abhaAddress: abha,
                                fromDate: fromDateStr,
                                toDate: toDateStr,
                                eraseAt: eraseAtStr,
                              );
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF8B5CF6),
                              foregroundColor: Colors.white,
                              padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 12),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                            ),
                            icon: const Icon(Icons.send_rounded, size: 16),
                            label: const Text('Send Consent Request',
                                style: TextStyle(fontWeight: FontWeight.bold)),
                          )),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
