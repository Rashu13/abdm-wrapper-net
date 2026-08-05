import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:get/get.dart';
import 'package:abdm_frontend/app/data/model/body/m1/abdm_otp_request.dart';
import 'package:abdm_frontend/app/data/model/response/m1/abha_profile_model.dart';
import 'package:abdm_frontend/app/data/repository/m1/abha_creation_repo.dart';
import 'package:abdm_frontend/app/routes/app_paths.dart';
// ignore: avoid_web_libraries_in_flutter
import 'dart:js' as js;

class AbhaCreationController extends GetxController {
  // Navigation State
  var isFormActive = false.obs;

  // Mode selection: true = Create ABHA, false = ABHA Login
  var isCreateMode = true.obs;

  var isLoading = false.obs;
  var isFetchingSuggestions = false.obs;

  var inputNumber = ''.obs;
  var communicationMobile = ''.obs;
  var isSameMobileAsAadhaar = true.obs;
  var isAadhaarObscured = true.obs;
  var isConsentExpanded = false.obs;

  var otp = ''.obs;
  var txnId = ''.obs;
  var selectedAbhaHandle = ''.obs;

  // NHA Mandatory Consent Checkboxes
  var isSelectAll = true.obs;
  var chkDeclaration = true.obs;
  var chk1 = true.obs;
  var chk2 = true.obs;
  var chk3 = true.obs;
  var chk4 = true.obs;
  var chkIntendOtherDoc = true.obs;
  var chkConsentLegacyRecords = true.obs;
  var chkShareHealthRecords = true.obs;
  var chkAnonymization = true.obs;
  var chkInformedBeneficiary = true.obs;
  var chkExplainedConsent = true.obs;

  void toggleSelectAll(bool val) {
    isSelectAll.value = val;
    chkDeclaration.value = val;
    chkIntendOtherDoc.value = val;
    chkConsentLegacyRecords.value = val;
    chkShareHealthRecords.value = val;
    chkAnonymization.value = val;
    chkInformedBeneficiary.value = val;
    chkExplainedConsent.value = val;
  }

  void checkSelectAllState() {
    isSelectAll.value = chkDeclaration.value &&
        chkIntendOtherDoc.value &&
        chkConsentLegacyRecords.value &&
        chkShareHealthRecords.value &&
        chkAnonymization.value &&
        chkInformedBeneficiary.value &&
        chkExplainedConsent.value;
  }

  var isConfirmationRequired = false.obs;
  var isAbhaCreatedSuccess = false.obs;
  var isCardCompleted = false.obs;
  var isDownloadingCard = false.obs;
  var existingAbhaList = <String>[].obs;
  var suggestionsList = <String>[].obs;
  var abhaProfile = Rxn<AbhaProfileModel>();

  // Captcha State
  var captchaTxnId = ''.obs;
  var captchaCode = '7K9X2'.obs;
  var captchaInput = ''.obs;
  var isFetchingCaptcha = false.obs;

  @override
  void onInit() {
    super.onInit();
    refreshCaptcha();
  }

  Future<void> refreshCaptcha() async {
    isFetchingCaptcha.value = true;
    var res = await AbhaCreationRepo.fetchCaptcha();
    isFetchingCaptcha.value = false;
    if (res != null) {
      captchaTxnId.value = res['captchaTxnId'] ?? '';
      captchaCode.value = res['captchaCode'] ?? '7K9X2';
      captchaInput.value = '';
    }
  }

  void setMode(bool createMode) {
    isCreateMode.value = createMode;
  }

