import 'package:abdm_frontend/util/constants.dart';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/modules/m2_hip/controllers/patient_registry_controller.dart';
import '../../dashboard/controllers/dashboard_controller.dart';
import '../../m3_hiu/controllers/health_record_controller.dart';
import '../../../../util/style.dart';

class PatientRegistryView extends StatelessWidget {
  const PatientRegistryView({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.put(PatientRegistryController());

    return Scaffold(
      backgroundColor: AppColor.background,
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildHeader(controller),
              const SizedBox(height: 20),
              _buildSearchBar(controller),
              const SizedBox(height: 20),
              Expanded(child: _buildBody(controller)),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeader(PatientRegistryController controller) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: glassDecoration(),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              gradient: const LinearGradient(
                colors: [Color(0xFF6366F1), Color(0xFF8B5CF6)],
              ),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(Icons.people_alt_rounded, color: Colors.white, size: 28),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(
                      "Patient Registry",
                      style: fontBold.copyWith(fontSize: 18),
                    ),
                    const SizedBox(width: 10),
                    Obx(() => Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                          decoration: BoxDecoration(
                            color: const Color(0xFF6366F1).withOpacity(0.15),
                            borderRadius: BorderRadius.circular(12),
                            border: Border.all(color: const Color(0xFF6366F1).withOpacity(0.4)),
                          ),
                          child: Text(
                            "${controller.patients.length} Registered",
                            style: fontSmall.copyWith(
                              color: const Color(0xFF6366F1),
                              fontWeight: FontWeight.bold,
                              fontSize: 10,
                            ),
                          ),
                        )),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  "Manage patient demographics, check linked care contexts, and link visits to ABHA",
                  style: fontSmall,
                ),
              ],
            ),
          ),
          Obx(() => controller.isLoading.value
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Color(0xFF6366F1)),
                )
              : IconButton(
                  icon: const Icon(Icons.sync_rounded, color: Color(0xFF6366F1)),
                  onPressed: controller.fetchPatients,
                )),
        ],
      ),
    );
  }

  Widget _buildSearchBar(PatientRegistryController controller) {
    return Container(
      height: 46,
      decoration: glassDecoration(),
      child: TextField(
        onChanged: (v) => controller.searchQuery.value = v,
        style: fontMedium.copyWith(fontSize: 13),
        decoration: InputDecoration(
          hintText: 'Search by name, ABHA address or mobile...',
          hintStyle: fontSmall.copyWith(fontSize: 12),
          prefixIcon: const Icon(Icons.search_rounded, color: AppColor.textSecondary, size: 18),
          border: InputBorder.none,
          contentPadding: const EdgeInsets.symmetric(vertical: 12),
        ),
      ),
    );
  }

  Widget _buildBody(PatientRegistryController controller) {
    return Obx(() {
      if (controller.isLoading.value && controller.patients.isEmpty) {
        return const Center(
            child: CircularProgressIndicator(color: Color(0xFF6366F1)));
      }

      if (controller.errorMessage.value.isNotEmpty) {
        return Center(
          child: Column(mainAxisAlignment: MainAxisAlignment.center, children: [
            const Icon(Icons.error_outline_rounded,
                color: Colors.redAccent, size: 48),
            const SizedBox(height: 12),
            Text(controller.errorMessage.value,
                style: fontSmall,
                textAlign: TextAlign.center),
            const SizedBox(height: 16),
            TextButton.icon(
              onPressed: controller.fetchPatients,
              icon: const Icon(Icons.refresh_rounded),
              label: const Text('Retry'),
              style: TextButton.styleFrom(
                  foregroundColor: const Color(0xFF6366F1)),
            ),
          ]),
        );
      }

      final list = controller.filtered;
      if (list.isEmpty) {
        return Center(
          child: Column(mainAxisSize: MainAxisSize.min, children: [
            Icon(Icons.person_search_rounded,
                color: AppColor.textSecondary.withOpacity(0.4), size: 64),
            const SizedBox(height: 16),
            Text(
              controller.searchQuery.value.isEmpty
                  ? 'No patients registered yet.\nAsk patients to scan QR & verify ABHA.'
                  : 'No results for "${controller.searchQuery.value}"',
              style: fontSmall,
              textAlign: TextAlign.center,
            ),
          ]),
        );
      }

      return GridView.builder(
        padding: EdgeInsets.zero,
        gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(
          maxCrossAxisExtent: 440,
          mainAxisSpacing: 16,
          crossAxisSpacing: 16,
          mainAxisExtent: 240,
        ),
        itemCount: list.length,
        itemBuilder: (context, i) => _buildPatientCard(list[i]),
      );
    });
  }

  Widget _buildPatientCard(GroupedPatient p) {
    final genderIcon = p.gender.toUpperCase() == 'M'
        ? Icons.male_rounded
        : p.gender.toUpperCase() == 'F'
            ? Icons.female_rounded
            : Icons.person_rounded;
    final genderColor = p.gender.toUpperCase() == 'M'
        ? const Color(0xFF3B82F6)
        : p.gender.toUpperCase() == 'F'
            ? const Color(0xFFEC4899)
            : const Color(0xFF8B5CF6);

    return Obx(() {
      final activeModel = p.selectedModel;

      return Container(
        padding: const EdgeInsets.all(16),
        decoration: glassDecoration(),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Top row: Avatar + Name + Visit Badge
            Row(
              children: [
                Container(
                  width: 42,
                  height: 42,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      colors: [genderColor.withOpacity(0.2), genderColor.withOpacity(0.05)],
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                    ),
                    shape: BoxShape.circle,
                    border: Border.all(color: genderColor.withOpacity(0.4), width: 1.5),
                  ),
                  child: Icon(genderIcon, color: genderColor, size: 20),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        p.name,
                        style: fontBold.copyWith(fontSize: 14, color: AppColor.textPrimary),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 2),
                      Text(
                        activeModel.formattedAbhaNumber,
                        style: fontBold.copyWith(
                          fontSize: 11,
                          color: AppColor.accent,
                          letterSpacing: 1.1,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 6),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: activeModel.careContextCount > 0
                        ? const Color(0xFF10B981).withOpacity(0.12)
                        : AppColor.border.withOpacity(0.2),
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(
                      color: activeModel.careContextCount > 0
                          ? const Color(0xFF10B981).withOpacity(0.3)
                          : AppColor.border.withOpacity(0.4),
                    ),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Container(
                        width: 5,
                        height: 5,
                        decoration: BoxDecoration(
                          color: activeModel.careContextCount > 0 ? const Color(0xFF10B981) : Colors.grey,
                          shape: BoxShape.circle,
                        ),
                      ),
                      const SizedBox(width: 4),
                      Text(
                        '${activeModel.careContextCount} visit${activeModel.careContextCount == 1 ? '' : 's'}',
                        style: fontBold.copyWith(
                          color: activeModel.careContextCount > 0
                              ? const Color(0xFF10B981)
                              : AppColor.textSecondary,
                          fontSize: 10,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            // Dropdown selection for ABHA Address
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: AppColor.background.withOpacity(0.5),
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColor.border.withOpacity(0.8)),
              ),
              child: Row(
                children: [
                  const Icon(Icons.fingerprint_rounded, size: 14, color: AppColor.textSecondary),
                  const SizedBox(width: 6),
                  Expanded(
                    child: p.models.length > 1
                        ? DropdownButtonHideUnderline(
                            child: DropdownButton<String>(
                              value: p.selectedAbhaAddress,
                              isDense: true,
                              isExpanded: true,
                              style: fontMedium.copyWith(fontSize: 11, color: AppColor.primary),
                              items: p.models.map((m) {
                                return DropdownMenuItem<String>(
                                  value: m.abhaAddress,
                                  child: Text(m.abhaAddress, overflow: TextOverflow.ellipsis),
                                );
                              }).toList(),
                              onChanged: (val) {
                                if (val != null) {
                                  p.selectedAbhaAddress = val;
                                }
                              },
                            ),
                          )
                        : Text(
                            p.selectedAbhaAddress,
                            style: fontMedium.copyWith(fontSize: 11, color: AppColor.primary),
                            overflow: TextOverflow.ellipsis,
                          ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),

            // Meta tags row
            Wrap(
              spacing: 6,
              runSpacing: 4,
              children: [
                if (p.dateOfBirth.isNotEmpty)
                  _buildMetaTag(Icons.cake_rounded, p.dateOfBirth),
                if (p.mobile.isNotEmpty)
                  _buildMetaTag(Icons.phone_rounded, p.mobile),
                if (activeModel.pincode.isNotEmpty)
                  _buildMetaTag(Icons.location_on_rounded, "Pin: ${activeModel.pincode}"),
              ],
            ),
            const Spacer(),

            // Actions row: Link Care Context button
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () {
                  final recordCtrl = Get.isRegistered<HealthRecordController>()
                      ? Get.find<HealthRecordController>()
                      : Get.put(HealthRecordController());

                  // Ensure the selected model exists in HealthRecordController's patients list
                  bool exists = recordCtrl.patients.any((pat) => pat.abhaAddress.toLowerCase() == activeModel.abhaAddress.toLowerCase());
                  if (!exists) {
                    recordCtrl.patients.add(activeModel);
                  }

                  // Set selected patient
                  recordCtrl.selectedPatient.value = recordCtrl.patients.firstWhere((pat) => pat.abhaAddress.toLowerCase() == activeModel.abhaAddress.toLowerCase());

                  // Refresh saved records
                  recordCtrl.fetchSavedHealthRecords(activeModel.abhaAddress);

                  // Switch dashboard tab to EMR Health Studio (Index 6)
                  Get.find<DashboardController>().changeTab(6);
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFF10B981),
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 10),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                  elevation: 0,
                ),
                icon: const Icon(Icons.add_link_rounded, size: 14),
                label: Text(
                  'Link Care Context & EMR',
                  style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 11),
                ),
              ),
            ),
          ],
        ),
      );
    });
  }

  Widget _buildMetaTag(IconData icon, String text) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 3),
      decoration: BoxDecoration(
        color: AppColor.surface,
        borderRadius: BorderRadius.circular(6),
        border: Border.all(color: AppColor.border.withOpacity(0.5)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 10, color: AppColor.textSecondary),
          const SizedBox(width: 4),
          Text(text, style: fontSmall.copyWith(fontSize: 9, color: AppColor.textSecondary)),
        ],
      ),
    );
  }
}
