class AbdmGenerateOtpRequest {
  final String? loginId;
  final String? loginType;
  final String? aadhaar;
  final String? mobile;
  final String? consentTimestamp;
  final String? operatorName;
  final String? beneficiaryName;
  final bool? chk1;
  final bool? chk2;
  final bool? chk3;
  final bool? chk4;
  final bool? chk5;
  final bool? chk6;
  final bool? chk7;
  final String? captchaTxnId;
  final String? captchaValue;

  AbdmGenerateOtpRequest({
    this.loginId,
    this.loginType = "aadhaar",
    this.aadhaar,
    this.mobile,
    this.consentTimestamp,
    this.operatorName = "",
    this.beneficiaryName = "",
    this.chk1 = true,
    this.chk2 = true,
    this.chk3 = true,
    this.chk4 = true,
    this.chk5 = true,
    this.chk6 = true,
    this.chk7 = true,
    this.captchaTxnId,
    this.captchaValue,
  });

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = {};
    if (loginId != null) data['loginId'] = loginId;
    if (loginType != null) data['loginType'] = loginType;
    if (aadhaar != null) data['aadhaar'] = aadhaar;
    if (mobile != null) data['mobile'] = mobile;
    if (captchaTxnId != null) data['captchaTxnId'] = captchaTxnId;
    if (captchaValue != null) data['captchaValue'] = captchaValue;
    data['consentTimestamp'] = consentTimestamp ?? DateTime.now().toUtc().toIso8601String();
    data['operatorName'] = operatorName ?? "";
    data['beneficiaryName'] = beneficiaryName ?? "";
    data['chk1'] = chk1 ?? true;
    data['chk2'] = chk2 ?? true;
    data['chk3'] = chk3 ?? true;
    data['chk4'] = chk4 ?? true;
    data['chk5'] = chk5 ?? true;
    data['chk6'] = chk6 ?? true;
    data['chk7'] = chk7 ?? true;
    return data;
  }
}

class AbdmVerifyOtpRequest {
  final String otp;
  final String txnId;
  final String? loginType;
  final String? mobile;

  AbdmVerifyOtpRequest({
    required this.otp,
    required this.txnId,
    this.loginType = "aadhaar",
    this.mobile,
  });

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = {
      'otp': otp,
      'txnId': txnId,
      'transactionId': txnId,
      'loginType': loginType ?? 'aadhaar',
    };
    if (mobile != null && mobile!.isNotEmpty) {
      data['mobile'] = mobile;
    }
    return data;
  }
}
