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
          String abhaNum = profile?.abhaNumber ?? '91-8842-1092-3312';
          String abhaAddr = profile?.abhaAddress ?? 'user@abdm';
          String name = profile?.name ?? 'Rahul Sharma';
          String gender = profile?.gender ?? 'M';
          String dob = profile?.dob ?? '15/08/1995';
          String mobile = profile?.mobile ?? '9876543210';

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

                      // Card Content: QR Code & User Identity
                      Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(8),
                            decoration: BoxDecoration(
                              color: Colors.white,
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: QrImageView(
                              data: 'ABHA_NUM:$abhaNum|ABHA_ADDR:$abhaAddr',
                              version: QrVersions.auto,
                              size: 110.0,
                            ),
                          ),
                          const SizedBox(width: 20),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(name, style: fontBold.copyWith(fontSize: 20, color: Colors.white)),
                                const SizedBox(height: 8),
                                Text('ABHA NUMBER', style: fontSmall.copyWith(color: Colors.white70, fontSize: 10)),
                                Text(abhaNum, style: fontBold.copyWith(color: AppColor.accent, fontSize: 16)),
                                const SizedBox(height: 8),
                                Text('ABHA ADDRESS', style: fontSmall.copyWith(color: Colors.white70, fontSize: 10)),
                                Text(abhaAddr, style: fontMedium.copyWith(color: Colors.white, fontSize: 14)),
                              ],
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
              const SizedBox(height: 32),

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
                          onPressed: () {
                            Get.snackbar('Download Started', 'Fetching printable PDF from /api/v3/m1/card...');
                          },
                          icon: const Icon(Icons.picture_as_pdf, color: Colors.white),
                          label: Text('Download Printable ABHA Card PDF', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
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
}
