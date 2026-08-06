import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:get_storage/get_storage.dart';
import 'app/routes/app_pages.dart';
import 'app/modules/splash/bindings/splash_binding.dart';
import 'app/modules/splash/views/splash_view.dart';
import 'util/constants.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await GetStorage.init();

  runApp(
    GetMaterialApp(
      debugShowCheckedModeBanner: false,
      title: "ABDM Healthcare Web Suite",
      theme: ThemeData.light().copyWith(
        scaffoldBackgroundColor: AppColor.background,
        primaryColor: AppColor.primary,
        cardColor: AppColor.surface,
        colorScheme: const ColorScheme.light(
          primary: AppColor.primary,
          secondary: AppColor.accent,
          surface: AppColor.surface,
          background: AppColor.background,
        ),
      ),
      initialRoute: AppPages.INITIAL,
      getPages: AppPages.routes,
      initialBinding: SplashBinding(),
      unknownRoute: GetPage(
        name: '/not-found',
        page: () => const SplashView(),
        binding: SplashBinding(),
      ),
      builder: (context, widget) {
        if (widget == null) return const SizedBox.shrink();
        return Container(
          color: AppColor.background,
          child: Center(
            child: widget,
          ),
        );
      },
    ),
  );
}
