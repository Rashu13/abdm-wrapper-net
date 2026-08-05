import 'package:get/get.dart';
import '../controllers/dashboard_controller.dart';
import '../../m1_abha/controllers/abha_creation_controller.dart';
import '../../m2_hip/controllers/care_context_controller.dart';
import '../../m3_hiu/controllers/health_record_controller.dart';

class DashboardBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<DashboardController>(() => DashboardController());
    Get.lazyPut<AbhaCreationController>(() => AbhaCreationController());
    Get.lazyPut<CareContextController>(() => CareContextController());
    Get.lazyPut<HealthRecordController>(() => HealthRecordController());
  }
}
