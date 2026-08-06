---
name: abdm-m2-v3-api-guide
description: Official ABDM (Ayushman Bharat Digital Mission) HIE-CM V3 User-Initiated Linking API Guide covering Discover, On-Discover, Link Init, Link On-Init, Link Confirm, Link On-Confirm, and SMS On-Notify callbacks.
---

# ABDM HIE-CM V3 User-Initiated Linking API Specification

User-initiated linking is the process in which a User/Patient searches for health records from ABDM-compliant health facilities (HIPs) via a PHR App (Patient HIU).

---

## 1. Sequence & Workflow Steps

```
[Patient / PHR App] ---> [HIE-CM Gateway] ---> [HIP Callback: /api/v3/hip/patient/care-context/discover]
                                                         |
                                             [HIP searches DB]
                                                         |
[HIE-CM Gateway] <--- [POST /api/v3/care-contexts/on-discover] <--- [HIP Wrapper]
```

```
[Patient clicks Link] ---> [HIE-CM Gateway] ---> [HIP Callback: /api/v3/hip/link/care-context/init]
                                                         |
                                         [HIP generates linkRef & sends OTP]
                                                         |
[HIE-CM Gateway] <--- [POST /api/v3/hip/link/care-context/on-init] <--- [HIP Wrapper]
```

```
[Patient enters OTP] ---> [HIE-CM Gateway] ---> [HIP Callback: /api/v3/hip/link/care-context/confirm]
                                                         |
                                         [HIP verifies OTP & links records]
                                                         |
[HIE-CM Gateway] <--- [POST /api/v3/hip/link/care-context/on-confirm] <--- [HIP Wrapper]
```

---

## 2. API Endpoints Reference

### 2.1 Patient Discovery
- **Gateway to HIP Callback:** `POST {callback_url}/api/v3/hip/patient/care-context/discover`
  - **Headers:** `REQUEST-ID`, `TIMESTAMP`, `X-HIP-ID`
  - **Body Payload:** Contains `transactionId`, `patient` object (with `id` i.e. ABHA Address, `name`, `gender`, `yearOfBirth`, `verifiedIdentifiers` e.g. MOBILE, and `unverifiedIdentifiers`).
- **HIP Response to Gateway:** `POST /api/v3/care-contexts/on-discover`
  - **Success Body:** `requestId`, `timestamp`, `transactionId`, `patient` (list of care contexts grouped by `hiType`), `matchedBy` (`["ABHA_ADDRESS"]`), `response` (`{ requestId }`).
  - **Error Body:** `code: "HIP-1000"`, `message: "Patient not found"`.

### 2.2 Link Initiation (OTP Trigger)
- **Gateway to HIP Callback:** `POST {callback_url}/api/v3/hip/link/care-context/init`
  - **Headers:** `REQUEST-ID`, `TIMESTAMP`, `X-HIP-ID`
  - **Body Payload:** `requestId`, `timestamp`, `transactionId`, `abhaAddress`, `patient` care contexts selected for linking.
- **HIP Response to Gateway:** `POST /api/v3/hip/link/care-context/on-init`
  - **Body Payload:** `requestId`, `timestamp`, `transactionId`, `link` (`referenceNumber`, `authenticationType: "MEDIATE"`, `meta: { communicationMedium: "MOBILE", communicationHint, communicationExpiry }`), `response` (`{ requestId }`).

### 2.3 Link Confirmation (OTP Verification & Final Linking)
- **Gateway to HIP Callback:** `POST {callback_url}/api/v3/hip/link/care-context/confirm`
  - **Headers:** `REQUEST-ID`, `TIMESTAMP`, `X-HIP-ID`
  - **Body Payload:** `requestId`, `timestamp`, `confirmation` (`linkRefNumber`, `token` i.e. OTP).
- **HIP Response to Gateway:** `POST /api/v3/hip/link/care-context/on-confirm`
  - **Body Payload (Success):** `requestId`, `timestamp`, `patient` (list of confirmed linked care contexts), `response` (`{ requestId }`).
  - **Body Payload (Error):** `code: "HIP-1000"`, `message: "CareContexts don't match"`.

---

## 3. Implementation in Wrapper
- **Controller:** `GatewayCallbackV3Controller.cs`
- **Manager:** `WorkflowV3Manager.cs`
- **Discovery Service:** `DiscoveryV3Service.cs`
- **Link Service:** `LinkV3Service.cs`
- **Database Logger:** `SqlServerRequestLogV3Service.cs` / `RequestLogV3Service.cs`
