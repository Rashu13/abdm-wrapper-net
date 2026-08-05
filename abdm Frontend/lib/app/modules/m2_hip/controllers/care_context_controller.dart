import 'package:get/get.dart';
import '../../../data/model/response/m2/care_context_model.dart';
import '../../../data/repository/m2/hip_care_context_repo.dart';

class CareContextController extends GetxController {
  var isLoading = false.obs;
  var isLinking = false.obs;
  var careContexts = <CareContextModel>[].obs;

  @override
  void onInit() {
    super.onInit();
    fetchCareContexts();
  }

  Future<void> fetchCareContexts() async {
    isLoading.value = true;
    var list = await HipCareContextRepo.getLinkedCareContexts();
    careContexts.value = list;
    isLoading.value = false;
  }

  Future<void> linkNewCareContext(String abhaAddress, String visitRef, {String? display}) async {
    isLinking.value = true;
    bool success = await HipCareContextRepo.linkCareContext(
      abhaAddress: abhaAddress,
      visitRef: visitRef,
      display: display,
    );
    isLinking.value = false;
    if (success) {
      Get.snackbar('Success', 'Care Context $visitRef linked to $abhaAddress!');
      fetchCareContexts();
    }
  }
}
