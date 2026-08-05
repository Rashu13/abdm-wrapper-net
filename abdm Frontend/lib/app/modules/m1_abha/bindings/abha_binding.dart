import 'package:get/get.dart';
import '../controllers/abha_creation_controller.dart';

class AbhaBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<AbhaCreationController>(() => AbhaCreationController());
  }
}
