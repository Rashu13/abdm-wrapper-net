---
name: foodking-flutter-architecture
description: Architecture overview, coding conventions, state management patterns, and data flow guidelines for the FoodKing Flutter Customer App codebase.
---

# FoodKing Flutter Codebase Architecture & Guidelines

This document serves as the authoritative guide for understanding, writing, extending, and maintaining code within the `foodking-customer-app` (`lib/`) directory.

---

## 1. Overview & Architecture Strategy

The application follows the **GetX Pattern Architecture (Module-Driven Clean Architecture)**. The codebase strictly separates UI, Business Logic, Dependency Injection, and Data Management layers.

```
lib/
├── app/
│   ├── data/            # Data Layer (Network, Repositories, Data DTOs)
│   ├── modules/         # Feature Modules (Bindings, Controllers, Views, Widgets)
│   └── routes/          # Navigation & Route Definitions
├── helper/              # Utility Services (Notifications, Device Info)
├── translation/         # Localization & Translations Dictionary
├── util/                # Constants, App Styling, Colors, API Configs
└── widget/              # Reusable Global UI Components
```

---

## 2. Directory Layout Breakdown

### `lib/app/modules/<feature_name>/`
Each feature (e.g., `home`, `cart`, `checkout`, `auth`, `order`, `profile`) is isolated inside its own module folder with 4 standard subdirectories:

1. **`bindings/`**: Defines dependency bindings by extending `Bindings`. Injects controllers lazily or eagerly via `Get.lazyPut(() => FeatureController())`.
2. **`controllers/`**: Manages state, API execution, and reactive variables by extending `GetxController`.
3. **`views/`**: Renders screens using `GetView<FeatureController>` or `StatelessWidget`.
4. **`widget/`**: Contains UI widgets exclusive to that specific feature module.

### `lib/app/data/`
1. **`api/` (`server.dart`)**: Low-level HTTP client handling requests (`GET`, `POST`, `PUT`, `DELETE`, `Multipart`). Injects headers (`Authorization`, `x-api-key`, `x-localization`, `Content-Type`).
2. **`model/`**:
   - `body/`: Request DTOs (e.g., `place_order_body.dart`, `notification_body.dart`).
   - `response/`: Response DTOs (e.g., `item_model.dart`, `category_model.dart`, `cart_model.dart`). Uses factory methods `fromJson` and `toJson`.
3. **`repository/`**: Static repository classes (e.g., `CategoryRepo`, `ItemRepo`, `BranchRepo`) that bridge raw API responses with Data Models.

### `lib/app/routes/`
- **`app_routes.dart`**: Declares string constants for paths (`_Paths`) and routes (`Routes`).
- **`app_pages.dart`**: Maps `Routes` to `GetPage` definitions, pairing each view with its initial binding.

### `lib/util/`
- **`api-list.dart`**: Central static API endpoints registry (`APIList.baseUrl`, endpoint strings, and dynamic endpoint builder methods).
- **`constant.dart`**: App theme colors (`AppColor`), image/icon path constants (`AppImages`).
- **`style.dart`**: Typography styling, screen dimensions (`Dimensions`), and font definitions (Rubik & Google Fonts Roboto for currency). Uses `flutter_screenutil` (`.sp`).

---

## 3. Core Coding Patterns & Conventions

### A. State Management & Controllers
- Controllers extend `GetxController`.
- State initialization and API prefetching are triggered inside `onInit()`.
- State updates use both GetX reactive primitives (`Rx`, `.obs`) and traditional GetX `update()` manual re-render notifications.

```dart
class HomeController extends GetxController {
  bool menuLoader = false;
  List<CategoryData> categoryDataList = [];

  @override
  void onInit() {
    getCategoryList();
    super.onInit();
  }

  getCategoryList() async {
    menuLoader = true;
    update();
    var categoryData = await CategoryRepo.getCategory();
    if (categoryData != null) {
      categoryDataList = categoryData.data!;
    }
    menuLoader = false;
    update();
  }
}
```

### B. Repositories
Repositories perform asynchronous API operations using `Server` and parse raw HTTP responses into strongly-typed Data Models.

```dart
class CategoryRepo {
  static Server server = Server();

  static Future<CategoryModel?> getCategory() async {
    try {
      var response = await server.getRequestWithoutToken(
        endPoint: APIList.category! + "?order_column=id&order_type=asc&status=5",
      );
      if (response != null && response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);
        return CategoryModel.fromJson(jsonResponse);
      }
    } catch (e) {
      debugPrint(e.toString());
    }
    return null;
  }
}
```

### C. UI & Responsive Design
- Screen scaling is handled via `flutter_screenutil` (`ScreenUtilInit` base resolution: `360x800`).
- Text styles are defined in `lib/util/style.dart` using font size extension `.sp` (e.g., `fontSizeSmall`, `fontBold`).
- Currencies utilize `GoogleFonts.roboto` to guarantee accurate representation of currency symbols.

### D. Navigation & Routing
Routes are triggered via GetX navigation methods:
```dart
Get.toNamed(Routes.CART);
Get.offAllNamed(Routes.DASHBOARD);
```

### E. Local Storage & Auth Token Management
- Uses `GetStorage` (`box.read('token')`, `box.read('languageCode')`, `box.read('isLogedIn')`).
- Authentication Bearer tokens and Localization codes are dynamically attached to network requests inside `Server._getHttpHeaders()`.

---

## 4. Guidelines for Adding New Features

1. **Create Module**: Add directory `lib/app/modules/<new_feature>/` with `bindings/`, `controllers/`, `views/`, and `widget/`.
2. **Define Routes**: Add route name in `app_routes.dart` and register `GetPage` entry with its binding in `app_pages.dart`.
3. **Data Layer**:
   - Add endpoint in `lib/util/api-list.dart`.
   - Create Response/Request models in `lib/app/data/model/`.
   - Create Repository static methods in `lib/app/data/repository/`.
4. **Controller Logic**: Write business logic and state calls in `<new_feature>_controller.dart`.
5. **UI View**: Implement UI in `<new_feature>_view.dart` utilizing global styles (`lib/util/style.dart`) and reusable widgets (`lib/widget/`).
