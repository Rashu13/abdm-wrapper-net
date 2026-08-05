// ─── RSA Encryption Utility ─────────────────────────────────────────────────
// Uses jsencrypt library for RSA public-key encryption of sensitive data
// (Aadhaar number, OTP) before sending over the wire.

import JSEncrypt from 'jsencrypt';

let encryptor: JSEncrypt | null = null;

/**
 * Initializes the RSA encryptor with the public key.
 * Call this once after fetching the public key from backend (/v1/auth/cert).
 */
export function initRsaEncryptor(publicKey: string): void {
  encryptor = new JSEncrypt();
  encryptor.setPublicKey(publicKey);
}

/**
 * Encrypts plaintext using RSA public key.
 * Returns base64-encoded ciphertext.
 * Throws if encryptor not initialized.
 */
export function rsaEncrypt(plaintext: string): string {
  if (!encryptor) {
    throw new Error(
      '[RSA] Encryptor not initialized. Call initRsaEncryptor() with the public key first.',
    );
  }

  const encrypted = encryptor.encrypt(plaintext);
  if (!encrypted) {
    throw new Error('[RSA] Encryption failed. Invalid public key or plaintext.');
  }

  return encrypted;
}

/**
 * Checks if the RSA encryptor has been initialized.
 */
export function isRsaReady(): boolean {
  return encryptor !== null;
}

/**
 * Resets the RSA encryptor (e.g., on session expiry).
 */
export function resetRsaEncryptor(): void {
  encryptor = null;
}

/**
 * Safely encrypt — returns null instead of throwing.
 * Use in contexts where you want graceful degradation.
 */
export function safeRsaEncrypt(plaintext: string): string | null {
  try {
    return rsaEncrypt(plaintext);
  } catch {
    return null;
  }
}
