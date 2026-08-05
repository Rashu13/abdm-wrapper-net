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

    // Extract full name
    String fullName = data['name'] ??
        '${data['firstName'] ?? ''} ${data['middleName'] ?? ''} ${data['lastName'] ?? ''}'.trim();
    if (fullName.isEmpty) fullName = json['name'] ?? 'ABHA User';

    // Extract existing ABHA addresses
    List<String> existingAddrs = [];
    if (data['mappedPhrAddress'] is List) {
      existingAddrs = List<String>.from(data['mappedPhrAddress']);
    } else if (data['accounts'] is List) {
      for (var acc in data['accounts']) {
        if (acc is Map && acc['abhaAddress'] != null) {
          existingAddrs.add(acc['abhaAddress'].toString());
        } else if (acc is String) {
          existingAddrs.add(acc);
        }
      }
    }

    // Token extraction — NHA ABDM returns xToken at different levels
    // Priority: data.xToken > data.token > json.xToken > json.token > data.ABHAToken
    String? token = data['xToken'] ??
        data['token'] ??
        data['ABHAToken'] ??
        json['xToken'] ??
        json['token'] ??
        json['ABHAToken'];

    return AbhaProfileModel(
      abhaNumber: data['healthIdNumber'] ?? data['abhaNumber'] ?? data['ABHANumber'],
      abhaAddress: data['abhaAddress'] ?? data['healthId'] ?? data['phrAddress'],
      existingAbhaAddresses: existingAddrs,
      name: fullName,
      firstName: data['firstName'],
      middleName: data['middleName'],
      lastName: data['lastName'],
      gender: data['gender'] ?? 'M',
      dob: data['dob'] ?? data['dateOfBirth'] ?? data['dayOfBirth'],
      yearOfBirth: data['yearOfBirth'],
      mobile: data['mobile'] ?? data['phoneNumber'],
      email: data['email'],
      profilePhoto: data['profilePhoto'] ?? data['photo'],
      userToken: token,
      address: data['address'],
      pincode: data['pincode'] ?? data['pinCode'],
      districtName: data['districtName'],
      stateName: data['stateName'],
      stateCode: data['stateCode'],
      districtCode: data['districtCode'],
    );
  }
}
