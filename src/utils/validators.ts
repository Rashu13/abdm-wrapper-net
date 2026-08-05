// ─── Aadhaar Validators ──────────────────────────────────────────────────────

/**
 * Verhoeff Algorithm — checksum validation for Aadhaar numbers.
 * This is the official algorithm used by UIDAI for Aadhaar validation.
 */
const VERHOEFF_D = [
  [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
  [1, 2, 3, 4, 0, 6, 7, 8, 9, 5],
  [2, 3, 4, 0, 1, 7, 8, 9, 5, 6],
  [3, 4, 0, 1, 2, 8, 9, 5, 6, 7],
  [4, 0, 1, 2, 3, 9, 5, 6, 7, 8],
  [5, 9, 8, 7, 6, 0, 4, 3, 2, 1],
  [6, 5, 9, 8, 7, 1, 0, 4, 3, 2],
  [7, 6, 5, 9, 8, 2, 1, 0, 4, 3],
  [8, 7, 6, 5, 9, 3, 2, 1, 0, 4],
  [9, 8, 7, 6, 5, 4, 3, 2, 1, 0],
];

const VERHOEFF_P = [
  [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
  [1, 5, 7, 6, 2, 8, 3, 0, 9, 4],
  [5, 8, 0, 3, 7, 9, 6, 1, 4, 2],
  [8, 9, 1, 6, 0, 4, 3, 5, 2, 7],
  [9, 4, 5, 3, 1, 2, 6, 8, 7, 0],
  [4, 2, 8, 6, 5, 7, 3, 9, 0, 1],
  [2, 7, 9, 3, 8, 0, 6, 4, 1, 5],
  [7, 0, 4, 6, 9, 1, 3, 2, 5, 8],
];

const VERHOEFF_INV = [0, 4, 3, 2, 1, 5, 6, 7, 8, 9];

function verhoeffCheck(num: string): boolean {
  let c = 0;
  const digits = num.split('').reverse().map(Number);
  for (let i = 0; i < digits.length; i++) {
    c = VERHOEFF_D[c][VERHOEFF_P[i % 8][digits[i]]];
  }
  return VERHOEFF_INV[c] === 0;
}

/**
 * Validates a 12-digit Aadhaar number using:
 * 1. Length check (must be 12 digits)
 * 2. Cannot start with 0 or 1 (UIDAI rule)
 * 3. Verhoeff checksum validation
 */
export function validateAadhaar(aadhaar: string): { valid: boolean; error?: string } {
  const cleaned = aadhaar.replace(/\s/g, '');

  if (!/^\d{12}$/.test(cleaned)) {
    return { valid: false, error: 'Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.' };
  }

  if (/^[01]/.test(cleaned)) {
    return { valid: false, error: 'Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.' };
  }

  // Note: Verhoeff check is strict; in dev, you may comment it out with test numbers
  if (!verhoeffCheck(cleaned)) {
    return { valid: false, error: 'Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.' };
  }

  return { valid: true };
}

/**
 * Validates a 6-digit OTP
 */
export function validateOtp(otp: string): { valid: boolean; error?: string } {
  if (!/^\d{6}$/.test(otp)) {
    return { valid: false, error: 'Please enter a valid 6-digit OTP.' };
  }
  return { valid: true };
}

/**
 * Validates mobile number (10 digits, starts with 6-9)
 */
export function validateMobile(mobile: string): { valid: boolean; error?: string } {
  if (!/^[6-9]\d{9}$/.test(mobile)) {
    return { valid: false, error: 'Please enter a valid 10-digit mobile number.' };
  }
  return { valid: true };
}

/**
 * Masks Aadhaar for display: XXXX-XXXX-1234
 */
export function maskAadhaar(aadhaar: string): string {
  const cleaned = aadhaar.replace(/\s/g, '');
  if (cleaned.length !== 12) return aadhaar;
  return `XXXX-XXXX-${cleaned.slice(8)}`;
}

/**
 * Formats raw 12-digit Aadhaar to display groups: 1234 5678 9012
 */
export function formatAadhaarDisplay(part1: string, part2: string, part3: string): string {
  return `${part1}${part2}${part3}`;
}
