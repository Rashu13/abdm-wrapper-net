---
name: abdm-m1-v3-api-guide
description: Official ABDM (Ayushman Bharat Digital Mission) ABHA V3 API Integration Skill Guide covering M1 Creation, Verification, Biometrics, Profile, Benefit APIs, and PHR ABHA Address verification.
---

# ABDM ABHA V3 Milestone 1 (M1) API Integration Skill Guide

This skill guide provides the technical reference and workflow steps for implementing **ABDM Milestone 1 (M1) ABHA V3 APIs** based on NHA Integrator Guide v1.4.

---

## 1. Environment & Authentication Architecture

### Base URLs:
- **Sandbox (SBX):** `https://abhasbx.abdm.gov.in/abha/api`
- **Production (PROD):** `https://abha.abdm.gov.in/api/abha`
- **PHR / ABHA Address Web (SBX):** `https://abhasbx.abdm.gov.in/abha/api/v3/phr/web`
- **PHR / ABHA Address Web (PROD):** `https://phr.abdm.gov.in/api/phr/web/v3`

### Common Headers Required:
- `REQUEST-ID`: Unique UUID v4 for request tracking.
- `TIMESTAMP`: ISO 8601 UTC timestamp (`YYYY-MM-DDTHH:mm:ss.sssZ`).
- `X-CM-ID`: Environment identifier (`sbx` or `abdm`).
- `Authorization`: `Bearer {accessToken}` (generated via Gateway Session API).

---

## 2. RSA Encryption Protocol

Sensitive values (Aadhaar Number, Mobile Number, OTP, Password) **must** be RSA encrypted.

1. **Fetch Public Cert:** `GET /v3/profile/public/certificate`
2. **Cipher Algorithm:** `RSA/ECB/OAEPWithSHA-1AndMGF1Padding`
3. **Encoding:** Base64 output of encrypted payload.

---

## 3. ABHA Creation Flow (Milestone 1)

### A. Aadhaar OTP Enrolment Flow
1. **Request OTP:** `POST /v3/enrollment/request/otp`
   - `scope`: `["abha-enrol"]`
   - `loginHint`: `"aadhaar"`
   - `loginId`: RSA-encrypted 12-digit Aadhaar number.
   - `otpSystem`: `"aadhaar"`
2. **Verify OTP & Enrol:** `POST /v3/enrollment/enrol/byAadhaar`
   - `authData`: `{ "authMethods": ["otp"], "otp": { "txnId": "...", "otpValue": "{encrypted_otp}", "mobile": "{primary_mobile}" } }`
   - `consent`: `{ "code": "abha-enrollment", "version": "1.4" }`
3. **Mobile & Email Verification:**
   - Verify non-Aadhaar primary mobile using `POST /v3/enrollment/request/otp` with scope `mobile-verify`.
4. **Suggestions & ABHA Address Setup:**
   - Fetch handle suggestions: `GET /v3/enrollment/enrol/suggestion?txnId={txnId}`
   - Finalize ABHA Address (`@abdm`): `POST /v3/enrollment/enrol/abha-address`

### B. Alternative Enrolment Modes:
- **Driving License (DL):** Document upload & verification flow.
- **Demographic Auth:** Matching Name, Gender, YOB without OTP.
- **Biometric Enrolment:** Fingerprint Auth, Face Auth, Iris Auth using RD Service.

---

## 4. ABHA Verification & Login Workflows

### Supported Verification Modes:
1. **Aadhaar OTP Login:** `POST /v3/enrollment/request/otp` (`loginHint: "aadhaar"`)
2. **Mobile OTP Login:** `POST /v3/enrollment/request/otp` (`loginHint: "mobile"`)
3. **ABHA OTP Login:** `POST /v3/enrollment/request/otp` (`loginHint: "abha-number"`)
4. **Biometric Login:** Face, Fingerprint, or Iris RD Service payload verification.

### Profile Retrieval & Card Download:
- **Get Profile:** `GET /v3/profile/account`
- **Download ABHA Card:** `GET /v3/profile/account/abha-card` (returns PNG/PDF byte stream)
- **Generate QR Code:** `GET /v3/profile/account/qr-code`

---

## 5. ABHA Address (PHR) Verification

For web/mobile PHR applications verifying existing `@abdm` handles:
- **Mobile OTP:** `POST /v3/phr/web/search/verify`
- **Aadhaar OTP:** `POST /v3/phr/web/search/aadhaar/verify`
- **Fetch PHR Profile:** `GET /v3/phr/web/profile`
