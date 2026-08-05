class AbhaProfileModel {
  final String? abhaNumber;
  final String? abhaAddress;
  final List<String>? existingAbhaAddresses;
  final String? name;
  final String? firstName;
  final String? middleName;
  final String? lastName;
  final String? gender;
  final String? dob;
  final String? yearOfBirth;
  final String? mobile;
  final String? email;
  final String? profilePhoto;
  final String? userToken;

  // Address fields
  final String? address;
  final String? pincode;
  final String? districtName;
  final String? stateName;
  final String? stateCode;
  final String? districtCode;

  AbhaProfileModel({
    this.abhaNumber,
    this.abhaAddress,
    this.existingAbhaAddresses,
    this.name,
    this.firstName,
    this.middleName,
    this.lastName,
    this.gender,
    this.dob,
    this.yearOfBirth,
    this.mobile,
    this.email,
    this.profilePhoto,
    this.userToken,
    this.address,
    this.pincode,
    this.districtName,
    this.stateName,
    this.stateCode,
    this.districtCode,
  });

  factory AbhaProfileModel.fromJson(Map<String, dynamic> json) {
    // NHA response can wrap data inside 'data' key OR be flat
    Map<String, dynamic> data = json;
    if (json.containsKey('data') && json['data'] is Map<String, dynamic>) {
      data = json['data'] as Map<String, dynamic>;
    }

    Map<String, dynamic> firstAccount = {};
    if (data['accounts'] is List && (data['accounts'] as List).isNotEmpty) {
      var item = (data['accounts'] as List).first;
      if (item is Map<String, dynamic>) {
        firstAccount = item;
      }
    }

    // Extract full name
    String fullName = data['name'] ?? firstAccount['name'] ??
        '${data['firstName'] ?? firstAccount['firstName'] ?? ''} ${data['middleName'] ?? ''} ${data['lastName'] ?? firstAccount['lastName'] ?? ''}'.trim();
    if (fullName.isEmpty) fullName = 'ABHA Holder';

    // Extract existing ABHA addresses
    List<String> existingAddrs = [];
    if (data['mappedPhrAddress'] is List) {
      existingAddrs = List<String>.from(data['mappedPhrAddress']);
    } else if (data['accounts'] is List) {
      for (var acc in data['accounts']) {
        if (acc is Map && acc['abhaAddress'] != null) {
          existingAddrs.add(acc['abhaAddress'].toString());
        } else if (acc is Map && acc['preferredAbhaAddress'] != null) {
          existingAddrs.add(acc['preferredAbhaAddress'].toString());
        } else if (acc is String) {
          existingAddrs.add(acc);
        }
      }
    }

    // Token extraction — NHA ABDM returns xToken at different levels
    String? token = data['xToken'] ??
        data['token'] ??
        data['ABHAToken'] ??
        json['xToken'] ??
        json['token'] ??
        json['ABHAToken'];

    String? abhaNum = data['healthIdNumber'] ??
        data['abhaNumber'] ??
        data['ABHANumber'] ??
        firstAccount['ABHANumber'] ??
        firstAccount['healthIdNumber'] ??
        firstAccount['abhaNumber'];

    String? abhaAddr = data['abhaAddress'] ??
        data['healthId'] ??
        data['phrAddress'] ??
        data['preferredAbhaAddress'] ??
        firstAccount['preferredAbhaAddress'] ??
        firstAccount['abhaAddress'];

    String? gender = data['gender'] ?? firstAccount['gender'] ?? 'N/A';
    String? dob = data['dob'] ??
        data['dateOfBirth'] ??
        data['dayOfBirth'] ??
        firstAccount['dob'] ??
        firstAccount['dateOfBirth'];

    if (dob == null && (data['yearOfBirth'] != null || firstAccount['yearOfBirth'] != null)) {
      dob = '${data['dayOfBirth'] ?? '01'}/${data['monthOfBirth'] ?? '01'}/${data['yearOfBirth'] ?? firstAccount['yearOfBirth']}';
    }

    String? mobile = data['mobile'] ??
        data['phoneNumber'] ??
        data['maskedMobile'] ??
        firstAccount['mobile'] ??
        firstAccount['maskedMobile'];

    return AbhaProfileModel(
      abhaNumber: abhaNum,
      abhaAddress: abhaAddr,
      existingAbhaAddresses: existingAddrs,
      name: fullName,
      firstName: data['firstName'] ?? firstAccount['firstName'],
      middleName: data['middleName'],
      lastName: data['lastName'] ?? firstAccount['lastName'],
      gender: gender,
      dob: dob ?? 'N/A',
      yearOfBirth: data['yearOfBirth'] ?? firstAccount['yearOfBirth'],
      mobile: mobile ?? 'N/A',
      email: data['email'] ?? firstAccount['email'],
      profilePhoto: data['profilePhoto'] ?? data['photo'] ?? firstAccount['profilePhoto'] ?? firstAccount['photo'],
      userToken: token,
      address: data['address'] ?? firstAccount['address'],
      pincode: data['pincode'] ?? data['pinCode'] ?? firstAccount['pincode'],
      districtName: data['districtName'] ?? firstAccount['districtName'],
      stateName: data['stateName'] ?? firstAccount['stateName'],
      stateCode: data['stateCode'] ?? firstAccount['stateCode'],
      districtCode: data['districtCode'] ?? firstAccount['districtCode'],
    );
  }
}
