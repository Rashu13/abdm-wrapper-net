import axios from 'axios';
import { GenerateOtpResponse } from '@/types/abha.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

export interface CaptchaData {
  captchaToken: string;
  captchaImage: string;
}

export async function fetchBackendCaptcha(): Promise<CaptchaData> {
  try {
    const response = await api.get<{ success: boolean; captchaToken: string; captchaImage: string }>('/api/captcha/generate');
    return {
      captchaToken: response.data.captchaToken,
      captchaImage: response.data.captchaImage,
    };
  } catch {
    console.warn('[Captcha] Backend API unreachable, using local fallback captcha');
    return generateFallbackCaptcha();
  }
}

export async function verifyBackendCaptcha(captchaToken: string, captchaCode: string): Promise<boolean> {
  try {
    const response = await api.post<{ success: boolean }>('/api/captcha/verify', {
      captchaToken,
      captchaCode,
    });
    return response.data.success;
  } catch {
    if (captchaToken.startsWith('FALLBACK:')) {
      const expectedCode = captchaToken.split(':')[1];
      return captchaCode.trim().toUpperCase() === expectedCode;
    }
    return false;
  }
}

/**
 * Call C# .NET Core ABDM M1 Generate Aadhaar OTP Endpoint
 */
export async function generateAadhaarOtp(encryptedAadhaar: string, captchaToken: string): Promise<GenerateOtpResponse> {
  const response = await api.post<{ success: boolean; txnId: string; message: string; maskedMobile?: string }>('/api/abha/generate-aadhaar-otp', {
    encryptedAadhaar,
    captchaToken,
  });

  return {
    txnId: response.data.txnId,
    message: response.data.message,
    maskedMobile: response.data.maskedMobile,
  };
}

/**
 * Fetch Public Key Certificate from Backend
 */
export async function fetchAndInitCert(): Promise<string> {
  const response = await api.get<{ success: boolean; publicKey: string }>('/api/abha/public-key');
  return response.data.publicKey;
}

/**
 * Call C# backend Download ABHA Card API
 */
export async function downloadAbhaCardApi(token?: string, abhaNumber?: string): Promise<Blob | null> {
  try {
    const response = await api.get('/api/abha/download-card', {
      params: { token, abhaNumber },
      responseType: 'blob',
    });
    return response.data;
  } catch (err) {
    console.warn('[ABHA Card API] Gateway unreachable or card unavailable:', err);
    return null;
  }
}

function generateFallbackCaptcha(): CaptchaData {
  const chars = '23456789ABCDEFGHJKLMNPQRSTUVWXYZ';
  let code = '';
  for (let i = 0; i < 6; i++) {
    code += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="160" height="44" viewBox="0 0 160 44"><rect width="100%" height="100%" fill="#f8fafc" rx="8"/><line x1="10" y1="10" x2="150" y2="34" stroke="#a78bfa" stroke-width="2" opacity="0.6"/><line x1="20" y1="36" x2="140" y2="8" stroke="#cbd5e1" stroke-width="2" opacity="0.6"/><text x="22" y="32" fill="#4c1d95" font-family="Courier New, monospace" font-size="24" font-weight="bold" letter-spacing="4">${code}</text></svg>`;
  const svgBase64 = `data:image/svg+xml;base64,${btoa(svg)}`;
  return {
    captchaToken: `FALLBACK:${code}`,
    captchaImage: svgBase64,
  };
}

export default api;
