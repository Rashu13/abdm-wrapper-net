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
              const SizedBox(height: 24),

              Text(
                'Communication Mobile Number',
                style: fontMedium.copyWith(fontSize: 14, color: AppColor.accent),
              ),
              const SizedBox(height: 6),
              TextFormField(
                initialValue: controller.communicationMobile.value,
                keyboardType: TextInputType.phone,
                maxLength: 10,
                onChanged: (v) => controller.communicationMobile.value = v.trim(),
                decoration: InputDecoration(
                  hintText: 'Enter 10-digit mobile number for ABHA card',
                  counterText: '',
                  prefixIcon: const Icon(Icons.phone_android, size: 20, color: AppColor.accent),
                  contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: const BorderSide(color: AppColor.border),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: const BorderSide(color: AppColor.accent, width: 2),
                  ),
                ),
              ),
              const SizedBox(height: 28),

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
                child: Obx(() {
                  if (controller.resendCount.value >= 2) {
                    return Container(
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                      decoration: BoxDecoration(
                        color: Colors.red.withOpacity(0.08),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: Colors.red.withOpacity(0.3)),
                      ),
                      child: Text(
                        'Maximum resend limit reached (2/2)',
                        style: fontMedium.copyWith(color: Colors.red, fontSize: 13),
                      ),
                    );
                  }

                  final isTimerActive = controller.resendSeconds.value > 0;
                  final secFormatted = controller.resendSeconds.value.toString().padLeft(2, '0');

                  return Column(
                    children: [
                      if (isTimerActive)
                        Text(
                          'Resend OTP in 00:$secFormatted',
                          style: fontMedium.copyWith(color: const Color(0xFF64748B), fontSize: 13),
                        ),
                      const SizedBox(height: 4),
                      TextButton.icon(
                        onPressed: controller.canResend.value && !controller.isLoading.value
                            ? () => controller.handleResendOtp()
                            : null,
                        icon: Icon(
                          Icons.refresh,
                          color: controller.canResend.value ? AppColor.accent : Colors.grey,
                        ),
                        label: Text(
                          controller.resendCount.value == 0
                              ? 'Resend Authentication OTP'
                              : 'Resend OTP (${controller.resendCount.value}/2)',
                          style: fontMedium.copyWith(
                            color: controller.canResend.value ? AppColor.accent : Colors.grey,
                          ),
                        ),
                      ),
                    ],
                  );
                }),
              )
            ],
          ),
        ),
      ),
    );
  }
}
