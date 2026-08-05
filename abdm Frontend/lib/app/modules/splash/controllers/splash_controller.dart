import 'package:get/get.dart';
import '../../../routes/app_paths.dart';

class SplashController extends GetxController {
  @override
  void onReady() {
    super.onReady();
    _navigateToNext();
  }

  void _navigateToNext() async {
    await Future.delayed(const Duration(seconds: 2));
    Get.offAllNamed(Routes.DASHBOARD);
  }
}
