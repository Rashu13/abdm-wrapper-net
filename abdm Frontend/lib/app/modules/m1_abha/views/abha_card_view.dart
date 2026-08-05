import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/routes/app_paths.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class AbhaCardView extends GetView<AbhaCreationController> {
  const AbhaCardView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF0F4F8),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 24),
          child: Obx(() {
            final profile = controller.abhaProfile.value;
            final String name = _val(profile?.name, 'ABHA Holder');
            final String abhaNum = _val(profile?.abhaNumber, 'N/A');
            final String abhaAddr = _val(profile?.abhaAddress, 'N/A');
            final String dob = _val(profile?.dob, 'N/A');
            final String gender = _val(profile?.gender, 'N/A');
            final String mobile = _val(profile?.mobile, 'N/A');
            final String address = _val(profile?.address, 'N/A');
            final String pincode = _val(profile?.pincode, 'N/A');
            final String district = _val(profile?.districtName, 'N/A');
            final String state = _val(profile?.stateName, 'N/A');
            final String email = _val(profile?.email, 'N/A');

            return ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 480),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // Header label
                  Container(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 10),
                    decoration: BoxDecoration(
                      color: const Color(0xFFE0ECFA),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Text(
                      'Your ABHA ID CARD',
                      style: fontBold.copyWith(
                        fontSize: 14,
                        color: const Color(0xFF1E3A5F),
                      ),
                    ),
                  ),
                  const SizedBox(height: 14),

                  // Main ABHA Card (dark blue - like image)
                  Container(
                    decoration: BoxDecoration(
                      color: const Color(0xFF1E3A5F),
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: [
                        BoxShadow(
                          color: const Color(0xFF1E3A5F).withOpacity(0.25),
                          blurRadius: 18,
                          offset: const Offset(0, 8),
                        ),
                      ],
                    ),
                    padding: const EdgeInsets.all(18),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        // Profile Photo
                        _buildPhoto(profile?.profilePhoto),
                        const SizedBox(width: 16),

                        // Name, DOB, ABHA Address
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                name,
                                style: fontBold.copyWith(
                                  color: Colors.white,
                                  fontSize: 17,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                '$dob | $abhaAddr',
                                style: fontRegular.copyWith(
                                  color: Colors.white70,
                                  fontSize: 11.5,
                                ),
                                overflow: TextOverflow.ellipsis,
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 12),

                        // ABHA Number (right side)
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 8),
                          decoration: BoxDecoration(
                            color: Colors.white.withOpacity(0.08),
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(color: Colors.white24),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  const Icon(Icons.credit_card,
                                      color: Colors.amber, size: 13),
                                  const SizedBox(width: 4),
                                  Text(
                                    'ABHA Number :',
                                    style: fontSmall.copyWith(
                                        color: Colors.white54, fontSize: 10),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 3),
                              Text(
                                abhaNum,
                                style: fontBold.copyWith(
                                    color: Colors.white, fontSize: 12),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 14),

                  // Additional details card
                  Container(
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(14),
                      border: Border.all(color: const Color(0xFFE2E8F0)),
                    ),
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 14),
                    child: Column(
                      children: [
                        _infoRow('Gender', gender),
                        _divider(),
                        _infoRow('Mobile', mobile),
                        _divider(),
                        _infoRow('Address', address),
                        _divider(),
                        _infoRow('District', district),
                        _divider(),
                        _infoRow('State', state),
                        _divider(),
                        _infoRow('Pincode', pincode),
                        if (email != 'N/A') ...[
                          _divider(),
                          _infoRow('Email', email),
                        ],
                      ],
                    ),
                  ),
                  const SizedBox(height: 16),

                  // Download ABHA Card Button (amber/yellow - like image)
                  Obx(() => SizedBox(
                        width: double.infinity,
                        height: 50,
                        child: ElevatedButton.icon(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFFF5A623),
                            foregroundColor: Colors.white,
                            elevation: 2,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                          onPressed: controller.isDownloadingCard.value
                              ? null
                              : () => controller.downloadAbhaCard(),
                          icon: controller.isDownloadingCard.value
                              ? const SizedBox(
                                  width: 18,
                                  height: 18,
                                  child: CircularProgressIndicator(
                                    color: Colors.white,
                                    strokeWidth: 2,
                                  ),
                                )
                              : const Icon(Icons.file_download_outlined,
                                  size: 20),
                          label: Text(
                            controller.isDownloadingCard.value
                                ? 'Downloading...'
                                : 'Download ABHA Card',
                            style: fontBold.copyWith(
                              color: Colors.white,
                              fontSize: 15,
                            ),
                          ),
                        ),
                      )),
                  const SizedBox(height: 12),

                  // Register Patient in HIP Database Button
                  Obx(() => SizedBox(
                        width: double.infinity,
                        height: 48,
                        child: ElevatedButton.icon(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF10B981),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                          onPressed: controller.isRegisteringDb.value
                              ? null
                              : () => controller.registerPatientInDatabase(),
                          icon: controller.isRegisteringDb.value
                              ? const SizedBox(
                                  width: 18,
                                  height: 18,
                                  child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                                )
                              : const Icon(Icons.how_to_reg, color: Colors.white, size: 20),
                          label: Text(
                            controller.isRegisteringDb.value
                                ? 'Registering in DB...'
                                : 'REGISTER PATIENT IN DATABASE',
                            style: fontBold.copyWith(
                              color: Colors.white,
                              fontSize: 14,
                            ),
                          ),
                        ),
                      )),
                  const SizedBox(height: 10),

                  // Verify Another button
                  TextButton(
                    onPressed: () {
                      controller.resetLoginState();
                      Get.offNamed(Routes.M1_SEARCH_ABHA);
                    },
                    child: Text(
                      'Verify Another',
                      style: fontMedium.copyWith(
                        color: const Color(0xFF1E3A5F),
                        fontSize: 14,
                      ),
                    ),
                  ),
                ],
              ),
            );
          }),
        ),
      ),
    );
  }

  String _val(String? v, String fallback) =>
      (v != null && v.isNotEmpty) ? v : fallback;

  Widget _buildPhoto(String? photoB64) {
    if (photoB64 != null && photoB64.trim().isNotEmpty) {
      try {
        String b64 = photoB64.trim();
        if (b64.contains(',')) b64 = b64.split(',').last;
        final bytes = base64Decode(b64);
        return ClipRRect(
          borderRadius: BorderRadius.circular(50),
          child: Image.memory(
            bytes,
            width: 62,
            height: 62,
            fit: BoxFit.cover,
            errorBuilder: (_, __, ___) => _defaultAvatar(),
          ),
        );
      } catch (_) {}
    }
    return _defaultAvatar();
  }

  Widget _defaultAvatar() => Container(
        width: 62,
        height: 62,
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.15),
          shape: BoxShape.circle,
          border: Border.all(color: Colors.white38, width: 1.5),
        ),
        child: const Icon(Icons.person, size: 36, color: Colors.white70),
      );

  Widget _infoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 90,
            child: Text(
              label,
              style: fontRegular.copyWith(
                color: const Color(0xFF64748B),
                fontSize: 12.5,
              ),
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              value,
              style: fontBold.copyWith(
                color: const Color(0xFF0F172A),
                fontSize: 12.5,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _divider() => const Divider(height: 1, color: Color(0xFFF1F5F9));
}
