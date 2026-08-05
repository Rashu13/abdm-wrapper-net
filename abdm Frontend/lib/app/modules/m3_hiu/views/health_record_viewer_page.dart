import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/health_record_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class HealthRecordViewerPage extends GetView<HealthRecordController> {
  const HealthRecordViewerPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        title: const Text('FHIR Decrypted Health Records (M3)'),
        backgroundColor: AppColor.surface,
        elevation: 0,
      ),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
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
                    Text('Decrypting FHIR Data Payload (ECDH Key Exchange)...', style: fontMedium),
                    const SizedBox(height: 6),
                    Text('Connecting to /v3/health-information/fetch-records', style: fontSmall),
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
                    const Icon(Icons.inventory_2_outlined, size: 60, color: AppColor.textSecondary),
                    const SizedBox(height: 16),
                    Text("No FHIR Records Delivered", style: fontBold),
                    const SizedBox(height: 6),
                    Text("Once HIP hospital transfers encrypted bundle, records will be rendered here.", style: fontSmall),
                  ],
                ),
              ),
            );
          }

          return ListView.builder(
            itemCount: controller.fhirRecords.length,
            itemBuilder: (context, index) {
              final record = controller.fhirRecords[index];
              return Container(
                margin: const EdgeInsets.only(bottom: 16),
                decoration: glassDecoration(),
                child: ExpansionTile(
                  leading: Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: AppColor.accent.withOpacity(0.15),
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(Icons.receipt_long, color: AppColor.accent),
                  ),
                  title: Text(record.title, style: fontBold.copyWith(fontSize: 16)),
                  subtitle: Text('Provider: ${record.doctorName} | Date: ${record.date}', style: fontSmall),
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          color: AppColor.background.withOpacity(0.8),
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(color: AppColor.border.withOpacity(0.5)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                const Icon(Icons.code, color: AppColor.accent, size: 18),
                                const SizedBox(width: 8),
                                Text('FHIR Resource Type: ${record.resourceType}', style: fontSmall.copyWith(fontWeight: FontWeight.bold, color: AppColor.accent)),
                              ],
                            ),
                            const SizedBox(height: 10),
                            Text(record.summary, style: fontRegular),
                          ],
                        ),
                      ),
                    )
                  ],
                ),
              );
            },
          );
        }),
      ),
    );
  }
}
