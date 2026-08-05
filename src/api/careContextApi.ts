import api from './abhaApi';

export interface CareContextItem {
  referenceNumber: string;
  display: string;
  hiType: string;
}

export interface LinkCareContextPayload {
  patientId: number;
  abhaNumber: string;
  abhaAddress: string;
  careContexts: CareContextItem[];
}

export interface PatientCareContextRecord {
  careContextId: number;
  patientId: number;
  abhaNumber: string;
  abhaAddress: string;
  referenceNumber: string;
  display: string;
  hiType: string;
  status: string;
  requestId: string;
  createdAt: string;
}

export interface LinkCareContextResponse {
  success: boolean;
  requestId: string;
  message: string;
  status: string;
  rawGatewayResponse: string;
  linkedRecords: PatientCareContextRecord[];
}

export async function linkCareContextsApi(payload: LinkCareContextPayload): Promise<LinkCareContextResponse> {
  const response = await api.post<LinkCareContextResponse>('/api/carecontext/link', payload);
  return response.data;
}

export async function saveCareContextsApi(payload: LinkCareContextPayload): Promise<LinkCareContextResponse> {
  const response = await api.post<LinkCareContextResponse>('/api/carecontext/save', payload);
  return response.data;
}

export async function fetchPatientCareContextsApi(patientId: number, abhaAddress?: string): Promise<PatientCareContextRecord[]> {
  const response = await api.get<{ success: boolean; data: PatientCareContextRecord[] }>('/api/carecontext/list', {
    params: { patientId, abhaAddress },
  });
  return response.data.data;
}

export async function sendDeepLinkSmsApi(mobile: string, abhaAddress: string, patientName: string): Promise<boolean> {
  const response = await api.post<{ success: boolean; message: string }>('/api/carecontext/send-sms', {
    mobile,
    abhaAddress,
    patientName,
  });
  return response.data.success;
}

export async function checkLinkStatusApi(requestId: string): Promise<string> {
  const response = await api.get<{ success: boolean; rawResponse: string }>(`/api/carecontext/status/${requestId}`);
  return response.data.rawResponse;
}

// ==========================================
// === M2 HIU: CONSENT & HEALTH INFO ===
// ==========================================

export interface ConsentInitPayload {
  patientAbhaAddress: string;
  purposeCode: string; // CAREMGT, BTG, PUBHLTH, HPAYMT, DSRCH, PATRQT
  hiTypes: string[];   // OPConsultation, Prescription, DiagnosticReport, DischargeSummary, etc.
  dateFrom: string;    // ISO8601
  dateTo: string;
  eraseAt: string;
}

export interface ConsentInitResponse {
  success: boolean;
  requestId: string;
  message: string;
  rawResponse: string;
}

export interface FetchHealthInfoPayload {
  consentId: string;
  dateFrom: string;
  dateTo: string;
}

export interface FetchHealthInfoResponse {
  success: boolean;
  requestId: string;
  message: string;
  rawResponse: string;
}

export async function initiateConsentApi(payload: ConsentInitPayload): Promise<ConsentInitResponse> {
  const response = await api.post<ConsentInitResponse>('/api/carecontext/consent-init', payload);
  return response.data;
}

export async function getConsentStatusApi(requestId: string): Promise<string> {
  const response = await api.get<{ success: boolean; rawResponse: string }>(`/api/carecontext/consent-status/${requestId}`);
  return response.data.rawResponse;
}

export async function fetchHealthInfoApi(payload: FetchHealthInfoPayload): Promise<FetchHealthInfoResponse> {
  const response = await api.post<FetchHealthInfoResponse>('/api/carecontext/health-info/fetch', payload);
  return response.data;
}

export async function getHealthInfoStatusApi(requestId: string): Promise<string> {
  const response = await api.get<{ success: boolean; rawResponse: string }>(`/api/carecontext/health-info/status/${requestId}`);
  return response.data.rawResponse;
}

// ==========================================
// === M3: SUBSCRIPTION ===
// ==========================================

export interface SubscriptionInitPayload {
  patientAbhaAddress: string;
  purposeCode: string;
  categories: string[];  // "LINK", "DATA"
  dateFrom: string;
  dateTo: string;
}

export interface SubscriptionInitResponse {
  success: boolean;
  requestId: string;
  message: string;
  rawResponse: string;
}

export async function initiateSubscriptionApi(payload: SubscriptionInitPayload): Promise<SubscriptionInitResponse> {
  const response = await api.post<SubscriptionInitResponse>('/api/carecontext/subscription-init', payload);
  return response.data;
}

export async function getSubscriptionStatusApi(requestId: string): Promise<string> {
  const response = await api.get<{ success: boolean; rawResponse: string }>(`/api/carecontext/subscription-status/${requestId}`);
  return response.data.rawResponse;
}
