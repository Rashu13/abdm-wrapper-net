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

    // Helper to pick first non-empty string value across sources and keys
    String? pickVal(List<Map<String, dynamic>> sources, List<String> keys) {
      for (var source in sources) {
        for (var key in keys) {
          if (source.containsKey(key) && source[key] != null) {
            var val = source[key].toString().trim();
            if (val.isNotEmpty) {
              return val;
            }
          }
        }
      }
      return null;
    }

    var sources = [firstAccount, data, json];

    // Extract full name
    String? nameVal = pickVal(sources, ['name', 'Name']);
    String? fn = pickVal(sources, ['firstName', 'FirstName']);
    String? mn = pickVal(sources, ['middleName', 'MiddleName']);
    String? ln = pickVal(sources, ['lastName', 'LastName']);
    String fullName = nameVal ?? '${fn ?? ''} ${mn ?? ''} ${ln ?? ''}'.trim();
    if (fullName.isEmpty) fullName = 'ABHA Holder';

    // Extract existing ABHA addresses
    List<String> existingAddrs = [];
    if (data['mappedPhrAddress'] is List) {
      existingAddrs = List<String>.from(data['mappedPhrAddress']);
    } else if (data['accounts'] is List) {
      for (var acc in data['accounts']) {
        if (acc is Map && acc['abhaAddress'] != null && acc['abhaAddress'].toString().trim().isNotEmpty) {
          existingAddrs.add(acc['abhaAddress'].toString().trim());
        } else if (acc is Map && acc['preferredAbhaAddress'] != null && acc['preferredAbhaAddress'].toString().trim().isNotEmpty) {
          existingAddrs.add(acc['preferredAbhaAddress'].toString().trim());
        } else if (acc is String && acc.trim().isNotEmpty) {
          existingAddrs.add(acc.trim());
        }
      }
    }

    String? token = pickVal(sources, ['xToken', 'token', 'ABHAToken', 'userToken', 'UserToken']);
    String? abhaNum = pickVal(sources, ['healthIdNumber', 'abhaNumber', 'ABHANumber', 'HealthIdNumber', 'AbhaNumber']);
    String? abhaAddr = pickVal(sources, ['abhaAddress', 'healthId', 'phrAddress', 'preferredAbhaAddress', 'AbhaAddress', 'PreferredAbhaAddress']);
    String? gender = pickVal(sources, ['gender', 'Gender']) ?? 'N/A';
    String? dob = pickVal(sources, ['dob', 'dateOfBirth', 'dayOfBirth', 'Dob', 'DateOfBirth']);

    if (dob == null) {
      String? yob = pickVal(sources, ['yearOfBirth', 'YearOfBirth']);
      if (yob != null) {
        String? mob = pickVal(sources, ['monthOfBirth', 'MonthOfBirth']) ?? '01';
        String? dobDay = pickVal(sources, ['dayOfBirth', 'DayOfBirth']) ?? '01';
        dob = '$dobDay/$mob/$yob';
      }
    }

    String? mobile = pickVal(sources, ['mobile', 'phoneNumber', 'maskedMobile', 'Mobile', 'PhoneNumber', 'MaskedMobile']);
    String? email = pickVal(sources, ['email', 'Email']);
    String? photo = pickVal(sources, ['profilePhoto', 'photo', 'kycPhoto', 'ProfilePhoto', 'Photo', 'KycPhoto']);
    String? address = pickVal(sources, ['address', 'Address', 'fullAddress', 'FullAddress']);
    String? pincode = pickVal(sources, ['pincode', 'pinCode', 'Pincode', 'PinCode']);
    String? districtName = pickVal(sources, ['districtName', 'district', 'DistrictName', 'District']);
    String? stateName = pickVal(sources, ['stateName', 'state', 'StateName', 'State']);
    String? stateCode = pickVal(sources, ['stateCode', 'StateCode']);
    String? districtCode = pickVal(sources, ['districtCode', 'DistrictCode']);

    return AbhaProfileModel(
      abhaNumber: abhaNum,
      abhaAddress: abhaAddr,
      existingAbhaAddresses: existingAddrs,
      name: fullName,
      firstName: fn,
      middleName: mn,
      lastName: ln,
      gender: gender,
      dob: dob ?? 'N/A',
      yearOfBirth: pickVal(sources, ['yearOfBirth', 'YearOfBirth']),
      mobile: mobile ?? 'N/A',
      email: email,
      profilePhoto: photo,
      userToken: token,
      address: address,
      pincode: pincode,
      districtName: districtName,
      stateName: stateName,
      stateCode: stateCode,
      districtCode: districtCode,
    );
  }
}

