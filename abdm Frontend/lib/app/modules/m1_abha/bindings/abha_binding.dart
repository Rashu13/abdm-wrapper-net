import 'package:get/get.dart';
import '../controllers/abha_creation_controller.dart';

class AbhaBinding extends Bindings {
  @override
  void dependencies() {
    // fenix: true - reuses existing instance if available, creates new only if disposed
    Get.lazyPut<AbhaCreationController>(
      () => AbhaCreationController(),
      fenix: true,
    );
  }
}
