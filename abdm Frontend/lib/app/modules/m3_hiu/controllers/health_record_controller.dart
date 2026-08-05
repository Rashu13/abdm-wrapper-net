import 'package:get/get.dart';
import '../../../data/api/fhir_parser.dart';
import '../../../data/model/response/m3/consent_model.dart';
import '../../../data/repository/m3/hiu_health_record_repo.dart';

class HealthRecordController extends GetxController {
  var isLoadingConsents = false.obs;
  var isFetchingRecords = false.obs;
  var consents = <ConsentModel>[].obs;
  var fhirRecords = <FhirRecordItem>[].obs;
  var selectedConsentId = ''.obs;

  @override
  void onInit() {
    super.onInit();
    fetchConsents();
  }

  Future<void> fetchConsents() async {
    isLoadingConsents.value = true;
    var list = await HiuHealthRecordRepo.getConsents();
    consents.value = list;
    isLoadingConsents.value = false;
  }

  Future<void> fetchAndDecryptRecords(String consentId) async {
    selectedConsentId.value = consentId;
    isFetchingRecords.value = true;
    var records = await HiuHealthRecordRepo.fetchDecryptedRecords(consentId);
    fhirRecords.value = records;
    isFetchingRecords.value = false;
  }
}
