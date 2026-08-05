import 'package:abdm_frontend/util/constants.dart';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/modules/m2_hip/controllers/patient_registry_controller.dart';

class PatientRegistryView extends StatelessWidget {
  const PatientRegistryView({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = Get.put(PatientRegistryController());

    return Scaffold(
      backgroundColor: AppColor.background,
      body: Column(
        children: [
          _buildHeader(controller),
          Expanded(child: _buildBody(controller)),
        ],
      ),
    );
  }

  Widget _buildHeader(PatientRegistryController controller) {
    return Container(
      padding: const EdgeInsets.fromLTRB(24, 52, 24, 20),
      decoration: BoxDecoration(
        color: AppColor.surface,
        border:
            Border(bottom: BorderSide(color: AppColor.border.withOpacity(0.5))),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [const Color(0xFF6366F1), const Color(0xFF8B5CF6)],
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.people_alt_rounded,
                    color: Colors.white, size: 22),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Patient Registry',
                        style: TextStyle(
                            color: AppColor.textPrimary,
                            fontSize: 22,
                            fontWeight: FontWeight.w700)),
                    Obx(() => Text(
                          '${controller.patients.length} patients registered',
                          style: TextStyle(
                              color: AppColor.textSecondary, fontSize: 13),
                        )),
                  ],
                ),
              ),
              Obx(() => controller.isLoading.value
                  ? const SizedBox(
                      width: 28,
                      height: 28,
                      child: CircularProgressIndicator(
                          strokeWidth: 2, color: Color(0xFF6366F1)))
                  : IconButton(
                      icon: const Icon(Icons.refresh_rounded),
                      color: AppColor.textSecondary,
                      onPressed: controller.fetchPatients,
                    )),
            ],
          ),
          const SizedBox(height: 16),
          // Search bar
          Container(
            height: 44,
            decoration: BoxDecoration(
              color: AppColor.background,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: AppColor.border),
            ),
            child: TextField(
              onChanged: (v) => controller.searchQuery.value = v,
              style: TextStyle(color: AppColor.textPrimary, fontSize: 14),
              decoration: InputDecoration(
                hintText: 'Search by name, ABHA address or mobile...',
                hintStyle:
                    TextStyle(color: AppColor.textSecondary, fontSize: 13),
                prefixIcon: Icon(Icons.search_rounded,
                    color: AppColor.textSecondary, size: 20),
                border: InputBorder.none,
                contentPadding: const EdgeInsets.symmetric(vertical: 12),
              ),
            ),
          ),
        ],
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
            Icon(Icons.error_outline_rounded,
                color: Colors.redAccent, size: 48),
            const SizedBox(height: 12),
            Text(controller.errorMessage.value,
                style: TextStyle(color: AppColor.textSecondary, fontSize: 14),
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
          child: Column(mainAxisAlignment: MainAxisAlignment.center, children: [
            Icon(Icons.person_search_rounded,
                color: AppColor.textSecondary.withOpacity(0.4), size: 64),
            const SizedBox(height: 16),
            Text(
              controller.searchQuery.value.isEmpty
                  ? 'No patients registered yet.\nAsk patients to scan QR & verify ABHA.'
                  : 'No results for "${controller.searchQuery.value}"',
              style: TextStyle(color: AppColor.textSecondary, fontSize: 14),
              textAlign: TextAlign.center,
            ),
          ]),
        );
      }

      return ListView.separated(
        padding: const EdgeInsets.all(20),
        itemCount: list.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (context, i) => _buildPatientCard(list[i]),
      );
    });
  }

  Widget _buildPatientCard(PatientRegistryModel p) {
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

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColor.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColor.border.withOpacity(0.6)),
        boxShadow: [
          BoxShadow(
              color: Colors.black.withOpacity(0.15),
              blurRadius: 10,
              offset: const Offset(0, 4)),
        ],
      ),
      child: Row(
        children: [
          // Avatar
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: genderColor.withOpacity(0.12),
              shape: BoxShape.circle,
              border:
                  Border.all(color: genderColor.withOpacity(0.3), width: 1.5),
            ),
            child: Icon(genderIcon, color: genderColor, size: 26),
          ),
          const SizedBox(width: 14),
          // Info
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  p.name.isNotEmpty ? p.name : p.patientDisplay,
                  style: TextStyle(
                      color: AppColor.textPrimary,
                      fontWeight: FontWeight.w600,
                      fontSize: 15),
                ),
                const SizedBox(height: 4),
                Row(children: [
                  Icon(Icons.fingerprint_rounded,
                      size: 13, color: AppColor.textSecondary),
                  const SizedBox(width: 4),
                  Expanded(
                    child: Text(
                      p.abhaAddress.isNotEmpty
                          ? p.abhaAddress
                          : 'No ABHA address',
                      style: TextStyle(
                          color: AppColor.textSecondary, fontSize: 12),
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ]),
                if (p.dateOfBirth.isNotEmpty) ...[
                  const SizedBox(height: 2),
                  Row(children: [
                    Icon(Icons.cake_rounded,
                        size: 13, color: AppColor.textSecondary),
                    const SizedBox(width: 4),
                    Text(p.dateOfBirth,
                        style: TextStyle(
                            color: AppColor.textSecondary, fontSize: 12)),
                  ]),
                ],
                if (p.mobile.isNotEmpty) ...[
                  const SizedBox(height: 2),
                  Row(children: [
                    Icon(Icons.phone_rounded,
                        size: 13, color: AppColor.textSecondary),
                    const SizedBox(width: 4),
                    Text(p.mobile,
                        style: TextStyle(
                            color: AppColor.textSecondary, fontSize: 12)),
                  ]),
                ],
              ],
            ),
          ),
          // Care context badge
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: p.careContextCount > 0
                      ? const Color(0xFF10B981).withOpacity(0.12)
                      : AppColor.border.withOpacity(0.2),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  '${p.careContextCount} visit${p.careContextCount == 1 ? '' : 's'}',
                  style: TextStyle(
                    color: p.careContextCount > 0
                        ? const Color(0xFF10B981)
                        : AppColor.textSecondary,
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
