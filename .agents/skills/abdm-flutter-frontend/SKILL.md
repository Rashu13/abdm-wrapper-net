---
name: abdm-flutter-frontend
description: Architecture overview, GetX state management, folder conventions, and M1/M2/M3 ABDM (Ayushman Bharat Digital Mission) integration guidelines for the ABDM Flutter Frontend application.
---

# ABDM Flutter Frontend Architecture & Implementation Skill Guide

This guide defines the architecture, folder organization, coding conventions, and implementation details for building the **ABDM (Ayushman Bharat Digital Mission) Flutter Frontend Application** covering **Milestones M1, M2, and M3**.

The architecture adapts the **GetX Module-Driven Clean Architecture** pattern.

---

## 1. ABDM Scope & Milestone Breakdown

The ABDM Frontend application is divided into 3 core ABDM functional milestones:

| Milestone | Feature Scope | Key Components & APIs |
| :--- | :--- | :--- |
| **M1** | **ABHA Creation & Authentication** | Aadhaar OTP, Mobile OTP, ABHA Address creation, ABHA Card display & PDF download, ABHA Login, Profile verification |
| **M2** | **HIP (Health Information Provider)** | Patient discovery by ABHA ID, Care Context linking (OPD/IPD/Lab), Consent notification handling, Data sharing push |
| **M3** | **HIU (Health Information User)** | Consent Request generation, Consent Artefact tracking, Encrypted FHIR record retrieval, Decryption (ECDH), FHIR Data rendering |

---

## 2. Directory Layout & Module Structure

```
lib/
├── main.dart
├── app/
│   ├── data/
│   │   ├── api/
│   │   │   ├── abdm_server.dart          # HTTP client for ABDM Wrapper API & Gateway
│   │   │   └── fhir_parser.dart          # FHIR Bundle decoder & helper utilities
│   │   ├── model/
│   │   │   ├── body/                     # Request DTOs
│   │   │   │   ├── m1/                   # Aadhaar OTP request, ABHA registration body
│   │   │   │   ├── m2/                   # Link Care Context body, HIP notify body
│   │   │   │   └── m3/                   # Consent request body, Health data fetch body
│   │   │   └── response/                 # Response DTOs
│   │   │       ├── m1/                   # ABHA Profile model, Auth Token model, OTP verify
│   │   │       ├── m2/                   # Care Context link response, Discovery response
│   │   │       └── m3/                   # Consent Artefact model, FHIR Health Record model
│   │   └── repository/
│   │       ├── m1/                       # AbhaCreationRepo, AbhaAuthRepo
│   │       ├── m2/                       # HipCareContextRepo, DiscoveryRepo
│   │       └── m3/                       # ConsentRepo, HiuHealthRecordRepo
│   ├── modules/                          # GetX Feature Modules
│   │   ├── m1_abha/                      # ABDM Milestone 1
│   │   │   ├── bindings/                 # AbhaBinding
│   │   │   ├── controllers/              # AbhaCreationController, AbhaAuthController
│   │   │   ├── views/                    # CreateAbhaView, VerifyOtpView, AbhaCardView
│   │   │   └── widget/                   # AadhaarInputCard, AbhaCardWidget
│   │   ├── m2_hip/                       # ABDM Milestone 2 (HIP)
│   │   │   ├── bindings/                 # HipRecordBinding
│   │   │   ├── controllers/              # CareContextController, HipDiscoveryController
│   │   │   ├── views/                    # LinkCareContextView, DiscoveryView
│   │   │   └── widget/                   # CareContextTile, PatientLinkCard
│   │   ├── m3_hiu/                       # ABDM Milestone 3 (HIU)
│   │   │   ├── bindings/                 # ConsentBinding, HealthRecordBinding
│   │   │   ├── controllers/              # ConsentController, HealthRecordController
│   │   │   ├── views/                    # ConsentRequestView, HealthRecordViewerPage
│   │   │   └── widget/                   # FhirRecordCard, ConsentStatusBadge
│   │   ├── splash/
│   │   └── dashboard/
│   └── routes/
│       ├── app_paths.dart                # Route path strings
│       └── app_pages.dart                # GetPage route configurations
├── helper/
│   ├── crypto_helper.dart                # Key pair generation (ECDH) & FHIR decryption
│   └── pdf_helper.dart                   # ABHA Card / Health Record PDF generator
├── translation/
│   └── language.dart                     # Multilingual support (English, Hindi, regional)
├── util/
│   ├── api_endpoints.dart                # Backend Base URL & ABDM API Endpoints
│   ├── constants.dart                    # Theme Colors, Medical Asset Icons
│   └── style.dart                        # ScreenUtil typography & styling definitions
└── widget/                               # Shared Global UI Components (Loaders, Toasts, Headers)
```

---

## 3. Module Design Specifications

### A. Milestone M1: ABHA Creation & Authentication (`m1_abha`)
- **Flow**:
  1. User enters Aadhaar Number -> API triggers OTP to Aadhaar registered mobile.
  2. User enters OTP -> Receives Aadhaar profile data -> Configures ABHA Address (`@abdm`).
  3. Generates ABHA Card -> Displays QR code, ABHA Number, and allows PDF download.
- **Controller Responsibilities (`AbhaCreationController`)**:
  - `sendAadhaarOtp(String aadhaarNumber)`
  - `verifyAadhaarOtp(String otp, String txnId)`
  - `createAbhaAddress(String abhaAddress, String password)`
  - `downloadAbhaCard()`

