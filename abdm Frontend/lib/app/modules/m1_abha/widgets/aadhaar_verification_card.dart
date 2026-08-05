import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../../routes/app_paths.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

import 'aadhaar_segmented_input.dart';

class AadhaarVerificationCard extends StatelessWidget {
  final AbhaCreationController controller;

  AadhaarVerificationCard({Key? key, required this.controller})
      : super(key: key);

  final TextEditingController _mobileController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: AppColor.surface,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColor.border.withOpacity(0.6)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header Bar with Integrated Back Button
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            decoration: const BoxDecoration(
              color: Color(0xFFEFF6FF), // Skyblue header bar
              borderRadius: BorderRadius.only(
                topLeft: Radius.circular(16),
                topRight: Radius.circular(16),
              ),
              border: Border(bottom: BorderSide(color: Color(0xFFCBD5E1))),
            ),
            child: Row(
              children: [
                InkWell(
                  onTap: () {
                    controller.isFormActive.value = false;
                    controller.txnId.value = '';
                  },
                  borderRadius: BorderRadius.circular(8),
                  child: Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(8),
                      border: Border.all(
                          color: const Color(0xFF0F4C81).withOpacity(0.3)),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.arrow_back,
                            size: 15, color: Color(0xFF0F4C81)),
                        const SizedBox(width: 4),
                        Text(
                          'Back',
                          style: fontBold.copyWith(
                              fontSize: 13, color: const Color(0xFF0F4C81)),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'ABHA',
                        style: fontBold.copyWith(
                            fontSize: 12,
                            color: const Color(0xFF2563EB),
                            letterSpacing: 1),
                      ),
                      Text(
                        'Create Ayushman Bharat Health Account',
                        style: fontBold.copyWith(
                            fontSize: 15, color: const Color(0xFF0F4C81)),
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 10),
                Obx(() => ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF0F4C81),
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 9),
                        elevation: 2,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                      ),
                      onPressed: controller.isLoading.value
                          ? null
                          : () => _showVerificationMethodDialog(context),
                      child: controller.isLoading.value
                          ? const SizedBox(
                              width: 16,
                              height: 16,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : Text(
                              'Verify',
                              style: fontBold.copyWith(
                                fontSize: 13,
                                color: Colors.white,
                              ),
                            ),
                    )),
              ],
            ),
          ),

          Padding(
            padding:
                const EdgeInsets.symmetric(horizontal: 18.0, vertical: 14.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Field 1 Label: Enter your Aadhaar Number to create ABHA
                Text(
                  'Enter your Aadhaar Number to create ABHA',
                  style: fontMedium.copyWith(
                      color: const Color(0xFF334155),
                      fontSize: 14,
                      fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),

                // 3 Segment Input Boxes + Eye Toggle Button
                AadhaarSegmentedInput(controller: controller),
                const SizedBox(height: 14),

                // Select All Consents Row Header
                Obx(() => Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 14, vertical: 8),
                      decoration: BoxDecoration(
                        color: const Color(0xFFEFF6FF),
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                            color: const Color(0xFF0F4C81).withOpacity(0.3)),
                      ),
                      child: Row(
                        children: [
                          SizedBox(
                            width: 24,
                            height: 24,
                            child: Checkbox(
                              value: controller.isSelectAll.value,
                              activeColor: const Color(0xFF0F4C81),
                              checkColor: Colors.white,
                              side: const BorderSide(
                                  color: Color(0xFF64748B), width: 1.8),
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(4)),
                              materialTapTargetSize:
                                  MaterialTapTargetSize.shrinkWrap,
                              onChanged: (val) {
                                if (val != null)
                                  controller.toggleSelectAll(val);
                              },
                            ),
                          ),
                          const SizedBox(width: 12),
                          GestureDetector(
                            onTap: () => controller
                                .toggleSelectAll(!controller.isSelectAll.value),
                            child: Text(
                              'Select All ',
                              style: fontBold.copyWith(
                                  fontSize: 13, color: const Color(0xFF0F4C81)),
                            ),
                          ),
                        ],
                      ),
                    )),
                const SizedBox(height: 2),

                // Symmetrical & Aligned Consent Checkbox Scrollable Box (Compact Height)
                Container(
                  height: 300,
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: const Color(0xFFF8FAFC),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: const Color(0xFFCBD5E1)),
                  ),
                  child: SingleChildScrollView(
                    physics: const BouncingScrollPhysics(),
                    child: Column(
                      children: [
                        // Consent 1: Voluntary Aadhaar Sharing (3-Line Expandable)
                        Obx(() {
                          bool isExpanded = controller.isConsentExpanded.value;
                          bool isChecked = controller.chkDeclaration.value;
                          const String fullText =
                              'I am voluntarily sharing my Aadhaar Number / Virtual ID issued to the Unique Identification Authority of India ("UIDAI"), and my demographic information for the purpose of creating an Ayushman Bharat Health Account number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address"). I authorize NHA to use my Aadhaar number / Virtual ID for performing Aadhaar based authentication with UIDAI as per the provisions of the Aadhaar (Targeted Delivery of Financial and other Subsidies, Benefits and Services) Act, 2016 for the aforesaid purpose. I understand that UIDAI will share my e-KYC details, on response of "Yes" with NHA upon successful authentication.';

                          return _buildSymmetricalCheckboxRow(
                            isChecked: isChecked,
                            onChanged: (v) {
                              controller.chkDeclaration.value = v ?? true;
                              controller.checkSelectAllState();
                            },
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                AnimatedCrossFade(
                                  firstChild: Text(
                                    fullText,
                                    maxLines: 3,
                                    overflow: TextOverflow.ellipsis,
                                    style: fontSmall.copyWith(
                                        fontSize: 12,
                                        color: AppColor.textSecondary,
                                        height: 1.45),
                                  ),
                                  secondChild: Text(
                                    fullText,
                                    style: fontSmall.copyWith(
                                        fontSize: 12,
                                        color: AppColor.textSecondary,
                                        height: 1.45),
                                  ),
                                  crossFadeState: isExpanded
                                      ? CrossFadeState.showSecond
                                      : CrossFadeState.showFirst,
                                  duration: const Duration(milliseconds: 250),
                                ),
                                const SizedBox(height: 4),
                                InkWell(
                                  onTap: () =>
                                      controller.isConsentExpanded.value =
                                          !controller.isConsentExpanded.value,
                                  child: Row(
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      Text(
                                        isExpanded
                                            ? 'Read Less'
                                            : '... Read More',
                                        style: fontSmall.copyWith(
                                            fontSize: 11,
                                            color: AppColor.accent,
                                            fontWeight: FontWeight.bold),
                                      ),
                                      Icon(
                                        isExpanded
                                            ? Icons.keyboard_arrow_up
                                            : Icons.keyboard_arrow_down,
                                        color: AppColor.accent,
                                        size: 16,
                                      ),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          );
                        }),
                        const Divider(height: 24, color: AppColor.border),

                        // Consent 2: Create ABHA with document other than Aadhaar
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked: controller.chkIntendOtherDoc.value,
                              onChanged: (v) {
                                controller.chkIntendOtherDoc.value = v ?? false;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I intend to create Ayushman Bharat Health Account Number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address") using document other than Aadhaar. (Click here to proceed further)',
                            )),
                        const SizedBox(height: 12),

                        // Consent 3: Legacy health records linking
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked:
                                  controller.chkConsentLegacyRecords.value,
                              onChanged: (v) {
                                controller.chkConsentLegacyRecords.value =
                                    v ?? true;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I consent to usage of my ABHA address and ABHA number for linking of my legacy (past) health records and those which will be generated during this encounter.',
                            )),
                        const SizedBox(height: 12),

                        // Consent 4: Sharing health records during encounter
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked: controller.chkShareHealthRecords.value,
                              onChanged: (v) {
                                controller.chkShareHealthRecords.value =
                                    v ?? true;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I authorize the sharing of all my health records with healthcare provider(s) for the purpose of providing healthcare services to me during this encounter.',
                            )),
                        const SizedBox(height: 12),

                        // Consent 5: Anonymization for public health
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked: controller.chkAnonymization.value,
                              onChanged: (v) {
                                controller.chkAnonymization.value = v ?? true;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I consent to the anonymization and subsequent use of my government health records for public health purposes.',
                            )),
                        const SizedBox(height: 12),

                        // Consent 6: Duly informed beneficiary
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked:
                                  controller.chkInformedBeneficiary.value,
                              onChanged: (v) {
                                controller.chkInformedBeneficiary.value =
                                    v ?? true;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I confirm that I have duly informed and explained the beneficiary of the contents of consent for aforementioned purposes.',
                            )),
                        const SizedBox(height: 10),

                        // Consent 7: Final informed consent
                        Obx(() => _buildSymmetricalCheckboxRow(
                              isChecked: controller.chkExplainedConsent.value,
                              onChanged: (v) {
                                controller.chkExplainedConsent.value =
                                    v ?? true;
                                controller.checkSelectAllState();
                              },
                              text:
                                  'I have been explained about the consent as stated above and hereby provide my consent for the aforementioned purposes.',
                            )),
                      ],
                    ),
                  ),
                ),
                // Captcha Security Section
                const SizedBox(height: 8),
                Obx(() => Row(
                      children: [
                        // Captcha Box Display + Refresh Button
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 12, vertical: 6),
                          decoration: BoxDecoration(
                            color: const Color(0xFFE2E8F0),
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(color: const Color(0xFFCBD5E1)),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                controller.captchaCode.value,
                                style: TextStyle(
                                  fontFamily: 'monospace',
                                  fontSize: 17,
                                  fontWeight: FontWeight.bold,
                                  letterSpacing: 4.0,
                                  color: const Color(0xFF0F4C81),
                                  decoration: TextDecoration.lineThrough,
                                  decorationColor: const Color(0xFF64748B),
                                  decorationThickness: 2,
                                ),
                              ),
                              const SizedBox(width: 6),
                              IconButton(
                                constraints: const BoxConstraints(),
                                padding: EdgeInsets.zero,
                                onPressed: controller.isFetchingCaptcha.value
                                    ? null
                                    : () => controller.refreshCaptcha(),
                                icon: controller.isFetchingCaptcha.value
                                    ? const SizedBox(
                                        width: 14,
                                        height: 14,
                                        child: CircularProgressIndicator(
                                            strokeWidth: 2),
                                      )
                                    : const Icon(Icons.refresh,
                                        size: 18, color: Color(0xFF0F4C81)),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 12),

                        // Captcha Input Field
                        Expanded(
                          child: TextField(
                            onChanged: (v) => controller.captchaInput.value = v,
                            style: fontMedium.copyWith(
                                color: const Color(0xFF0F172A), fontSize: 14),
                            decoration: InputDecoration(
                              isDense: true,
                              labelText: 'Security Captcha',
                              labelStyle: fontMedium.copyWith(
                                  color: const Color(0xFF334155), fontSize: 13),
                              hintText: 'Enter Captcha',
                              hintStyle: fontRegular.copyWith(
                                  color: const Color(0xFF94A3B8), fontSize: 13),
                              filled: true,
                              fillColor: const Color(0xFFF8FAFC),
                              contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 12, vertical: 8),
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                                borderSide: const BorderSide(
                                    color: Color(0xFFCBD5E1), width: 1.5),
                              ),
                              enabledBorder: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                                borderSide: const BorderSide(
                                    color: Color(0xFFCBD5E1), width: 1.5),
                              ),
                              focusedBorder: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                                borderSide: const BorderSide(
                                    color: Color(0xFF0F4C81), width: 2.0),
                              ),
                            ),
                          ),
                        ),
                      ],
                    )),
                const SizedBox(height: 10),

                // Action Button: Generate OTP
                Obx(() => SizedBox(
                      width: double.infinity,
                      height: 44,
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: AppColor.primaryGradient,
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.transparent,
                            shadowColor: Colors.transparent,
                            shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(10)),
                          ),
                          onPressed: controller.isLoading.value
                              ? null
                              : () {
                                  String aadhaarNum =
                                      controller.inputNumber.value.trim();
                                  if (aadhaarNum.length < 12) {
                                    Get.snackbar('Validation Error',
                                        'Please enter a valid 12-digit Aadhaar Number across all 3 boxes.');
                                    return;
                                  }
                                  controller.sendOtp(aadhaarNum);
                                },
                          child: controller.isLoading.value
                              ? const CircularProgressIndicator(
                                  color: Colors.white)
                              : Text(
                                  'Generate OTP',
                                  style: fontMedium.copyWith(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold),
                                ),
                        ),
                      ),
                    )),
              ],
            ),
          ),
        ],
      ),
    );
  }

  // Reusable Symmetrical Checkbox Row
  Widget _buildSymmetricalCheckboxRow({
    required bool isChecked,
    required ValueChanged<bool?> onChanged,
    String? text,
    Widget? child,
  }) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 24,
          height: 24,
          child: Checkbox(
            value: isChecked,
            activeColor: const Color(0xFF0F4C81),
            checkColor: Colors.white,
            side: const BorderSide(color: Color(0xFF64748B), width: 1.8),
            shape:
                RoundedRectangleBorder(borderRadius: BorderRadius.circular(4)),
            materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
            onChanged: onChanged,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: child ??
              GestureDetector(
                onTap: () => onChanged(!isChecked),
                child: Text(
                  text ?? '',
                  style: fontSmall.copyWith(
                      fontSize: 12,
                      color: const Color(0xFF334155),
                      height: 1.45),
                ),
              ),
        ),
      ],
    );
  }

  void _showVerificationMethodDialog(BuildContext context) {
    Get.toNamed(Routes.M1_SEARCH_ABHA);
  }
}
