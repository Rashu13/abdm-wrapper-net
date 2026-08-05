import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:qr_flutter/qr_flutter.dart';
import 'package:abdm_frontend/app/routes/app_paths.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class AbhaCardView extends GetView<AbhaCreationController> {
  const AbhaCardView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        title: const Text('ABHA Card & Digital Profile (M1)'),
        backgroundColor: AppColor.surface,
        elevation: 0,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Obx(() {
          final profile = controller.abhaProfile.value;
          String abhaNum = (profile?.abhaNumber != null && profile!.abhaNumber!.isNotEmpty) ? profile.abhaNumber! : 'N/A';
          String abhaAddr = (profile?.abhaAddress != null && profile!.abhaAddress!.isNotEmpty) ? profile.abhaAddress! : 'N/A';
          String name = (profile?.name != null && profile!.name!.isNotEmpty) ? profile.name! : 'ABHA Holder';
          String gender = (profile?.gender != null && profile!.gender!.isNotEmpty) ? profile.gender! : 'N/A';
          String dob = (profile?.dob != null && profile!.dob!.isNotEmpty) ? profile.dob! : 'N/A';
          String mobile = (profile?.mobile != null && profile!.mobile!.isNotEmpty) ? profile.mobile! : 'N/A';
          String address = (profile?.address != null && profile!.address!.isNotEmpty) ? profile.address! : 'N/A';
          String pincode = (profile?.pincode != null && profile!.pincode!.isNotEmpty) ? profile.pincode! : 'N/A';
          String district = (profile?.districtName != null && profile!.districtName!.isNotEmpty) ? profile.districtName! : 'N/A';
          String state = (profile?.stateName != null && profile!.stateName!.isNotEmpty) ? profile.stateName! : 'N/A';
          String email = (profile?.email != null && profile!.email!.isNotEmpty) ? profile.email! : 'N/A';

          return Column(
            children: [
              // Digital ABHA Card Container (Gradient Theme)
              Container(
                width: double.infinity,
                decoration: BoxDecoration(
                  gradient: AppColor.abhaCardGradient,
                  borderRadius: BorderRadius.circular(20),
                  boxShadow: [
                    BoxShadow(
                      color: AppColor.primary.withOpacity(0.3),
                      blurRadius: 20,
                      spreadRadius: -2,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: Padding(
                  padding: const EdgeInsets.all(24.0),
                  child: Column(
                    children: [
                      // Header Row: Emblem & Status
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Row(
                            children: [
                              Container(
                                padding: const EdgeInsets.all(8),
                                decoration: BoxDecoration(
                                  color: Colors.white.withOpacity(0.15),
                                  shape: BoxShape.circle,
                                ),
                                child: const Icon(Icons.health_and_safety, color: Colors.white, size: 28),
                              ),
                              const SizedBox(width: 10),
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text('ABHA HEALTH CARD', style: fontBold.copyWith(color: Colors.white, fontSize: 16)),
                                  Text('National Health Authority (NHA)', style: fontSmall.copyWith(color: Colors.white70, fontSize: 10)),
                                ],
                              ),
                            ],
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
                            decoration: BoxDecoration(
                              color: AppColor.success,
                              borderRadius: BorderRadius.circular(20),
                            ),
                            child: Row(
                              children: [
                                const Icon(Icons.verified, color: Colors.white, size: 14),
                                const SizedBox(width: 4),
                                Text('VERIFIED', style: fontSmall.copyWith(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 11)),
                              ],
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 20),
                      Divider(color: Colors.white.withOpacity(0.2)),
                      const SizedBox(height: 16),

                      // Card Content: Profile Photo, Details & QR Code
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          _buildProfilePhoto(profile?.profilePhoto),
                          const SizedBox(width: 14),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(name, style: fontBold.copyWith(fontSize: 18, color: Colors.white)),
                                const SizedBox(height: 6),
                                Text('ABHA NUMBER', style: fontSmall.copyWith(color: Colors.white70, fontSize: 10)),
                                Text(abhaNum, style: fontBold.copyWith(color: AppColor.accent, fontSize: 15)),
                                const SizedBox(height: 6),
                                Text('ABHA ADDRESS', style: fontSmall.copyWith(color: Colors.white70, fontSize: 10)),
                                Text(abhaAddr, style: fontMedium.copyWith(color: Colors.white, fontSize: 13)),
                              ],
                            ),
                          ),
                          const SizedBox(width: 10),
                          Container(
                            padding: const EdgeInsets.all(6),
                            decoration: BoxDecoration(
                              color: Colors.white,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: QrImageView(
                              data: 'ABHA_NUM:$abhaNum|ABHA_ADDR:$abhaAddr',
                              version: QrVersions.auto,
                              size: 74.0,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 20),
                      Divider(color: Colors.white.withOpacity(0.2)),
                      const SizedBox(height: 12),

                      // Footer Details
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          _buildCardDetailBadge("GENDER", gender),
                          _buildCardDetailBadge("DATE OF BIRTH", dob),
                          _buildCardDetailBadge("MOBILE", mobile),
                        ],
                      )
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 24),

              // Address & Location Details Container
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(20),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.04),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ],
                  border: Border.all(color: const Color(0xFFE2E8F0)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        const Icon(Icons.location_on_outlined, color: Color(0xFF0F4C81), size: 22),
                        const SizedBox(width: 8),
                        Text(
                          'Address & Contact Information',
                          style: fontBold.copyWith(fontSize: 15, color: const Color(0xFF0F172A)),
                        ),
                      ],
                    ),
                    const Divider(height: 24, color: Color(0xFFE2E8F0)),
                    _buildDetailRow('Full Address', address),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(child: _buildDetailRow('Pincode', pincode)),
                        Expanded(child: _buildDetailRow('District', district)),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(child: _buildDetailRow('State', state)),
                        Expanded(child: _buildDetailRow('Email ID', email)),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 24),

              // Action Buttons
              Container(
                padding: const EdgeInsets.all(20),
                decoration: glassDecoration(),
                child: Column(
                  children: [
                    SizedBox(
                      width: double.infinity,
                      height: 52,
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: AppColor.primaryGradient,
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: ElevatedButton.icon(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.transparent,
                            shadowColor: Colors.transparent,
                          ),
                          onPressed: controller.isDownloadingCard.value
                              ? null
                              : () {
                                  controller.downloadAbhaCard();
                                },
                          icon: controller.isDownloadingCard.value
                              ? const SizedBox(
                                  width: 20,
                                  height: 20,
                                  child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                                )
                              : const Icon(Icons.picture_as_pdf, color: Colors.white),
                          label: Text(
                            controller.isDownloadingCard.value ? 'Downloading ABHA Card...' : 'Download Printable ABHA Card PDF',
                            style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold),
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    SizedBox(
                      width: double.infinity,
                      height: 52,
                      child: OutlinedButton.icon(
                        style: OutlinedButton.styleFrom(
                          side: const BorderSide(color: AppColor.primary),
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        ),
                        onPressed: () {
                          Get.offAllNamed(Routes.DASHBOARD);
                        },
                        icon: const Icon(Icons.dashboard, color: AppColor.accent),
                        label: Text('Back to ABDM Portal Dashboard', style: fontMedium.copyWith(color: AppColor.accent, fontWeight: FontWeight.bold)),
                      ),
                    ),
                  ],
                ),
              )
            ],
          );
        }),
      ),
    );
  }

  Widget _buildProfilePhoto(String? photoB64) {
    if (photoB64 != null && photoB64.trim().isNotEmpty) {
      try {
        String cleanB64 = photoB64.trim();
        if (cleanB64.contains(',')) {
          cleanB64 = cleanB64.split(',').last;
        }
        final bytes = base64Decode(cleanB64);
        return Container(
          width: 76,
          height: 90,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(10),
            border: Border.all(color: Colors.white, width: 2),
            color: Colors.white.withOpacity(0.2),
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: Image.memory(
              bytes,
              fit: BoxFit.cover,
              errorBuilder: (_, __, ___) => const Icon(Icons.person, size: 45, color: Colors.white),
            ),
          ),
        );
      } catch (_) {}
    }
    return Container(
      width: 76,
      height: 90,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: Colors.white.withOpacity(0.5), width: 1.5),
        color: Colors.white.withOpacity(0.15),
      ),
      child: const Icon(Icons.person, size: 45, color: Colors.white70),
    );
  }

  Widget _buildCardDetailBadge(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: fontSmall.copyWith(color: Colors.white60, fontSize: 9)),
        const SizedBox(height: 2),
        Text(value, style: fontMedium.copyWith(color: Colors.white, fontSize: 12, fontWeight: FontWeight.bold)),
      ],
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: fontSmall.copyWith(color: const Color(0xFF64748B), fontSize: 11, fontWeight: FontWeight.w600),
        ),
        const SizedBox(height: 3),
        Text(
          value.isNotEmpty ? value : 'N/A',
          style: fontMedium.copyWith(color: const Color(0xFF0F172A), fontSize: 13.5, fontWeight: FontWeight.bold),
        ),
      ],
    );
  }
}
