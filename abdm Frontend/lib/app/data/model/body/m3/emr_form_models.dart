class MedicineFormItem {
  String drugName;
  String dosagePattern;
  String route;
  String method;
  String reason;
  String snomedCode;

  MedicineFormItem({
    this.drugName = '',
    this.dosagePattern = '1-0-1 (After Food)',
    this.route = 'Oral',
    this.method = 'After Food',
    this.reason = '',
    this.snomedCode = '',
  });

  Map<String, dynamic> toJson() => {
        'medicine': drugName,
        'name': drugName,
        'dosage': dosagePattern,
        'route': route,
        'method': method,
        'reason': reason,
        if (snomedCode.isNotEmpty) 'snomedCode': snomedCode,
      };
}

class LabResultFormItem {
  String testName;
  String value;
  String unit;

  LabResultFormItem({
    this.testName = '',
    this.value = '',
    this.unit = '',
  });

  Map<String, dynamic> toJson() => {
        'testName': testName,
        'value': value,
        'unit': unit,
      };
}

class VitalFormItem {
  String vitalName;
  String value;
  String unit;

  VitalFormItem({
    this.vitalName = '',
    this.value = '',
    this.unit = '',
  });

  Map<String, dynamic> toJson() => {
        'vitalName': vitalName,
        'value': value,
        'unit': unit,
      };
}

class AllergyFormItem {
  String allergyName;
  String type; // medication, food, environment, biologic
  String status; // active, inactive, resolved

  AllergyFormItem({
    this.allergyName = '',
    this.type = 'medication',
    this.status = 'active',
  });

  Map<String, dynamic> toJson() => {
        'allergyName': allergyName,
        'type': type,
        'status': status,
      };
}

class ImmunizationFormItem {
  String vaccineName;
  String lotNumber;
  String doseNumber;
  String date;

  ImmunizationFormItem({
    this.vaccineName = '',
    this.lotNumber = '',
    this.doseNumber = '1',
    this.date = '',
  });

  Map<String, dynamic> toJson() => {
        'vaccineName': vaccineName,
        'lotNumber': lotNumber,
        'doseNumber': doseNumber,
        'date': date,
      };
}
