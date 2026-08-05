import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/abha_creation_controller.dart';
import '../widgets/aadhaar_verification_card.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class CreateAbhaView extends GetView<AbhaCreationController> {
  CreateAbhaView({Key? key}) : super(key: key);

  final TextEditingController _mobileInputController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header Title Banner
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
                    child: const Icon(Icons.badge_outlined, color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Milestone M1: ABHA Creation & Authentication", style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Create new ABHA via Aadhaar OTP or Login with registered ABHA Mobile.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Mode Switcher Bar (Glassmorphic)
            Obx(() => Container(
                  padding: const EdgeInsets.all(6),
                  decoration: glassDecoration(radius: 14),
                  child: Row(
                    children: [
                      Expanded(
                        child: GestureDetector(
                          onTap: () => controller.setMode(true),
                          child: AnimatedContainer(
                            duration: const Duration(milliseconds: 200),
                            padding: const EdgeInsets.symmetric(vertical: 14),
                            decoration: BoxDecoration(
                              gradient: controller.isCreateMode.value ? AppColor.primaryGradient : null,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Center(
                              child: Text(
                                'Create New ABHA (Aadhaar)',
                                style: fontMedium.copyWith(
                                  color: controller.isCreateMode.value ? Colors.white : AppColor.textSecondary,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ),
                        ),
                      ),
                      Expanded(
                        child: GestureDetector(
                          onTap: () => controller.setMode(false),
                          child: AnimatedContainer(
                            duration: const Duration(milliseconds: 200),
                            padding: const EdgeInsets.symmetric(vertical: 14),
                            decoration: BoxDecoration(
                              gradient: !controller.isCreateMode.value ? AppColor.primaryGradient : null,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Center(
                              child: Text(
                                'ABHA Login (Mobile)',
                                style: fontMedium.copyWith(
                                  color: !controller.isCreateMode.value ? Colors.white : AppColor.textSecondary,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                )),
            const SizedBox(height: 24),

            // Main View Area: Aadhaar Verification Card (Create Mode) or Login Form (Login Mode)
            Obx(() {
              bool isCreate = controller.isCreateMode.value;
              if (isCreate) {
                return AadhaarVerificationCard(controller: controller);
              }

              // Login Form
              return Container(
                padding: const EdgeInsets.all(24),
                decoration: glassDecoration(),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Enter Registered Mobile / ABHA Address', style: fontBold.copyWith(fontSize: 16)),
                    const SizedBox(height: 6),
                    Text('Enter your 10-digit mobile number or ABHA handle (e.g., user@abdm).', style: fontSmall),
                    const SizedBox(height: 24),
                    TextField(
                      controller: _mobileInputController,
                      style: fontMedium,
                      decoration: InputDecoration(
                        labelText: 'Mobile / ABHA Address',
                        hintText: 'e.g., 9876543210 or user@abdm',
                        filled: true,
                        fillColor: AppColor.background,
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: BorderSide(color: AppColor.border),
                        ),
                        prefixIcon: const Icon(Icons.phone_android, color: AppColor.accent),
                      ),
                    ),
                    const SizedBox(height: 24),
                    SizedBox(
                      width: double.infinity,
                      height: 52,
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: AppColor.primaryGradient,
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.transparent,
                            shadowColor: Colors.transparent,
                          ),
                          onPressed: controller.isLoading.value ? null : () => controller.sendOtp(_mobileInputController.text),
                          child: controller.isLoading.value
                              ? const CircularProgressIndicator(color: Colors.white)
                              : Text('Send Login OTP', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
                        ),
                      ),
                    ),
                  ],
                ),
              );
            }),
          ],
        ),
      ),
    );
  }
}
