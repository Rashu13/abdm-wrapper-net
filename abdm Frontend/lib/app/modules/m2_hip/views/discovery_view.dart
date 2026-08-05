import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/care_context_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class DiscoveryView extends GetView<CareContextController> {
  const DiscoveryView({Key? key}) : super(key: key);

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
                    child: const Icon(Icons.local_hospital_outlined, color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Milestone M2: HIP Care Contexts & Discovery", style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Discover patient by ABHA ID and link OPD visits, diagnostic reports, and IPD records.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColor.primary,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                    ),
                    onPressed: () {
                      controller.linkNewCareContext('rahul@abdm', 'OPD-VISIT-${DateTime.now().millisecondsSinceEpoch % 10000}');
                    },
                    icon: const Icon(Icons.add_link, color: Colors.white),
                    label: Text('Link New Context', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Content List Section
            Expanded(
              child: Obx(() {
                if (controller.isLoading.value) {
                  return const Center(child: CircularProgressIndicator(color: AppColor.accent));
                }
                if (controller.careContexts.isEmpty) {
                  return Center(
                    child: Container(
                      padding: const EdgeInsets.all(30),
                      decoration: glassDecoration(),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(Icons.folder_open, size: 60, color: AppColor.textSecondary),
                          const SizedBox(height: 16),
                          Text("No Linked Care Contexts Found", style: fontBold),
                          const SizedBox(height: 6),
                          Text("Click 'Link New Context' to connect OPD/IPD records to patient ABHA.", style: fontSmall),
                        ],
                      ),
                    ),
                  );
                }

                return ListView.builder(
                  itemCount: controller.careContexts.length,
                  itemBuilder: (context, index) {
                    final item = controller.careContexts[index];
                    return Container(
                      margin: const EdgeInsets.only(bottom: 16),
                      padding: const EdgeInsets.all(18),
                      decoration: glassDecoration(),
                      child: Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(12),
                            decoration: BoxDecoration(
                              color: AppColor.primary.withOpacity(0.15),
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: const Icon(Icons.medical_services, color: AppColor.accent, size: 24),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(item.display, style: fontBold.copyWith(fontSize: 16)),
                                const SizedBox(height: 4),
                                Row(
                                  children: [
                                    Text('Reference: ${item.referenceNumber}', style: fontSmall.copyWith(color: AppColor.accent)),
                                    const SizedBox(width: 16),
                                    Text('Date: ${item.date}', style: fontSmall),
                                  ],
                                ),
                              ],
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                            decoration: BoxDecoration(
                              color: AppColor.info.withOpacity(0.15),
                              borderRadius: BorderRadius.circular(20),
                              border: Border.all(color: AppColor.info.withOpacity(0.3)),
                            ),
                            child: Text(item.type, style: fontSmall.copyWith(color: AppColor.info, fontWeight: FontWeight.bold)),
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