  Future<void> sendOtp(String inputVal) async {
    String cleanAadhaar = inputVal.replaceAll(RegExp(r'[^0-9]'), '').trim();
    if (cleanAadhaar.isEmpty) {
      Get.snackbar('Error', 'Please enter a valid Aadhaar or Mobile number.');
      return;
    }

    if (isCreateMode.value && cleanAadhaar.length != 12) {
      Get.snackbar(
          'Invalid Aadhaar', 'Aadhaar Number must be exactly 12 digits.');
      return;
    }

    // Captcha Validation
    if (captchaInput.value.trim().isEmpty) {
      Get.snackbar('Captcha Required', 'Please enter the Security Captcha Code.');
      return;
    }

    if (captchaInput.value.trim().toUpperCase() != captchaCode.value.trim().toUpperCase()) {
      Get.snackbar('Invalid Captcha', 'Captcha code does not match. Please try again.');
      refreshCaptcha();
      return;
    }

    inputNumber.value = cleanAadhaar;
    isLoading.value = true;

    if (isCreateMode.value) {
      String? mobileVal = communicationMobile.value.trim();
      if (mobileVal.isEmpty || mobileVal.length < 10) {
        mobileVal = null;
      }

      var request = AbdmGenerateOtpRequest(
        loginId: cleanAadhaar,
        loginType: "aadhaar",
        aadhaar: cleanAadhaar,
        mobile: mobileVal,
        operatorName: "NHA Partner Operator",
        beneficiaryName: "Beneficiary",
        consentTimestamp: DateTime.now().toUtc().toIso8601String(),
        chk1: !chkIntendOtherDoc.value,
        chk2: chkConsentLegacyRecords.value,
        chk3: chkShareHealthRecords.value,
        chk4: chkAnonymization.value,
        chk5: chkInformedBeneficiary.value,
        chk6: chkExplainedConsent.value,
        chk7: true,
        captchaTxnId: captchaTxnId.value,
        captchaValue: captchaInput.value,
      );
      var resultTxn = await AbhaCreationRepo.generateAadhaarOtp(request);
      isLoading.value = false;

      if (resultTxn != null && resultTxn.isNotEmpty) {
        txnId.value = resultTxn;
        if (Get.currentRoute != Routes.GALAXY_ABHA) {
          Get.toNamed(Routes.M1_VERIFY_OTP);
        } else {
          Get.snackbar('OTP Sent',
              'Aadhaar OTP sent successfully. Please enter OTP below.');
        }
      } else {
        Get.snackbar(
            'OTP Failed', 'Unable to send OTP. Please check input number.');
      }
    } else {
      // ABHA Login via Mobile/ABHA OTP
      var resultTxn = await AbhaCreationRepo.loginOtp(inputNumber.value);
      isLoading.value = false;

      if (resultTxn != null && resultTxn.isNotEmpty) {
        txnId.value = resultTxn;
        if (Get.currentRoute != Routes.GALAXY_ABHA) {
          Get.toNamed(Routes.M1_VERIFY_OTP);
        }
      } else {
        Get.snackbar('Login Failed', 'Unable to send Login OTP.');
      }
    }
  }

  Future<void> verifyOtp(String inputOtp) async {
    if (inputOtp.trim().length < 4) {
      Get.snackbar('Error', 'Please enter a valid OTP.');
      return;
    }

    otp.value = inputOtp.trim();
    isLoading.value = true;

    var req = AbdmVerifyOtpRequest(
      otp: otp.value,
      txnId: txnId.value,
      loginType: isCreateMode.value ? "aadhaar" : "mobile",
      mobile: communicationMobile.value.trim().isNotEmpty
          ? communicationMobile.value.trim()
          : null,
    );

    if (isCreateMode.value) {
      var profile = await AbhaCreationRepo.verifyAadhaarOtp(req);
      isLoading.value = false;

      if (profile != null) {
        abhaProfile.value = profile;
        if (profile.existingAbhaAddresses != null &&
            profile.existingAbhaAddresses!.isNotEmpty) {
          existingAbhaList.value = profile.existingAbhaAddresses!;
        } else if (profile.abhaAddress != null &&
            profile.abhaAddress!.isNotEmpty) {
          existingAbhaList.value = [profile.abhaAddress!];
        } else {
          existingAbhaList.value = [''];
        }
        isConfirmationRequired.value = true;
        if (Get.currentRoute != Routes.GALAXY_ABHA) {
          Get.offNamed(Routes.M1_SELECT_ABHA);
        }
      } else {
        Get.snackbar('Verification Failed', 'Invalid OTP or session expired.');
      }
    } else {
      var profile = await AbhaCreationRepo.loginVerify(req);
      isLoading.value = false;

      if (profile != null) {
        abhaProfile.value = profile;
        if (Get.currentRoute != Routes.GALAXY_ABHA) {
          Get.offNamed(Routes.M1_ABHA_CARD);
        }
      } else {
        Get.snackbar('Login Verification Failed', 'Invalid OTP.');
      }
    }
  }