```dart
class AbhaCreationController extends GetxController {
  var isLoading = false.obs;
  var txnId = ''.obs;
  var abhaProfile = Rxn<AbhaProfileModel>();

  Future<void> sendAadhaarOtp(String aadhaarNumber) async {
    isLoading.value = true;
    var result = await AbhaCreationRepo.generateAadhaarOtp(aadhaarNumber);
    if (result != null) {
      txnId.value = result.txnId ?? '';
      Get.toNamed(Routes.M1_VERIFY_OTP);
    }
    isLoading.value = false;
  }
}
```

### B. Milestone M2: HIP - Health Information Provider (`m2_hip`)
- **Flow**:
  1. Health Facility (Hospital/Clinic) discovers patient by ABHA ID.
  2. Facility attaches Care Contexts (e.g., OPD Visit #1024, Lab Report #552) to patient's ABHA.
  3. Patient receives link notification and approves care context binding.
- **Controller Responsibilities (`CareContextController`)**:
  - `searchPatientByAbha(String abhaId)`
  - `linkCareContext({required String patientId, required String careContextId, required String display})`
  - `getLinkedCareContexts()`

```dart
class CareContextController extends GetxController {
  var isLinking = false.obs;
  List<CareContextData> linkedContexts = [];

  Future<void> linkContext(String patientAbha, String visitId) async {
    isLinking.value = true;
    update();
    var response = await HipCareContextRepo.linkCareContext(
      patientAbha: patientAbha,
      visitId: visitId,
    );
    if (response != null && response.isSuccess) {
      Get.snackbar('Success', 'Care Context linked successfully!');
      getLinkedCareContexts();
    }
    isLinking.value = false;
    update();
  }
}
```

### C. Milestone M3: HIU - Consent & Health Records (`m3_hiu`)
- **Flow**:
  1. Doctor/HIU raises a Consent Request specifying record types (OPD, Prescription, Lab), date range, and purpose.
  2. Patient receives consent request -> Approves or Denies.
  3. Once GRANTED, HIU requests encrypted health data bundle.
  4. App decrypts data bundle using ECDH key pair (`crypto_helper.dart`) and renders FHIR records (Prescriptions, Diagnostic Reports, Discharge Summaries).
- **Controller Responsibilities (`ConsentController` & `HealthRecordController`)**:
  - `createConsentRequest(ConsentRequestBody body)`
  - `fetchConsentStatus(String requestId)`
  - `fetchAndDecryptHealthRecord(String consentArtefactId)`

```dart
class HealthRecordController extends GetxController {
  var isFetching = false.obs;
  var fhirRecord = Rxn<FhirBundleModel>();

  Future<void> loadRecord(String artefactId) async {
    isFetching.value = true;
    // 1. Generate keypair for Diffie-Hellman key exchange
    var keyPair = CryptoHelper.generateEcdhKeys();
    
    // 2. Fetch encrypted data bundle from ABDM wrapper API
    var encryptedData = await HiuHealthRecordRepo.fetchEncryptedData(artefactId, keyPair.publicKey);
    
    // 3. Decrypt payload using private key & shared key
    if (encryptedData != null) {
      var jsonString = CryptoHelper.decryptFhirBundle(encryptedData, keyPair.privateKey);
      fhirRecord.value = FhirParser.parseBundle(jsonString);
    }
    isFetching.value = false;
  }
}
```

---

## 4. Network & Security Guidelines (`AbdmServer`)

All network communication with the backend wrapper / ABDM Gateway passes through `lib/app/data/api/abdm_server.dart`.

### Headers Required:
- `Authorization`: Bearer Token (`GetStorage().read('auth_token')`)
- `X-CM-ID`: Consent Manager ID (`sbx` or `abdm`)
- `Content-Type`: `application/json`
- `Accept`: `application/json`

---

## 5. UI & Styling Rules for ABDM Frontend

1. **Responsive Layouts**: Use `flutter_screenutil` (`.sp`, `.h`, `.w`) with base design canvas size `(360, 800)`.
2. **Medical Typography**:
   - Primary Font: `Rubik` or `Inter`.
   - ABHA Numbers & Sensitive Identifiers: Medium/Bold style with copyable QR code.
3. **Status Badges**:
   - `GRANTED` -> Green (`AppColor.green`)
   - `REQUESTED` / `PENDING` -> Orange/Amber
   - `DENIED` / `EXPIRED` / `REVOKED` -> Red (`AppColor.red`)

---

## 6. How to Implement a New ABDM Feature Step-by-Step

1. **API Registration**: Add API endpoint constant to `util/api_endpoints.dart`.
2. **DTO Models**: Create Request/Response model files in `app/data/model/<body|response>/<m1|m2|m3>/`.
3. **Repository**: Create static repo methods in `app/data/repository/<m1|m2|m3>/`.
4. **Module Scaffold**: Create module directory in `app/modules/<m1_abha|m2_hip|m3_hiu>/` with `bindings/`, `controllers/`, `views/`, `widget/`.
5. **Route Mapping**: Register path in `routes/app_paths.dart` and `GetPage` in `routes/app_pages.dart`.
6. **UI & State**: Bind view to controller using `GetView<Controller>` and call repository methods.
