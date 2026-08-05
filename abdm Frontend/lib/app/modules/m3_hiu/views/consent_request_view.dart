import 'package:abdm_frontend/app/routes/app_paths.dart' show Routes;
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/health_record_controller.dart';

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
                      gradient: AppColor.primaryGradient,
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
                          "Track GRANTED consent requests and fetch decrypted FHIR medical record bundles.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Consents List Section
            Expanded(
              child: Obx(() {
                if (controller.isLoadingConsents.value) {
                  return const Center(
                      child: CircularProgressIndicator(color: AppColor.accent));
                }
                if (controller.consents.isEmpty) {
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
                          Text("No Consent Artefacts Active", style: fontBold),
                          const SizedBox(height: 6),
                          Text(
                              "Initiate a consent request via /v3/consent-init to request patient records.",
                              style: fontSmall),
                        ],
                      ),
                    ),
                  );
                }

                return ListView.builder(
                  itemCount: controller.consents.length,
                  itemBuilder: (context, index) {
                    final consent = controller.consents[index];
                    bool isGranted = consent.status == 'GRANTED';

                    return Container(
                      margin: const EdgeInsets.only(bottom: 16),
                      padding: const EdgeInsets.all(20),
                      decoration: glassDecoration(),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Text(consent.requesterName,
                                  style: fontBold.copyWith(fontSize: 16)),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 5),
                                decoration: BoxDecoration(
                                  color: (isGranted
                                          ? AppColor.success
                                          : AppColor.warning)
                                      .withOpacity(0.15),
                                  borderRadius: BorderRadius.circular(20),
                                  border: Border.all(
                                      color: (isGranted
                                              ? AppColor.success
                                              : AppColor.warning)
                                          .withOpacity(0.3)),
                                ),
                                child: Text(
                                  consent.status,
                                  style: fontSmall.copyWith(
                                    color: isGranted
                                        ? AppColor.success
                                        : AppColor.warning,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          Text('Purpose: ${consent.purpose}',
                              style: fontRegular),
                          Text('Expires: ${consent.expiryDate}',
                              style: fontSmall),
                          const SizedBox(height: 16),
                          if (isGranted)
                            Align(
                              alignment: Alignment.centerRight,
                              child: Container(
                                decoration: BoxDecoration(
                                  gradient: AppColor.primaryGradient,
                                  borderRadius: BorderRadius.circular(10),
                                ),
                                child: ElevatedButton.icon(
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.transparent,
                                    shadowColor: Colors.transparent,
                                  ),
                                  onPressed: () {
                                    controller
                                        .fetchAndDecryptRecords(consent.id);
                                    Get.toNamed(Routes.M3_HEALTH_RECORDS);
                                  },
                                  icon: const Icon(Icons.folder_open,
                                      color: Colors.white, size: 18),
                                  label: Text('View Decrypted Health Records',
                                      style: fontMedium.copyWith(
                                          color: Colors.white,
                                          fontWeight: FontWeight.bold)),
                                ),
                              ),
                            ),
                        ],
                      ),
                    );
                  },
                );
              }),
            ),
          ],
        ),
      ),
    );
  }
}
