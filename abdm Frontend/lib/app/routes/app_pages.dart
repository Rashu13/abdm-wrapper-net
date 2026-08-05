import 'package:get/get.dart';
import 'app_paths.dart';
import '../modules/splash/bindings/splash_binding.dart';
import '../modules/splash/views/splash_view.dart';
import '../modules/dashboard/bindings/dashboard_binding.dart';
import '../modules/dashboard/views/dashboard_view.dart';
import '../modules/m1_abha/bindings/abha_binding.dart';
import '../modules/m1_abha/views/create_abha_view.dart';
import '../modules/m1_abha/views/verify_otp_view.dart';
import '../modules/m1_abha/views/select_abha_address_view.dart';
import '../modules/m1_abha/views/abha_card_view.dart';
import '../modules/m1_abha/views/search_abha_card_view.dart';
import '../modules/m1_abha/views/galaxy_health_abha_view.dart';
import '../modules/m2_hip/bindings/hip_binding.dart';
import '../modules/m2_hip/views/discovery_view.dart';
import '../modules/m3_hiu/bindings/hiu_binding.dart';
import '../modules/m3_hiu/views/consent_request_view.dart';
import '../modules/m3_hiu/views/health_record_viewer_page.dart';

class AppPages {
  static const INITIAL = Routes.SPLASH;

  static final routes = [
    GetPage(
      name: Routes.SPLASH,
      page: () => const SplashView(),
      binding: SplashBinding(),
    ),
    GetPage(
      name: Routes.DASHBOARD,
      page: () => DashboardView(),
      binding: DashboardBinding(),
    ),
    GetPage(
      name: Routes.M1_CREATE_ABHA,
      page: () => CreateAbhaView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.M1_VERIFY_OTP,
      page: () => VerifyOtpView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.M1_SELECT_ABHA,
      page: () => SelectAbhaAddressView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.M1_ABHA_CARD,
      page: () => const AbhaCardView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.M1_SEARCH_ABHA,
      page: () => const SearchAbhaCardView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.GALAXY_ABHA,
      page: () => GalaxyHealthAbhaView(),
      binding: AbhaBinding(),
    ),
    GetPage(
      name: Routes.M2_DISCOVERY,
      page: () => const DiscoveryView(),
      binding: HipBinding(),
    ),
    GetPage(
      name: Routes.M3_CONSENT_REQUEST,
      page: () => const ConsentRequestView(),
      binding: HiuBinding(),
    ),
    GetPage(
      name: Routes.M3_HEALTH_RECORDS,
      page: () => const HealthRecordViewerPage(),
      binding: HiuBinding(),
    ),
  ];
}