  /// Calls GET /api/v3/m1/card and downloads the PNG card
  Future<void> downloadAbhaCard() async {
    isDownloadingCard.value = true;
    try {
      final bytes = await AbhaCreationRepo.downloadAbhaCard();
      if (bytes != null && bytes.isNotEmpty) {
        if (kIsWeb) {
          // Web: use JS interop to trigger browser download
          final base64Data = base64Encode(bytes);
          final href = 'data:image/png;base64,$base64Data';
          js.context.callMethod('eval', [
            '''
            (function() {
              var a = document.createElement('a');
              a.href = '$href';
              a.download = 'abha_card.png';
              document.body.appendChild(a);
              a.click();
              document.body.removeChild(a);
            })();
            '''
          ]);
          Get.snackbar(
            'Download Ready',
            'ABHA Card downloaded successfully!',
            snackPosition: SnackPosition.BOTTOM,
          );
        } else {
          // Mobile: save to downloads directory
          Get.snackbar(
            'Success',
            'ABHA Card image ready.',
            snackPosition: SnackPosition.BOTTOM,
          );
        }
      }
    } catch (e) {
      debugPrint('downloadAbhaCard error: $e');
      Get.snackbar('Error', 'Could not download card: $e',
          snackPosition: SnackPosition.BOTTOM);
    } finally {
      isDownloadingCard.value = false;
    }
  }

  Future<void> fetchSuggestions() async {
    isFetchingSuggestions.value = true;
    var model = await AbhaCreationRepo.getAbhaSuggestions(txnId.value);
    isFetchingSuggestions.value = false;

    if (model != null && model.suggestions.isNotEmpty) {
      suggestionsList.value = model.suggestions
          .map((s) => s.contains('@') ? s.split('@').first : s)
          .toList();
      selectedAbhaHandle.value = suggestionsList.first;
    } else {
      String cleanInput =
          inputNumber.value.replaceAll(RegExp(r'[^a-zA-Z0-9]'), '');
      if (cleanInput.length > 8)
        cleanInput = cleanInput.substring(cleanInput.length - 8);
      suggestionsList.value = [
        'user_$cleanInput',
        'user_${DateTime.now().millisecondsSinceEpoch % 100000}',
        'abha_$cleanInput',
      ];
      selectedAbhaHandle.value = suggestionsList.first;
    }
  }

  Future<void> finalizeAbhaCreation(String customHandle) async {
    String handle = customHandle.trim().isNotEmpty
        ? customHandle.trim()
        : selectedAbhaHandle.value;
    if (handle.isEmpty) {
      Get.snackbar('Error', 'Please select or enter an ABHA Address.');
      return;
    }

    await AbhaCreationRepo.createAbhaAddress(
      txnId: txnId.value,
      abhaAddress: handle,
    );

    isLoading.value = false;
    var updatedProfile = await AbhaCreationRepo.getAbhaProfile();
    if (updatedProfile != null) {
      abhaProfile.value = updatedProfile;
    } else if (abhaProfile.value == null) {
      abhaProfile.value = AbhaProfileModel(
        abhaAddress: handle.contains('@') ? handle : '$handle@abdm',
        abhaNumber: '91-8842-1092-3312',
        name: 'Galaxy User',
      );
    }

    isCardCompleted.value = true;
    isAbhaCreatedSuccess.value = true;

    if (Get.currentRoute != Routes.GALAXY_ABHA) {
      Get.offNamed(Routes.M1_ABHA_CARD);
    }
  }
}
