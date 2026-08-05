import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:pinput/pinput.dart';
import '../controllers/abha_creation_controller.dart';
import '../widgets/aadhaar_segmented_input.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class SearchAbhaCardView extends GetView<AbhaCreationController> {
  const SearchAbhaCardView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24.0),
          child: Container(
            constraints: const BoxConstraints(maxWidth: 560),
            child: const SearchAbhaCardWidget(),
          ),
        ),
      ),
    );
  }
}

class SearchAbhaCardWidget extends StatefulWidget {
  const SearchAbhaCardWidget({Key? key}) : super(key: key);

  @override
  State<SearchAbhaCardWidget> createState() => _SearchAbhaCardWidgetState();
}

class _SearchAbhaCardWidgetState extends State<SearchAbhaCardWidget> {
  int _selectedTabIndex =
      0; // 0: Aadhaar Number, 1: Mobile Number, 2: ABHA Number, 3: ABHA Address
  String _authType = 'Aadhaar OTP';
  final TextEditingController _inputController = TextEditingController();
  final TextEditingController _otpController = TextEditingController();

  final controller = Get.find<AbhaCreationController>();

  @override
  void initState() {
    super.initState();
    if (controller.captchaCode.value.isEmpty) {
      controller.refreshCaptcha();
    }
  }

  @override
  void dispose() {
    _inputController.dispose();
    _otpController.dispose();
    super.dispose();
  }

  String get _currentLabel {
    switch (_selectedTabIndex) {
      case 0:
        return 'Aadhaar Number';
      case 1:
        return 'Mobile Number';
      case 2:
        return 'ABHA Number';
      case 3:
        return 'ABHA Address';
      default:
        return 'Aadhaar Number';
    }
  }

  String get _currentHint {
    switch (_selectedTabIndex) {
      case 0:
        return 'Enter Aadhaar Number';
      case 1:
        return 'Enter Mobile Number';
      case 2:
        return 'Enter ABHA Number (XX-xxxx-xxxx-xxxx)';
      case 3:
        return 'Enter ABHA Address';
      default:
        return 'Enter Aadhaar Number';
    }
  }

  @override
  Widget build(BuildContext context) {
    return Obx(() {
      final profile = controller.abhaProfile.value;
      if (controller.isCardCompleted.value && profile != null) {
        return _buildVerifiedCardDisplay(profile);
      }
      return Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.06),
                blurRadius: 20,
                offset: const Offset(0, 8),
              ),
            ],
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Header Bar with Back Button
              Container(
                width: double.infinity,
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                decoration: const BoxDecoration(
                  color: Color(0xFFF1F5F9),
                  borderRadius: BorderRadius.only(
                    topLeft: Radius.circular(20),
                    topRight: Radius.circular(20),
                  ),
                ),
                child: Row(
                  children: [
                    IconButton(
                      icon: const Icon(Icons.arrow_back,
                          color: Color(0xFF1E293B)),
                      onPressed: () {
                        controller.txnId.value = '';
                        Get.back();
                      },
                    ),
                    const SizedBox(width: 4),
                    Text(
                      'Search / Download ABHA Card',
                      style: fontBold.copyWith(
                        fontSize: 18,
                        color: const Color(0xFF1E293B),
                      ),
                    ),
                  ],
                ),
              ),

