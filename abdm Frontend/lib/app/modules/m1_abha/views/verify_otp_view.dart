import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:pinput/pinput.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class VerifyOtpView extends GetView<AbhaCreationController> {
  VerifyOtpView({Key? key}) : super(key: key);

  final TextEditingController _otpController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    final defaultPinTheme = PinTheme(
      width: 54,
      height: 60,
      textStyle: fontBold.copyWith(fontSize: 22, color: AppColor.accent),
      decoration: BoxDecoration(
        color: AppColor.background,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColor.border),
      ),
    );

    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        title: Obx(() => Text(controller.isCreateMode.value ? 'Verify Aadhaar OTP (M1)' : 'Verify Login OTP')),
        backgroundColor: AppColor.surface,
        elevation: 0,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Container(
          padding: const EdgeInsets.all(28),
          decoration: glassDecoration(),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Obx(() => Text(
                    controller.isCreateMode.value ? 'Step 2: Enter Aadhaar Authentication OTP' : 'Enter ABHA Login OTP',
                    style: fontBold.copyWith(fontSize: 18),
                  )),
              const SizedBox(height: 8),
              Obx(() => Text(
                    'An OTP has been dispatched to mobile registered with ${controller.inputNumber.value}',
                    style: fontSmall,
                  )),
              const SizedBox(height: 32),

              Center(
                child: Pinput(
                  controller: _otpController,
                  length: 6,
                  defaultPinTheme: defaultPinTheme,
                  focusedPinTheme: defaultPinTheme.copyWith(
                    decoration: defaultPinTheme.decoration!.copyWith(
                      border: Border.all(color: AppColor.accent, width: 2),
                    ),
                  ),
                  onCompleted: (pin) => controller.verifyOtp(pin),
                ),
              ),
              const SizedBox(height: 36),

              Obx(() => SizedBox(
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
                        onPressed: controller.isLoading.value
                            ? null
                            : () => controller.verifyOtp(_otpController.text),
                        child: controller.isLoading.value
                            ? const CircularProgressIndicator(color: Colors.white)
                            : Text('Verify OTP & Proceed', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
                      ),
                    ),
                  )),
              const SizedBox(height: 20),

              Center(
                child: TextButton.icon(
                  onPressed: () => controller.sendOtp(controller.inputNumber.value),
                  icon: const Icon(Icons.refresh, color: AppColor.accent),
                  label: Text('Resend Authentication OTP', style: fontMedium.copyWith(color: AppColor.accent)),
                ),
              )
            ],
          ),
        ),
      ),
    );
  }
}
