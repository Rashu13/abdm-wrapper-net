import 'package:get/get.dart';
import 'dart:html' as html;

class DashboardController extends GetxController {
  var currentIndex = 0.obs;

  @override
  void onInit() {
    super.onInit();
    
    // Load initial tab from the browser URL parameter if present
    _updateTabFromUrl();

    // Listen to browser Back/Forward navigation buttons
    try {
      html.window.onPopState.listen((event) {
        _updateTabFromUrl();
      });
    } catch (e) {
      print("Error registering popstate listener: $e");
    }
  }

  void changeTab(int index, {bool updateHistory = true}) {
    if (index < 0 || index > 6) return;
    currentIndex.value = index;

    if (updateHistory) {
      try {
        final currentUrl = html.window.location.href;
        final uri = Uri.parse(currentUrl);
        // Construct the new URL hash containing the tab query parameter (e.g. #/dashboard?tab=3)
        final newUrl = '${uri.origin}${uri.path}#/dashboard?tab=$index';
        html.window.history.pushState(null, '', newUrl);
      } catch (e) {
        print("Error updating browser history state: $e");
      }
    }
  }

  void _updateTabFromUrl() {
    try {
      final href = html.window.location.href;
      final uri = Uri.parse(href);
      final fragment = uri.fragment; // e.g. "/dashboard?tab=3"
      if (fragment.isNotEmpty) {
        final fragmentUri = Uri.parse(fragment);
        final tabStr = fragmentUri.queryParameters['tab'];
        if (tabStr != null) {
          final tabIndex = int.tryParse(tabStr);
          if (tabIndex != null && tabIndex >= 0 && tabIndex <= 6) {
            currentIndex.value = tabIndex;
          }
        }
      }
    } catch (e) {
      print("Error parsing tab index from URL: $e");
    }
  }
}
