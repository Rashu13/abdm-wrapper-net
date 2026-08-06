import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:get/get.dart';

import 'package:qr_flutter/qr_flutter.dart';
import '../../../data/model/response/m1/abha_profile_model.dart';
import '../controllers/abha_creation_controller.dart';
import '../widgets/aadhaar_verification_card.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

import 'package:abdm_frontend/app/routes/app_paths.dart';

class GalaxyHealthAbhaView extends GetView<AbhaCreationController> {
  GalaxyHealthAbhaView({Key? key}) : super(key: key);

  final TextEditingController _otpController = TextEditingController();
  final TextEditingController _customHandleController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 10),
          child: Container(
            constraints: const BoxConstraints(maxWidth: 850),
            child: Obx(() {
              // If user selected Create ABHA or an OTP transaction is active, show the Creation Form
              if (controller.isFormActive.value ||
                  controller.txnId.value.isNotEmpty ||
                  controller.isCardCompleted.value ||
                  controller.abhaProfile.value != null) {
                return _buildAbhaFormWidget(context);
              }

              // Default Home View: Show ONLY the 5 Quick Action Cards Grid
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.only(bottom: 12),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          "ABDM Gateway Dashboard",
                          style: fontBold.copyWith(fontSize: 22, color: const Color(0xFF0F172A)),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          "Select a module below to proceed",
                          style: fontRegular.copyWith(fontSize: 14, color: const Color(0xFF64748B)),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 12),
                  _buildQuickActionCards(context),
                ],
              );
            }),
          ),
        ),
      ),
    );
  }

  Widget _buildStepIndicator(int currentStep) {
    final steps = ['Aadhaar', 'OTP Verify', 'ABHA Address', 'Success'];
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 18, horizontal: 16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0xFFE2E8F0)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.015),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        children: List.generate(steps.length, (index) {
          final stepNum = index + 1;
          final isCompleted = stepNum < currentStep;
          final isActive = stepNum == currentStep;

          return Expanded(
            child: Row(
              children: [
                // Step Circle
                AnimatedContainer(
                  duration: const Duration(milliseconds: 300),
                  width: 28,
                  height: 28,
                  decoration: BoxDecoration(
                    color: isCompleted
                        ? const Color(0xFF10B981)
                        : (isActive ? const Color(0xFF2563EB) : const Color(0xFFF1F5F9)),
                    shape: BoxShape.circle,
                    border: Border.all(
                      color: isActive ? const Color(0xFF2563EB) : Colors.transparent,
                      width: 1.5,
                    ),
                  ),
                  child: Center(
                    child: isCompleted
                        ? const Icon(Icons.check, color: Colors.white, size: 14)
                        : Text(
                            '$stepNum',
                            style: TextStyle(
                              color: isActive ? Colors.white : const Color(0xFF64748B),
                              fontSize: 12,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                  ),
                ),
                const SizedBox(width: 8),
                // Step Title
                Expanded(
                  child: Text(
                    steps[index],
                    style: TextStyle(
                      color: isActive
                          ? const Color(0xFF0F172A)
                          : (isCompleted ? const Color(0xFF10B981) : const Color(0xFF94A3B8)),
                      fontSize: 12.5,
                      fontWeight: isActive ? FontWeight.bold : FontWeight.normal,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                if (index < steps.length - 1) ...[
                  const SizedBox(width: 8),
                  Container(
                    width: 20,
                    height: 2,
                    color: isCompleted ? const Color(0xFF10B981) : const Color(0xFFE2E8F0),
                  ),
                  const SizedBox(width: 8),
                ],
              ],
            ),
          );
        }),
      ),
    );
  }

  // Interactive Form Widget Wrapper
  Widget _buildAbhaFormWidget(BuildContext context) {
    return Obx(() {
      int currentStep = 1;
      if (controller.isCardCompleted.value) {
        currentStep = 4;
      } else if (controller.txnId.value.isNotEmpty &&
          controller.suggestionsList.isNotEmpty) {
        currentStep = 3;
      } else if (controller.txnId.value.isNotEmpty) {
        currentStep = 2;
      }

      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildStepIndicator(currentStep),
          const SizedBox(height: 16),
          _buildAbhaFormWidgetInner(context),
        ],
      );
    });
  }

  Widget _buildAbhaFormWidgetInner(BuildContext context) {
    return Container(
      padding: EdgeInsets.zero,
      child: Obx(() {
        final profile = controller.abhaProfile.value;

        // Success Modal Dialog Overlay
        if (controller.isAbhaCreatedSuccess.value) {
          return Stack(
            alignment: Alignment.center,
            children: [
              if (profile != null && profile.abhaNumber != null)
                Opacity(opacity: 0.3, child: _buildCardDisplay(profile)),
              Container(
                constraints: const BoxConstraints(maxWidth: 420),
                padding:
                    const EdgeInsets.symmetric(horizontal: 24, vertical: 28),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.25),
                      blurRadius: 24,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Align(
                      alignment: Alignment.topRight,
                      child: GestureDetector(
                        onTap: () =>
                            controller.isAbhaCreatedSuccess.value = false,
                        child: const Icon(Icons.close,
                            color: Color(0xFF64748B), size: 20),
                      ),
                    ),
                    const SizedBox(height: 8),
                    Container(
                      width: 58,
                      height: 58,
                      decoration: const BoxDecoration(
                        color: Color(0xFF1E3A60),
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(Icons.check,
                          color: Colors.white, size: 30),
                    ),
                    const SizedBox(height: 20),
                    Text(
                      'ABHA ID Created Successfully',
                      textAlign: TextAlign.center,
                      style: fontBold.copyWith(
                          color: const Color(0xFF1E293B), fontSize: 18),
                    ),
                    const SizedBox(height: 12),
                    Text(
                      'Your ABHA (Ayushman Bharat Health Account) has been created successfully.\n\nYou can now download your ABHA card.',
                      textAlign: TextAlign.center,
                      style: fontRegular.copyWith(
                        color: const Color(0xFF64748B),
                        fontSize: 13,
                        height: 1.45,
                      ),
                    ),
                    const SizedBox(height: 26),
                    SizedBox(
                      width: 200,
                      height: 44,
                      child: ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: const Color(0xFF1E3A60),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                          elevation: 0,
                        ),
                        onPressed: () =>
                            controller.isAbhaCreatedSuccess.value = false,
                        child: Text(
                          'View ABHA Card',
                          style: fontBold.copyWith(
                              color: Colors.white, fontSize: 14),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          );
        }

        // If ABHA Card profile created and completed, show ABHA Card
        if (controller.isCardCompleted.value && profile != null) {
          return _buildCardDisplay(profile);
        }

        // Confirmation Required Screen (If existing ABHA addresses found)
        if (controller.isConfirmationRequired.value) {
          String existingAddrsStr = controller.existingAbhaList.join(', ');
          return Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.08),
                  blurRadius: 20,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Header Bar
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                  decoration: BoxDecoration(
                    color: const Color(0xFFEDF4FE),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Container(
                        padding: const EdgeInsets.all(6),
                        decoration: const BoxDecoration(
                          color: Color(0xFF0F2A4A),
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(Icons.help_outline,
                            color: Colors.white, size: 20),
                      ),
                      const SizedBox(width: 12),
                      Text(
                        'Confirmation Required',
                        style: fontBold.copyWith(
                            color: const Color(0xFF0F2A4A), fontSize: 16),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 20),

                // Body Text Container
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: const Color(0xFFF8FAFC),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'ABHA addresses already exist for this Aadhaar. The following ABHA addresses are linked:',
                        style: fontRegular.copyWith(
                            color: const Color(0xFF334155),
                            fontSize: 13,
                            height: 1.4),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        existingAddrsStr.isNotEmpty
                            ? existingAddrsStr
                            : 'ravikumar27031994@abdm, ravi.kumar.cgp@abdm.',
                        style: fontMedium.copyWith(
                            color: const Color(0xFF1E293B),
                            fontSize: 13,
                            height: 1.4,
                            fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        'Do you want to create new ABHA Address?',
                        style: fontRegular.copyWith(
                            color: const Color(0xFF334155), fontSize: 13),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),

                // Action Buttons Row
                Row(
                  children: [
                    // Button 1: Yes, Create New
                    Expanded(
                      child: SizedBox(
                        height: 44,
                        child: OutlinedButton(
                          style: OutlinedButton.styleFrom(
                            side: const BorderSide(color: Color(0xFFCBD5E1)),
                            shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8)),
                          ),
                          onPressed: () {
                            controller.isConfirmationRequired.value = false;
                            controller.fetchSuggestions();
                          },
                          child: Text(
                            'Yes, Create New',
                            style: fontMedium.copyWith(
                                color: const Color(0xFF1E293B), fontSize: 13),
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: 16),

                    // Button 2: No, View Existing
                    Expanded(
                      child: SizedBox(
                        height: 44,
                        child: ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF1E3A60),
                            shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8)),
                            elevation: 0,
                          ),
                          onPressed: () {
                            controller.isConfirmationRequired.value = false;
                            String firstHandle =
                                controller.existingAbhaList.isNotEmpty
                                    ? controller.existingAbhaList.first
                                    : 'ravikumar27031994@abdm';
                            controller.selectedAbhaHandle.value = firstHandle;
                            controller.finalizeAbhaCreation(firstHandle);
                          },
                          child: Text(
                            'No, View Existing',
                            style: fontBold.copyWith(
                                color: Colors.white, fontSize: 13),
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          );
        }

        // Step 3: Choose ABHA Address (Dark Galaxy Theme)
        if (controller.txnId.value.isNotEmpty &&
            controller.suggestionsList.isNotEmpty) {
          return Container(
            decoration: BoxDecoration(
              color: AppColor.surface,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColor.border.withOpacity(0.6)),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.25),
                  blurRadius: 16,
                  offset: const Offset(0, 8),
                ),
              ],
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Card Header
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
                  decoration: BoxDecoration(
                    color: AppColor.background.withOpacity(0.6),
                    borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(16),
                      topRight: Radius.circular(16),
                    ),
                    border: Border(bottom: BorderSide(color: AppColor.border.withOpacity(0.5))),
                  ),
                  child: Text(
                    'ABHA ID Setup',
                    style: fontBold.copyWith(color: Colors.white, fontSize: 16),
                  ),
                ),

                Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Subheading Text
                      Text(
                        'Choose a username to set up your ABHA ID',
                        style: fontRegular.copyWith(color: AppColor.textSecondary, fontSize: 13),
                      ),
                      const SizedBox(height: 16),

                      // Outlined ABHA Address Field
                      TextField(
                        controller: _customHandleController,
                        style: fontMedium.copyWith(color: Colors.white, fontSize: 15),
                        decoration: InputDecoration(
                          labelText: 'Your ABHA Address',
                          labelStyle: fontMedium.copyWith(color: AppColor.accent, fontSize: 14),
                          hintText: 'Enter ABHA Address',
                          hintStyle: fontRegular.copyWith(color: AppColor.textSecondary),
                          floatingLabelBehavior: FloatingLabelBehavior.always,
                          filled: true,
                          fillColor: AppColor.background,
                          contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(10),
                            borderSide: BorderSide(color: AppColor.border),
                          ),
                          focusedBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(10),
                            borderSide: const BorderSide(color: AppColor.accent, width: 2),
                          ),
                        ),
                      ),
                      const SizedBox(height: 20),

                      // Validation Rules Box
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          color: AppColor.background.withOpacity(0.8),
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: AppColor.border.withOpacity(0.4)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'ABHA Address Validation rule:',
                              style: fontBold.copyWith(color: Colors.white70, fontSize: 13),
                            ),
                            const SizedBox(height: 10),
                            _buildRuleBullet('Minimum length 8, Maximum length 18 characters'),
                            const SizedBox(height: 6),
                            _buildRuleBullet('Allowed special characters: 1 dot (.) and/or 1 underscore (_)'),
                            const SizedBox(height: 6),
                            _buildRuleBullet('Special characters cannot be at the beginning or end'),
                            const SizedBox(height: 6),
                            _buildRuleBullet('Only alphanumeric characters allowed (with . and _)'),
                          ],
                        ),
                      ),
                      const SizedBox(height: 24),

                      // Suggestions Section Header
                      Text(
                        'Suggestions:',
                        style: fontMedium.copyWith(color: AppColor.textSecondary, fontSize: 13),
                      ),
                      const SizedBox(height: 12),

                      // Suggestions Oval Chips
                      Obx(() => Wrap(
                            spacing: 10,
                            runSpacing: 10,
                            children: controller.suggestionsList.map((handle) {
                              bool isSelected = controller.selectedAbhaHandle.value == handle ||
                                  _customHandleController.text == handle;
                              return InkWell(
                                onTap: () {
                                  controller.selectedAbhaHandle.value = handle;
                                  _customHandleController.text = handle;
                                },
                                borderRadius: BorderRadius.circular(20),
                                child: Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                                  decoration: BoxDecoration(
                                    color: isSelected
                                        ? AppColor.accent.withOpacity(0.2)
                                        : AppColor.background,
                                    borderRadius: BorderRadius.circular(20),
                                    border: Border.all(
                                      color: isSelected ? AppColor.accent : AppColor.border,
                                      width: isSelected ? 1.5 : 1,
                                    ),
                                  ),
                                  child: Text(
                                    handle,
                                    style: fontMedium.copyWith(
                                      fontSize: 13,
                                      color: isSelected ? AppColor.accent : Colors.white70,
                                      fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                                    ),
                                  ),
                                ),
                              );
                            }).toList(),
                          )),
                      const SizedBox(height: 28),

                      // Create ABHA ID Button
                      SizedBox(
                        width: double.infinity,
                        height: 48,
                        child: ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColor.primary,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10),
                            ),
                            elevation: 4,
                          ),
                          onPressed: () => controller.finalizeAbhaCreation(_customHandleController.text),
                          child: Text(
                            'Create ABHA ID',
                            style: fontBold.copyWith(color: Colors.white, fontSize: 16),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        }

        if (controller.txnId.value.isNotEmpty) {
          // Step 2: Verify OTP (Exact match to screenshot spec)
          String rawMob = controller.communicationMobile.value.isNotEmpty
              ? controller.communicationMobile.value
              : controller.inputNumber.value;
          String maskedMob = rawMob.length >= 4
              ? '******${rawMob.substring(rawMob.length - 4)}'
              : '';

          return Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: AppColor.background,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColor.border.withValues(alpha: 0.4)),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.2),
                  blurRadius: 16,
                  offset: const Offset(0, 8),
                ),
              ],
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Info Banner Box
                Container(
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: const Color(0xFFE8EFF5),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Icon(Icons.info,
                          color: Color(0xFF0F4C81), size: 22),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Text(
                          'We just sent an OTP on the Mobile Number $maskedMob linked with Aadhaar. Enter the OTP below to proceed with ABHA creation',
                          style: fontRegular.copyWith(
                            color: const Color(0xFF1E293B),
                            fontSize: 13,
                            height: 1.45,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),

                // OTP Verification Input Field
                TextField(
                  controller: _otpController,
                  keyboardType: TextInputType.number,
                  maxLength: 6,
                  style: fontMedium.copyWith(fontSize: 15, letterSpacing: 2),
                  decoration: InputDecoration(
                    labelText: 'OTP Verification',
                    hintText: 'Enter OTP',
                    floatingLabelBehavior: FloatingLabelBehavior.always,
                    counterText: '',
                    contentPadding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 16),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(color: Color(0xFFCBD5E1)),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide:
                          const BorderSide(color: AppColor.accent, width: 2),
                    ),
                  ),
                ),
                const SizedBox(height: 14),

                // Communication Mobile Input Field
                TextFormField(
                  initialValue: controller.communicationMobile.value,
                  keyboardType: TextInputType.phone,
                  maxLength: 10,
                  onChanged: (v) => controller.communicationMobile.value = v.trim(),
                  style: fontMedium.copyWith(fontSize: 14),
                  decoration: InputDecoration(
                    labelText: 'Communication Mobile Number',
                    hintText: 'Enter 10-digit Mobile Number',
                    floatingLabelBehavior: FloatingLabelBehavior.always,
                    counterText: '',
                    prefixIcon: const Icon(Icons.phone_android, size: 18, color: Color(0xFF0F4C81)),
                    contentPadding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 14),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(color: Color(0xFFCBD5E1)),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide:
                          const BorderSide(color: AppColor.accent, width: 2),
                    ),
                  ),
                ),
                const SizedBox(height: 14),

                // Resend Timer & Action Button
                Obx(() {
                  if (controller.resendCount.value >= 2) {
                    return Text(
                      'Maximum resend limit reached (2/2)',
                      style: fontSmall.copyWith(
                        color: Colors.red,
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                      ),
                    );
                  }

                  final isTimerActive = controller.resendSeconds.value > 0;
                  final secFormatted = controller.resendSeconds.value.toString().padLeft(2, '0');

                  return Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        isTimerActive
                            ? 'Resend in 00:$secFormatted'
                            : 'Didn\'t receive OTP?',
                        style: fontSmall.copyWith(
                          color: const Color(0xFF94A3B8),
                          fontSize: 13,
                        ),
                      ),
                      TextButton.icon(
                        onPressed: controller.canResend.value && !controller.isLoading.value
                            ? () => controller.handleResendOtp()
                            : null,
                        style: TextButton.styleFrom(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          minimumSize: Size.zero,
                          tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        ),
                        icon: Icon(
                          Icons.refresh,
                          size: 14,
                          color: controller.canResend.value ? AppColor.accent : Colors.grey,
                        ),
                        label: Text(
                          controller.resendCount.value == 0
                              ? 'Resend OTP'
                              : 'Resend OTP (${controller.resendCount.value}/2)',
                          style: fontMedium.copyWith(
                            color: controller.canResend.value ? AppColor.accent : Colors.grey,
                            fontSize: 13,
                          ),
                        ),
                      ),
                    ],
                  );
                }),
                const SizedBox(height: 24),

                // Verify Action Button
                SizedBox(
                  width: double.infinity,
                  height: 48,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF4C809E),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                      elevation: 0,
                    ),
                    onPressed: () => controller.verifyOtp(_otpController.text),
                    child: Text(
                      'Verify',
                      style: fontBold.copyWith(
                        color: Colors.white,
                        fontSize: 16,
                      ),
                    ),
                  ),
                ),
              ],
            ),
          );
        }

        // Step 1: Generate OTP Aadhaar Verification Card
        return AadhaarVerificationCard(controller: controller);
      }),
    );
  }

  Widget _buildCardDisplay(AbhaProfileModel profile) {
    Widget photoWidget;
    if (profile.profilePhoto != null && profile.profilePhoto!.isNotEmpty) {
      if (profile.profilePhoto!.startsWith('http')) {
        photoWidget = ClipRRect(
          borderRadius: BorderRadius.circular(8),
          child: Image.network(profile.profilePhoto!, width: 70, height: 70, fit: BoxFit.cover),
        );
      } else {
        try {
          String cleanBase64 = profile.profilePhoto!.contains(',')
              ? profile.profilePhoto!.split(',').last
              : profile.profilePhoto!;
          photoWidget = ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: Image.memory(base64Decode(cleanBase64), width: 70, height: 70, fit: BoxFit.cover),
          );
        } catch (e) {
          photoWidget = _buildDefaultPhotoAvatar();
        }
      }
    } else {
      photoWidget = _buildDefaultPhotoAvatar();
    }

    return Column(
      children: [
        // Official ABHA Card Surface
        Container(
          padding: const EdgeInsets.all(22),
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [Color(0xFF0F3B68), Color(0xFF0F766E)],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.3),
                blurRadius: 20,
                offset: const Offset(0, 8),
              ),
            ],
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Top Header Row
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      const Icon(Icons.health_and_safety, color: Colors.white, size: 22),
                      const SizedBox(width: 8),
                      Text(
                        "AYUSHMAN BHARAT HEALTH ACCOUNT",
                        style: fontBold.copyWith(color: Colors.white, fontSize: 13, letterSpacing: 0.5),
                      ),
                    ],
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: const Color(0xFF10B981),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      "ACTIVE",
                      style: fontSmall.copyWith(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 10),
                    ),
                  ),
                ],
              ),
              const Divider(color: Colors.white24, height: 24),

              // Profile Photo & Core Details Row
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Profile Photo
                  Container(
                    padding: const EdgeInsets.all(3),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: photoWidget,
                  ),
                  const SizedBox(width: 16),

                  // Name & Identifiers
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          profile.name ?? 'Galaxy User',
                          style: fontBold.copyWith(color: Colors.white, fontSize: 18),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'ABHA Number: ${profile.abhaNumber ?? '91-6505-0651-2757'}',
                          style: fontBold.copyWith(color: const Color(0xFF38BDF8), fontSize: 14),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          'ABHA Address: ${profile.abhaAddress ?? 'user@abdm'}',
                          style: fontMedium.copyWith(color: Colors.white.withOpacity(0.9), fontSize: 13),
                        ),
                      ],
                    ),
                  ),

                  // QR Code
                  Container(
                    padding: const EdgeInsets.all(5),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: QrImageView(
                      data: 'ABHA:${profile.abhaNumber ?? profile.abhaAddress ?? ''}',
                      version: QrVersions.auto,
                      size: 72,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),

              // Bottom Profile Info Row (Gender, DOB, Mobile)
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                decoration: BoxDecoration(
                  color: Colors.black.withOpacity(0.2),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        _buildInfoColumn('GENDER',
                            profile.gender == 'F' ? 'Female' : profile.gender == 'M' ? 'Male' : profile.gender ?? 'N/A'),
                        _buildInfoColumn('DATE OF BIRTH', profile.dob ?? profile.yearOfBirth ?? 'N/A'),
                        _buildInfoColumn('MOBILE',
                            profile.mobile != null && profile.mobile!.length >= 10
                                ? '******${profile.mobile!.substring(profile.mobile!.length - 4)}'
                                : profile.mobile ?? 'N/A'),
                      ],
                    ),
                    if (profile.email != null || profile.address != null || profile.pincode != null) ...[
                      const SizedBox(height: 8),
                      const Divider(color: Colors.white24, height: 1),
                      const SizedBox(height: 8),
                    ],
                    if (profile.email != null)
                      _buildInfoRow(Icons.email_outlined, profile.email!),
                    if (profile.address != null)
                      _buildInfoRow(Icons.location_on_outlined, profile.address!),
                    if (profile.stateName != null || profile.districtName != null || profile.pincode != null)
                      _buildInfoRow(
                        Icons.map_outlined,
                        [
                          if (profile.districtName != null) profile.districtName!,
                          if (profile.stateName != null) profile.stateName!,
                          if (profile.pincode != null) 'PIN: ${profile.pincode}',
                        ].join(', '),
                      ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 20),

        // Action Buttons Row: View ABHA Card & Download ABHA Card
        Row(
          children: [
            Expanded(
              child: SizedBox(
                height: 50,
                child: ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF0F4C81),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    elevation: 4,
                  ),
                  onPressed: () {
                    Get.toNamed(Routes.M1_ABHA_CARD);
                  },
                  icon: const Icon(Icons.credit_card, color: Colors.white, size: 20),
                  label: Text(
                    'View ABHA Card',
                    style: fontBold.copyWith(color: Colors.white, fontSize: 14),
                  ),
                ),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: SizedBox(
                height: 50,
                child: Obx(() => ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF2563EB),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    elevation: 4,
                  ),
                  onPressed: controller.isDownloadingCard.value
                      ? null
                      : () => controller.downloadAbhaCard(),
                  icon: controller.isDownloadingCard.value
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                          ),
                        )
                      : const Icon(Icons.file_download_outlined, color: Colors.white, size: 20),
                  label: Text(
                    controller.isDownloadingCard.value
                        ? 'Downloading...'
                        : 'Download Card',
                    style: fontBold.copyWith(color: Colors.white, fontSize: 14),
                  ),
                )),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),

        // Register Patient in HIP Database Action Button
        SizedBox(
          width: double.infinity,
          height: 50,
          child: Obx(() => ElevatedButton.icon(
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFF10B981),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(10),
              ),
              elevation: 4,
            ),
            onPressed: controller.isRegisteringDb.value
                ? null
                : () => controller.registerPatientInDatabase(),
            icon: controller.isRegisteringDb.value
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                    ),
                  )
                : const Icon(Icons.how_to_reg, color: Colors.white, size: 20),
            label: Text(
              controller.isRegisteringDb.value
                  ? 'Registering in DB...'
                  : 'REGISTER PATIENT IN DATABASE',
              style: fontBold.copyWith(color: Colors.white, fontSize: 14),
            ),
          )),
        ),
      ],
    );
  }

  Widget _buildDefaultPhotoAvatar() {
    return Container(
      width: 70,
      height: 70,
      decoration: BoxDecoration(
        color: const Color(0xFFE2E8F0),
        borderRadius: BorderRadius.circular(8),
      ),
      child: const Icon(Icons.person, color: Color(0xFF64748B), size: 45),
    );
  }

  Widget _buildInfoColumn(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: fontSmall.copyWith(color: Colors.white54, fontSize: 10, letterSpacing: 0.5),
        ),
        const SizedBox(height: 2),
        Text(
          value,
          style: fontBold.copyWith(color: Colors.white, fontSize: 12),
        ),
      ],
    );
  }

  Widget _buildInfoRow(IconData icon, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        children: [
          Icon(icon, color: Colors.white60, size: 14),
          const SizedBox(width: 6),
          Expanded(
            child: Text(
              value,
              style: fontRegular.copyWith(color: Colors.white.withOpacity(0.85), fontSize: 12),
              overflow: TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildRuleBullet(String text) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Padding(
          padding: EdgeInsets.only(top: 5, right: 8),
          child: Icon(Icons.circle, color: Color(0xFFF87171), size: 6),
        ),
        Expanded(
          child: Text(
            text,
            style: fontRegular.copyWith(
                color: const Color(0xFFEF4444), fontSize: 12, height: 1.35),
          ),
        ),
      ],
    );
  }

  Widget _buildQuickActionCards(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        int crossAxisCount = constraints.maxWidth > 650 ? 2 : 1;
        double childAspectRatio = constraints.maxWidth > 650 ? 2.8 : 2.5;

        return GridView(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: crossAxisCount,
            crossAxisSpacing: 16,
            mainAxisSpacing: 16,
            childAspectRatio: childAspectRatio,
          ),
          children: [
            _buildMiniActionCard(
              icon: Icons.add_circle_outline,
              title: "Create ABHA Number & Link",
              subtitle: "Generate a new ABHA number and link patient records",
              onTap: () {
                controller.txnId.value = '';
                controller.isAbhaCreatedSuccess.value = false;
                controller.isFormActive.value = true;
              },
            ),
            _buildMiniActionCard(
              icon: Icons.people_outline,
              title: "Patients List",
              subtitle: "View and manage all registered patients",
              onTap: () => Get.toNamed(Routes.M2_DISCOVERY),
            ),
            _buildMiniActionCard(
              icon: Icons.link,
              title: "Link Health Records",
              subtitle: "Link existing health records to ABHA",
              onTap: () => Get.toNamed(Routes.M2_DISCOVERY),
            ),
            _buildMiniActionCard(
              icon: Icons.shield_outlined,
              title: "Consent Management",
              subtitle: "Request and manage patient consent",
              onTap: () => Get.toNamed(Routes.M3_CONSENT_REQUEST),
            ),
            _buildMiniActionCard(
              icon: Icons.history,
              title: "Token History",
              subtitle: "View token & session history",
              onTap: () => _showTokenModal(context),
            ),
          ],
        );
      },
    );
  }

  Widget _buildMiniActionCard({
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(18),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: const Color(0xFFE2E8F0)),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.02),
              blurRadius: 8,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 48,
              height: 48,
              decoration: BoxDecoration(
                color: const Color(0xFFF1F5F9),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, size: 22, color: const Color(0xFF334155)),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: fontBold.copyWith(
                        fontSize: 15, color: const Color(0xFF0F172A)),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 3),
                  Text(
                    subtitle,
                    style: fontRegular.copyWith(
                        fontSize: 12, color: const Color(0xFF64748B), height: 1.25),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showTokenModal(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => Dialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Container(
          width: 480,
          padding: const EdgeInsets.all(20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text("ABDM Token Status",
                      style: fontBold.copyWith(fontSize: 16)),
                  IconButton(
                    onPressed: () => Navigator.pop(context),
                    icon: const Icon(Icons.close),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Text("Gateway Bearer Token:", style: fontMedium.copyWith(fontSize: 13)),
              const SizedBox(height: 4),
              SelectableText(
                "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3Mi...",
                style: fontRegular.copyWith(fontSize: 11, color: Color(0xFF64748B)),
              ),
              const SizedBox(height: 16),
              Align(
                alignment: Alignment.centerRight,
                child: TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text("Close"),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