              Padding(
                padding: const EdgeInsets.all(24.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Subtitle
                    Text(
                      'Search your ABHA card, using below options',
                      style: fontRegular.copyWith(
                        fontSize: 14,
                        color: const Color(0xFF64748B),
                      ),
                    ),
                    const SizedBox(height: 20),

                    // Segmented Tab Bar (Pill selector)
                    Container(
                      padding: const EdgeInsets.all(4),
                      decoration: BoxDecoration(
                        color: const Color(0xFFF1F5F9),
                        borderRadius: BorderRadius.circular(24),
                      ),
                      child: Row(
                        children: [
                          _buildTabPill('Aadhaar Number', 0),
                          _buildTabPill('Mobile Number', 1),
                          _buildTabPill('ABHA Number', 2),
                          _buildTabPill('ABHA Address', 3),
                        ],
                      ),
                    ),
                    const SizedBox(height: 24),

                    // Input Field Section
                    if (_selectedTabIndex == 0) ...[
                      Text(
                        'Enter 12-digit Aadhaar Number',
                        style: fontMedium.copyWith(
                          color: const Color(0xFF334155),
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 8),
                      AadhaarSegmentedInput(controller: controller),
                    ] else ...[
                      TextFormField(
                        controller: _inputController,
                        keyboardType: _selectedTabIndex == 1
                            ? TextInputType.number
                            : TextInputType.text,
                        maxLength: _selectedTabIndex == 1 ? 10 : 30,
                        style: fontMedium.copyWith(
                          fontSize: 15,
                          color: const Color(0xFF0F172A),
                        ),
                        decoration: InputDecoration(
                          labelText: _currentLabel,
                          labelStyle: fontMedium.copyWith(
                            color: const Color(0xFF64748B),
                            fontSize: 13,
                          ),
                          hintText: _currentHint,
                          hintStyle: fontRegular.copyWith(
                            color: const Color(0xFF94A3B8),
                            fontSize: 13,
                          ),
                          counterText: '',
                          contentPadding: const EdgeInsets.symmetric(
                              horizontal: 16, vertical: 16),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(10),
                            borderSide:
                                const BorderSide(color: Color(0xFFCBD5E1)),
                          ),
                          focusedBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(10),
                            borderSide: const BorderSide(
                                color: Color(0xFF1E3E5B), width: 2),
                          ),
                        ),
                      ),
                    ],

                    // Authentication Type for ABHA Number & ABHA Address Tabs
                    if (_selectedTabIndex == 2 || _selectedTabIndex == 3) ...[
                      const SizedBox(height: 20),
                      Text(
                        'Authentication Type',
                        style: fontMedium.copyWith(
                          fontSize: 14,
                          color: const Color(0xFF64748B),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 10),
                      Row(
                        children: [
                          Row(
                            children: [
                              Radio<String>(
                                value: 'Aadhaar OTP',
                                groupValue: _authType,
                                activeColor: const Color(0xFF1E3E5B),
                                materialTapTargetSize:
                                    MaterialTapTargetSize.shrinkWrap,
                                onChanged: (val) {
                                  if (val != null)
                                    setState(() => _authType = val);
                                },
                              ),
                              const SizedBox(width: 4),
                              GestureDetector(
                                onTap: () =>
                                    setState(() => _authType = 'Aadhaar OTP'),
                                child: Text(
                                  'Aadhaar OTP',
                                  style: fontMedium.copyWith(
                                      fontSize: 14,
                                      color: const Color(0xFF0F172A)),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(width: 32),
                          Row(
                            children: [
                              Radio<String>(
                                value: 'ABHA OTP',
                                groupValue: _authType,
                                activeColor: const Color(0xFF1E3E5B),
                                materialTapTargetSize:
                                    MaterialTapTargetSize.shrinkWrap,
                                onChanged: (val) {
                                  if (val != null)
                                    setState(() => _authType = val);
                                },
                              ),
                              const SizedBox(width: 4),
                              GestureDetector(
                                onTap: () =>
                                    setState(() => _authType = 'ABHA OTP'),
                                child: Text(
                                  'ABHA OTP',
                                  style: fontMedium.copyWith(
                                      fontSize: 14,
                                      color: const Color(0xFF0F172A)),
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ],

                    // Captcha Security Section (Only show if OTP not yet sent)
                    Obx(() {
                      if (controller.txnId.value.isNotEmpty ||
                          _selectedTabIndex != 0) {
                        return const SizedBox.shrink();
                      }
                      return Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const SizedBox(height: 20),
                          Row(
                            children: [
                              // Captcha Box Display + Refresh Button
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 6),
                                decoration: BoxDecoration(
                                  color: const Color(0xFFE2E8F0),
                                  borderRadius: BorderRadius.circular(10),
                                  border: Border.all(
                                      color: const Color(0xFFCBD5E1)),
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Text(
                                      controller.captchaCode.value,
                                      style: const TextStyle(
                                        fontFamily: 'monospace',
                                        fontSize: 17,
                                        fontWeight: FontWeight.bold,
                                        letterSpacing: 4.0,
                                        color: Color(0xFF0F4C81),
                                        decoration: TextDecoration.lineThrough,
                                        decorationColor: Color(0xFF64748B),
                                        decorationThickness: 2,
                                      ),
                                    ),
                                    const SizedBox(width: 6),
                                    IconButton(
                                      constraints: const BoxConstraints(),
                                      padding: EdgeInsets.zero,
                                      onPressed: controller
                                              .isFetchingCaptcha.value
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
                                              size: 18,
                                              color: Color(0xFF0F4C81)),
                                    ),
                                  ],
                                ),
                              ),
                              const SizedBox(width: 12),

                              // Captcha Input Field
                              Expanded(
                                child: TextField(
                                  onChanged: (v) =>
                                      controller.captchaInput.value = v,
                                  style: fontMedium.copyWith(
                                      color: const Color(0xFF0F172A),
                                      fontSize: 14),
                                  decoration: InputDecoration(
                                    isDense: true,
                                    labelText: 'Security Captcha',
                                    labelStyle: fontMedium.copyWith(
                                        color: const Color(0xFF334155),
                                        fontSize: 13),
                                    hintText: 'Enter Captcha',
                                    hintStyle: fontRegular.copyWith(
                                        color: const Color(0xFF94A3B8),
                                        fontSize: 13),
                                    filled: true,
                                    fillColor: const Color(0xFFF8FAFC),
                                    contentPadding: const EdgeInsets.symmetric(
                                        horizontal: 12, vertical: 12),
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
                                          color: Color(0xFF1E3E5B), width: 2.0),
                                    ),
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ],
                      );
                    }),

                    const SizedBox(height: 24),

                    // Dynamic Action Area: Send OTP Button OR OTP Verification Section
                    Obx(() {
                      final hasOtpBeenSent = controller.txnId.value.isNotEmpty;

                      if (!hasOtpBeenSent) {
                        // Send OTP Button
                        return SizedBox(
                          width: double.infinity,
                          height: 52,
                          child: ElevatedButton(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF1E3E5B),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(10),
                              ),
                              elevation: 2,
                            ),
                            onPressed: controller.isLoading.value
                                ? null
                                : () {
                                    String inputVal = _selectedTabIndex == 0
                                        ? controller.inputNumber.value.trim()
                                        : _inputController.text.trim();

                                    if (inputVal.isEmpty) {
                                      Get.snackbar('Input Required',
                                          'Please enter $_currentLabel');
                                      return;
                                    }
                                    if (_selectedTabIndex == 0 &&
                                        inputVal
                                                .replaceAll(
                                                    RegExp(r'[^0-9]'), '')
                                                .length <
                                            12) {
                                      Get.snackbar('Invalid Aadhaar',
                                          'Aadhaar Number must be exactly 12 digits');
                                      return;
                                    }
                                    if (_selectedTabIndex == 1 &&
                                        inputVal
                                                .replaceAll(
                                                    RegExp(r'[^0-9]'), '')
                                                .length <
                                            10) {
                                      Get.snackbar('Invalid Mobile',
                                          'Mobile Number must be exactly 10 digits');
                                      return;
                                    }
                                    String loginTypeParam = 'mobile';
                                    if (_selectedTabIndex == 0) {
                                      loginTypeParam = 'aadhaar';
                                    } else if (_selectedTabIndex == 1) {
                                      loginTypeParam = 'mobile';
                                    } else if (_selectedTabIndex == 2) {
                                      loginTypeParam =
                                          _authType == 'Aadhaar OTP'
                                              ? 'aadhaar'
                                              : 'abha-number';
                                    } else if (_selectedTabIndex == 3) {
                                      loginTypeParam =
                                          _authType == 'Aadhaar OTP'
                                              ? 'aadhaar'
                                              : 'abha-address';
                                    }

                                    controller.inputNumber.value = inputVal;
                                    controller.selectedLoginType.value =
                                        loginTypeParam;
                                    controller.setMode(false);
                                    controller.sendOtp(inputVal,
                                        loginType: loginTypeParam);
                                  },
                            child: controller.isLoading.value
                                ? const CircularProgressIndicator(
                                    color: Colors.white,
                                    strokeWidth: 2.5,
                                  )
                                : Text(
                                    'Send OTP',
                                    style: fontBold.copyWith(
                                      color: Colors.white,
                                      fontSize: 16,
                                    ),
                                  ),
                          ),
                        );
                      } else {
                        // Send OTP button disappears! OTP Verification Section appears below:
                        String bannerText;
                        if (controller.maskedMobile.value.isNotEmpty) {
                          String mobileStr = controller.maskedMobile.value;
                          if (mobileStr.startsWith('******') ||
                              mobileStr.contains('*')) {
                            bannerText =
                                'OTP sent to Aadhaar registered mobile number $mobileStr. Enter the OTP below to proceed.';
                          } else {
                            bannerText =
                                'OTP sent to Aadhaar registered mobile number ending with ****** $mobileStr. Enter the OTP below to proceed.';
                          }
                        } else if (controller
                                .communicationMobile.value.length >=
                            4) {
                          String last4 = controller.communicationMobile.value
                              .substring(
                                  controller.communicationMobile.value.length -
                                      4);
                          bannerText =
                              'OTP sent to Aadhaar registered mobile number ending with ****** $last4. Enter the OTP below to proceed.';
                        } else if (controller.inputNumber.value.length == 10) {
                          String last4 =
                              controller.inputNumber.value.substring(6);
                          bannerText =
                              'OTP sent to mobile number ending with ****** $last4. Enter the OTP below to proceed.';
                        } else {
                          bannerText =
                              'OTP sent to Aadhaar registered mobile number. Enter the OTP below to proceed.';
                        }

                        final defaultPinTheme = PinTheme(
                          width: 50,
                          height: 54,
                          textStyle: fontBold.copyWith(
                              fontSize: 20, color: const Color(0xFF0F4C81)),
                          decoration: BoxDecoration(
                            color: const Color(0xFFF8FAFC),
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(color: const Color(0xFFCBD5E1)),
                          ),
                        );

                        return Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            // Information Banner
                            Container(
                              width: double.infinity,
                              padding: const EdgeInsets.all(14),
                              decoration: BoxDecoration(
                                color: const Color(0xFFEFF6FF),
                                borderRadius: BorderRadius.circular(10),
                                border: Border.all(
                                    color: const Color(0xFF3B82F6)
                                        .withOpacity(0.3)),
                              ),
                              child: Row(
                                children: [
                                  const Icon(Icons.info_outline,
                                      color: Color(0xFF2563EB), size: 20),
                                  const SizedBox(width: 10),
                                  Expanded(
                                    child: Text(
                                      bannerText,
                                      style: fontMedium.copyWith(
                                        fontSize: 13,
                                        color: const Color(0xFF1E3E5B),
                                        height: 1.4,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            const SizedBox(height: 24),

                            // Section Title: OTP Verification
                            Text(
                              'OTP Verification',
                              style: fontBold.copyWith(
                                fontSize: 17,
                                color: const Color(0xFF0F172A),
                              ),
                            ),
                            const SizedBox(height: 14),

                            // Subtitle: Enter OTP
                            Text(
                              'Enter OTP',
                              style: fontMedium.copyWith(
                                fontSize: 13.5,
                                color: const Color(0xFF64748B),
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                            const SizedBox(height: 10),

                            // Pinput OTP Input Field
                            Center(
                              child: Pinput(
                                controller: _otpController,
                                length: 6,
                                defaultPinTheme: defaultPinTheme,
                                focusedPinTheme: defaultPinTheme.copyWith(
                                  decoration:
                                      defaultPinTheme.decoration!.copyWith(
                                    border: Border.all(
                                        color: const Color(0xFF0F4C81),
                                        width: 2),
                                  ),
                                ),
                              ),
                            ),
                            const SizedBox(height: 18),

                            // Resend OTP Row & Attempts Info
                            Obx(() {
                              final isTimerActive =
                                  controller.resendSeconds.value > 0;
                              final secFormatted = controller
                                  .resendSeconds.value
                                  .toString()
                                  .padLeft(2, '0');
                              final remainingAttempts =
                                  2 - controller.resendCount.value;

                              return Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      TextButton.icon(
                                        onPressed: controller.canResend.value &&
                                                !controller.isLoading.value
                                            ? () => controller.handleResendOtp()
                                            : null,
                                        style: TextButton.styleFrom(
                                          padding: EdgeInsets.zero,
                                          minimumSize: Size.zero,
                                          tapTargetSize:
                                              MaterialTapTargetSize.shrinkWrap,
                                        ),
                                        icon: Icon(
                                          Icons.refresh,
                                          size: 16,
                                          color: controller.canResend.value
                                              ? const Color(0xFF0F4C81)
                                              : Colors.grey,
                                        ),
                                        label: Text(
                                          isTimerActive
                                              ? 'Resend OTP (00:$secFormatted)'
                                              : 'Resend OTP',
                                          style: fontBold.copyWith(
                                            fontSize: 13,
                                            color: controller.canResend.value
                                                ? const Color(0xFF0F4C81)
                                                : Colors.grey,
                                          ),
                                        ),
                                      ),
                                      if (remainingAttempts >= 0)
                                        Text(
                                          'You have $remainingAttempts attempts remaining to resend OTP.',
                                          style: fontRegular.copyWith(
                                            fontSize: 12,
                                            color: const Color(0xFF64748B),
                                          ),
                                        ),
                                    ],
                                  ),
                                ],
                              );
                            }),
                            const SizedBox(height: 24),

                            // Verify Button
                            SizedBox(
                              width: double.infinity,
                              height: 52,
                              child: ElevatedButton(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: const Color(0xFF0F4C81),
                                  shape: RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(10),
                                  ),
                                  elevation: 2,
                                ),
                                onPressed: controller.isLoading.value
                                    ? null
                                    : () {
                                        String otpText =
                                            _otpController.text.trim();
                                        if (otpText.length < 4) {
                                          Get.snackbar('Input Error',
                                              'Please enter a valid OTP.');
                                          return;
                                        }
                                        controller.verifyOtp(otpText);
                                      },
                                child: controller.isLoading.value
                                    ? const CircularProgressIndicator(
                                        color: Colors.white,
                                        strokeWidth: 2.5,
                                      )
                                    : Text(
                                        'Verify',
                                        style: fontBold.copyWith(
                                          color: Colors.white,
                                          fontSize: 16,
                                        ),
                                      ),
                              ),
                            ),
                          ],
                        );
                      }
                    }),
                  ],
                ),
              ),
            ],
          ));
    });
  }

  Widget _buildVerifiedCardDisplay(dynamic profile) {
    String name = profile.name ?? 'ABHA Holder';
    String abhaNum = profile.abhaNumber ?? 'N/A';
    String abhaAddr = profile.abhaAddress ?? 'N/A';
    String dob = profile.dob ?? 'N/A';
    String gender = profile.gender ?? 'N/A';
    String mobile = profile.mobile ?? 'N/A';
    String address = profile.address ?? 'N/A';
    String district = profile.districtName ?? 'N/A';
    String state = profile.stateName ?? 'N/A';
    String pincode = profile.pincode ?? 'N/A';

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.06),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('ABHA Card Details',
                  style: fontBold.copyWith(
                      fontSize: 18, color: const Color(0xFF1E293B))),
              TextButton.icon(
                onPressed: () {
                  controller.resetLoginState();
                  controller.isCardCompleted.value = false;
                },
                icon: const Icon(Icons.refresh, size: 16),
                label: const Text('Verify Another'),
              ),
            ],
          ),
          const SizedBox(height: 16),
          // ABHA Card Banner
          Container(
            decoration: BoxDecoration(
              color: const Color(0xFF1E3A5F),
              borderRadius: BorderRadius.circular(16),
            ),
            padding: const EdgeInsets.all(18),
            child: Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(name,
                          style: fontBold.copyWith(
                              color: Colors.white, fontSize: 17)),
                      const SizedBox(height: 4),
                      Text('ABHA: $abhaNum',
                          style: fontBold.copyWith(
                              color: const Color(0xFF38BDF8), fontSize: 13)),
                      const SizedBox(height: 2),
                      Text('$dob | $abhaAddr',
                          style: fontRegular.copyWith(
                              color: Colors.white70, fontSize: 11)),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 14),
          // Details Grid
          Container(
            decoration: BoxDecoration(
              color: const Color(0xFFF8FAFC),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: const Color(0xFFE2E8F0)),
            ),
            padding: const EdgeInsets.all(14),
            child: Column(
              children: [
                _infoItem('Gender', gender),
                _infoItem('Mobile', mobile),
                _infoItem('Address', address),
                _infoItem('District/State', '$district, $state ($pincode)'),
              ],
            ),
          ),
          const SizedBox(height: 18),
          // Download Action
          Obx(() => SizedBox(
                width: double.infinity,
                height: 48,
                child: ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFFF5A623),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)),
                  ),
                  onPressed: controller.isDownloadingCard.value
                      ? null
                      : () => controller.downloadAbhaCard(),
                  icon: const Icon(Icons.file_download_outlined,
                      color: Colors.white),
                  label: Text(
                    controller.isDownloadingCard.value
                        ? 'Downloading...'
                        : 'Download ABHA Card',
                    style: fontBold.copyWith(color: Colors.white, fontSize: 15),
                  ),
                ),
              )),
          const SizedBox(height: 12),

          // Register Patient in HIP Database Action
          Obx(() => SizedBox(
                width: double.infinity,
                height: 48,
                child: ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF10B981),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(10)),
                  ),
                  onPressed: controller.isRegisteringDb.value
                      ? null
                      : () => controller.registerPatientInDatabase(),
                  icon: controller.isRegisteringDb.value
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                        )
                      : const Icon(Icons.how_to_reg, color: Colors.white, size: 20),
                  label: Text(
                    controller.isRegisteringDb.value
                        ? 'Registering in DB...'
                        : 'REGISTER PATIENT IN DATABASE',
                    style: fontBold.copyWith(color: Colors.white, fontSize: 14),
                  ),
                ),
              )),
        ],
      ),
    );
  }

  Widget _infoItem(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          SizedBox(
              width: 100,
              child: Text(label,
                  style: fontRegular.copyWith(
                      color: const Color(0xFF64748B), fontSize: 12))),
          Expanded(
              child: Text(value,
                  style: fontBold.copyWith(
                      color: const Color(0xFF0F172A), fontSize: 12))),
        ],
      ),
    );
  }

  Widget _buildTabPill(String title, int index) {
    bool isSelected = _selectedTabIndex == index;
    return Expanded(
      child: GestureDetector(
        onTap: () {
          setState(() {
            _selectedTabIndex = index;
            _inputController.clear();
            _otpController.clear();
            controller.txnId.value = '';
            if (index == 0) {
              controller.inputNumber.value = '';
            }
          });
        },
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          padding: const EdgeInsets.symmetric(vertical: 10),
          decoration: BoxDecoration(
            color: isSelected ? const Color(0xFF1E3E5B) : Colors.transparent,
            borderRadius: BorderRadius.circular(20),
          ),
          child: Center(
            child: Text(
              title,
              textAlign: TextAlign.center,
              style: fontMedium.copyWith(
                fontSize: 12,
                color: isSelected ? Colors.white : const Color(0xFF64748B),
                fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
              ),
            ),
          ),
        ),
      ),
    );
  }
}
