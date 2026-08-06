import 'package:flutter/material.dart';

class MedicineFormItem {
  String dosagePattern;
  String route;
  String method;

  late final TextEditingController drugNameCtrl;
  late final TextEditingController reasonCtrl;
  late final TextEditingController snomedCodeCtrl;

  MedicineFormItem({
    String drugName = '',
    this.dosagePattern = 'Morning & Night (1-0-1)',
    this.route = 'Oral',
    this.method = 'After Food',
    String reason = '',
    String snomedCode = '',
  }) {
    drugNameCtrl = TextEditingController(text: drugName);
    reasonCtrl = TextEditingController(text: reason);
    snomedCodeCtrl = TextEditingController(text: snomedCode);
  }

  String get drugName => drugNameCtrl.text;
  set drugName(String val) => drugNameCtrl.text = val;

  String get reason => reasonCtrl.text;
  set reason(String val) => reasonCtrl.text = val;

  String get snomedCode => snomedCodeCtrl.text;
  set snomedCode(String val) => snomedCodeCtrl.text = val;

  Map<String, dynamic> toJson() => {
        'medicine': drugNameCtrl.text,
        'name': drugNameCtrl.text,
        'dosage': dosagePattern,
        'route': route,
        'method': method,
        'reason': reasonCtrl.text,
        if (snomedCodeCtrl.text.isNotEmpty) 'snomedCode': snomedCodeCtrl.text,
      };

  void dispose() {
    drugNameCtrl.dispose();
    reasonCtrl.dispose();
    snomedCodeCtrl.dispose();
  }
}

class LabResultFormItem {
  late final TextEditingController testNameCtrl;
  late final TextEditingController valueCtrl;
  late final TextEditingController unitCtrl;
  late final TextEditingController snomedCodeCtrl;

  LabResultFormItem({
    String testName = '',
    String value = '',
    String unit = '',
    String snomedCode = '',
  }) {
    testNameCtrl = TextEditingController(text: testName);
    valueCtrl = TextEditingController(text: value);
    unitCtrl = TextEditingController(text: unit);
    snomedCodeCtrl = TextEditingController(text: snomedCode);
  }

  String get testName => testNameCtrl.text;
  set testName(String val) => testNameCtrl.text = val;

  String get value => valueCtrl.text;
  set value(String val) => valueCtrl.text = val;

  String get unit => unitCtrl.text;
  set unit(String val) => unitCtrl.text = val;

  String get snomedCode => snomedCodeCtrl.text;
  set snomedCode(String val) => snomedCodeCtrl.text = val;

  Map<String, dynamic> toJson() => {
        'testName': testNameCtrl.text,
        'value': valueCtrl.text,
        'unit': unitCtrl.text,
        if (snomedCodeCtrl.text.isNotEmpty) 'snomedCode': snomedCodeCtrl.text,
      };

  void dispose() {
    testNameCtrl.dispose();
    valueCtrl.dispose();
    unitCtrl.dispose();
    snomedCodeCtrl.dispose();
  }
}

class VitalFormItem {
  late final TextEditingController vitalNameCtrl;
  late final TextEditingController valueCtrl;
  late final TextEditingController unitCtrl;

  VitalFormItem({
    String vitalName = '',
    String value = '',
    String unit = '',
  }) {
    vitalNameCtrl = TextEditingController(text: vitalName);
    valueCtrl = TextEditingController(text: value);
    unitCtrl = TextEditingController(text: unit);
  }

  String get vitalName => vitalNameCtrl.text;
  set vitalName(String val) => vitalNameCtrl.text = val;

  String get value => valueCtrl.text;
  set value(String val) => valueCtrl.text = val;

  String get unit => unitCtrl.text;
  set unit(String val) => unitCtrl.text = val;

  Map<String, dynamic> toJson() => {
        'vitalName': vitalNameCtrl.text,
        'value': valueCtrl.text,
        'unit': unitCtrl.text,
      };

  void dispose() {
    vitalNameCtrl.dispose();
    valueCtrl.dispose();
    unitCtrl.dispose();
  }
}

class AllergyFormItem {
  late final TextEditingController allergyNameCtrl;
  String type; // medication, food, environment, biologic
  String status; // active, inactive, resolved

  AllergyFormItem({
    String allergyName = '',
    this.type = 'medication',
    this.status = 'active',
  }) {
    allergyNameCtrl = TextEditingController(text: allergyName);
  }

  String get allergyName => allergyNameCtrl.text;
  set allergyName(String val) => allergyNameCtrl.text = val;

  Map<String, dynamic> toJson() => {
        'allergyName': allergyNameCtrl.text,
        'type': type,
        'status': status,
      };

  void dispose() {
    allergyNameCtrl.dispose();
  }
}

class ImmunizationFormItem {
  late final TextEditingController vaccineNameCtrl;
  late final TextEditingController lotNumberCtrl;
  late final TextEditingController doseNumberCtrl;
  late final TextEditingController dateCtrl;

  ImmunizationFormItem({
    String vaccineName = '',
    String lotNumber = '',
    String doseNumber = '1',
    String date = '',
  }) {
    vaccineNameCtrl = TextEditingController(text: vaccineName);
    lotNumberCtrl = TextEditingController(text: lotNumber);
    doseNumberCtrl = TextEditingController(text: doseNumber);
    dateCtrl = TextEditingController(text: date);
  }

  String get vaccineName => vaccineNameCtrl.text;
  set vaccineName(String val) => vaccineNameCtrl.text = val;

  String get lotNumber => lotNumberCtrl.text;
  set lotNumber(String val) => lotNumberCtrl.text = val;

  String get doseNumber => doseNumberCtrl.text;
  set doseNumber(String val) => doseNumberCtrl.text = val;

  String get date => dateCtrl.text;
  set date(String val) => dateCtrl.text = val;

  Map<String, dynamic> toJson() => {
        'vaccineName': vaccineNameCtrl.text,
        'lotNumber': lotNumberCtrl.text,
        'doseNumber': doseNumberCtrl.text,
        'date': dateCtrl.text,
      };

  void dispose() {
    vaccineNameCtrl.dispose();
    lotNumberCtrl.dispose();
    doseNumberCtrl.dispose();
    dateCtrl.dispose();
  }
}
