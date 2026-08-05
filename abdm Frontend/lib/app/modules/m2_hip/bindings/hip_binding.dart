import 'package:get/get.dart';
import '../controllers/care_context_controller.dart';
import '../controllers/scan_share_controller.dart';

class HipBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<CareContextController>(() => CareContextController());
    Get.lazyPut<ScanShareController>(() => ScanShareController());
  }
}
