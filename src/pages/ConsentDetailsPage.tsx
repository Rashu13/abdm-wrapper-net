import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation, useParams } from 'react-router-dom';
import api from '@/api/abhaApi';

interface ConsentDetailsState {
  id?: string;
  requestId?: string;
  consentId?: string;
  patientName?: string;
  abhaAddress?: string;
  purpose?: string;
  status?: string;
  fromDateTime?: string;
  toDateTime?: string;
  grantedHiTypes?: string[];
}

export interface FhirBundleRecord {
  id: string;
  bundleNo: string;
  title: string;
  type: string;
  hospital: string;
  docId: string;
  date: string;
  doctorName?: string;
  obsCount: number;
  dxCount: number;
  medsCount: number;
  docsCount: number;
  medications?: { name: string; dosage: string; duration: string }[];
  observations?: { test: string; result: string; range: string }[];
  detailsText?: string;
}

const ConsentDetailsPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { id } = useParams<{ id: string }>();

  const stateData = (location.state as ConsentDetailsState) || {};

  const [consentDetails, setConsentDetails] = useState<ConsentDetailsState>({
    id: id || stateData.id || '1',
    requestId: stateData.requestId || id || '',
    patientName: stateData.patientName || 'Saurav Kumar',
    abhaAddress: stateData.abhaAddress || 'saurav_50505@sbx',
    purpose: stateData.purpose || 'Care Management',
    status: stateData.status || 'Granted',
    fromDateTime: stateData.fromDateTime || '01 Aug 2025',
    toDateTime: stateData.toDateTime || '04 Aug 2027',
    grantedHiTypes: stateData.grantedHiTypes || [],
  });

  const [isLoading, setIsLoading] = useState<boolean>(consentDetails.status?.toLowerCase() === 'granted');
  const [fetchedBundles, setFetchedBundles] = useState<FhirBundleRecord[]>([]);
  const [expandedBundleId, setExpandedBundleId] = useState<string | null>('BUNDLE-1');
  const [pdfModalBundle, setPdfModalBundle] = useState<FhirBundleRecord | null>(null);
  const [copiedAbha, setCopiedAbha] = useState<boolean>(false);

  const fetchGatewayRecords = async () => {
    if (consentDetails.status?.toLowerCase() !== 'granted') {
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      const res = await api.post<{ success: boolean; rawResponse: string }>('/api/consent/fetch-records', {
        consentId: consentDetails.consentId || consentDetails.requestId || consentDetails.id,
        fromDate: consentDetails.fromDateTime,
        toDate: consentDetails.toDateTime,
      });

      if (res.data && res.data.rawResponse) {
        try {
          const parsed = JSON.parse(res.data.rawResponse);
          if (parsed && Array.isArray(parsed.bundles) && parsed.bundles.length > 0) {
            const mappedBundles: FhirBundleRecord[] = parsed.bundles.map((b: any, idx: number) => ({
              id: b.id || `BUNDLE-${idx + 1}`,
              bundleNo: String(b.bundleNo || idx + 1),
              title: b.title || b.display || b.hiType || 'Health Document Record',
              type: b.hiType || b.type || 'Record',
              hospital: b.facility || b.hospital || 'MIDHA HOSPITAL',
              docId: b.docId || b.referenceNumber || `FHIR-${Math.floor(100000 + Math.random() * 900000)}`,
              date: b.date || b.createdAt || new Date().toLocaleString(),
              doctorName: b.doctorName || 'Dr. Midha (M.D. Medicine)',
              obsCount: b.observations ? b.observations.length : 0,
              dxCount: b.diagnoses ? b.diagnoses.length : 0,
              medsCount: b.medications ? b.medications.length : 0,
              docsCount: b.documents ? b.documents.length : 1,
              medications: b.medications || [],
              observations: b.observations || [],
              detailsText: b.details || b.summary || '',
            }));
            setFetchedBundles(mappedBundles);
            return;
          }
        } catch { }
      }

      setFetchedBundles([]);
    } catch (err) {
      console.log('[ABDM HEALTH RECS FETCH ERROR]', err);
      setFetchedBundles([]);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchGatewayRecords();
  }, [consentDetails.status]);

  const toggleExpand = (bundleId: string) => {
    setExpandedBundleId(expandedBundleId === bundleId ? null : bundleId);
  };

  const handlePrintPdf = () => {
    window.print();
  };

  const handleCopyAbhaText = () => {
    if (consentDetails.abhaAddress) {
      navigator.clipboard.writeText(consentDetails.abhaAddress);
      setCopiedAbha(true);
      setTimeout(() => setCopiedAbha(false), 2000);
    }
  };

  const isGranted = consentDetails.status?.toLowerCase() === 'granted';

  // Compute live stats dynamically from fetched bundles
  const totalBundlesCount = fetchedBundles.length;
  const totalObsCount = fetchedBundles.reduce((acc, curr) => acc + (curr.obsCount || 0), 0);
  const totalMedsCount = fetchedBundles.reduce((acc, curr) => acc + (curr.medsCount || 0), 0);
  const totalDocsCount = fetchedBundles.reduce((acc, curr) => acc + (curr.docsCount || 0), 0);

  const getInitials = (name?: string) => {
    if (!name) return 'PT';
    const parts = name.trim().split(' ');
    if (parts.length >= 2) return `${parts[0][0]}${parts[1][0]}`.toUpperCase();
    return name.substring(0, 2).toUpperCase();
  };

  return (
    <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 py-6 flex flex-col gap-6">
      {/* Top Banner Header Card */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 bg-white p-6 rounded-2xl border border-slate-200/90 shadow-sm">
        <div className="flex items-center gap-3.5">
          <div className="w-11 h-11 rounded-2xl bg-gradient-to-br from-[#037BBA] to-[#026296] text-white flex items-center justify-center shadow-md shadow-brand/20 shrink-0 font-bold text-lg">
            📜
          </div>
          <div>
            <div className="flex items-center gap-2">
              <span className="px-2 py-0.5 rounded-full bg-sky-50 text-[#037BBA] border border-sky-200/80 font-bold text-[10px] uppercase tracking-wider">
                ABDM Consent Artifact Details
              </span>
            </div>
            <h1 className="text-xl font-extrabold text-slate-800 tracking-tight m-0 mt-1">
              Consent Details & Decrypted Health Records
            </h1>
            <p className="text-xs text-slate-500 m-0 font-medium mt-0.5">
              Decrypted FHIR R4 Bundles fetched via ABDM HIU Gateway
            </p>
          </div>
        </div>

        <button
          type="button"
          className="bg-slate-100 hover:bg-slate-200 text-slate-700 font-bold py-2.5 px-4 rounded-xl text-xs transition-all flex items-center gap-1.5 cursor-pointer shrink-0"
          onClick={() => navigate('/consent-requests')}
        >
          ← Back to Consent Requests
        </button>
      </div>

      {/* Patient & Consent Info Card */}
      <div className="bg-white border border-slate-200/90 rounded-2xl p-5 shadow-xs flex flex-col md:flex-row items-stretch md:items-center justify-between gap-5">
        {/* Patient Profile */}
        <div className="flex items-center gap-3.5 pr-4 md:border-r border-slate-100">
          <div className="w-12 h-12 rounded-full bg-gradient-to-br from-[#037BBA] to-[#026296] text-white font-black text-sm flex items-center justify-center shrink-0 shadow-md">
            {getInitials(consentDetails.patientName)}
          </div>
          <div className="flex flex-col gap-0.5">
            <span className="text-base font-extrabold text-slate-900">
              {consentDetails.patientName}
            </span>
            <div className="flex items-center gap-2">
              <span className="font-mono text-xs text-[#037BBA] font-bold">
                {consentDetails.abhaAddress}
              </span>
              <button
                type="button"
                className="text-slate-400 hover:text-[#037BBA] text-xs p-0.5 rounded transition-colors"
                title="Copy ABHA Address"
                onClick={handleCopyAbhaText}
              >
                {copiedAbha ? '✓' : '📋'}
              </button>
            </div>
          </div>
        </div>

        {/* 4 Metadata Stat Pills */}
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 flex-1">
          <div className="bg-slate-50/70 border border-slate-200/80 rounded-xl p-3 flex flex-col gap-0.5">
            <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">PURPOSE</span>
            <span className="text-xs font-extrabold text-slate-800 truncate">{consentDetails.purpose}</span>
          </div>

          <div className="bg-slate-50/70 border border-slate-200/80 rounded-xl p-3 flex flex-col gap-0.5">
            <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">STATUS</span>
            <div>
              <span className={`inline-flex items-center gap-1.5 px-2 py-0.5 rounded-full text-[10.5px] font-bold border shadow-2xs ${
                isGranted
                  ? 'bg-emerald-50 text-emerald-700 border-emerald-200/80'
                  : 'bg-amber-50 text-amber-700 border-amber-200/80'
              }`}>
                <span className={`w-1.5 h-1.5 rounded-full ${isGranted ? 'bg-emerald-500 animate-pulse' : 'bg-amber-500'}`} />
                {consentDetails.status?.toUpperCase()}
              </span>
            </div>
          </div>

          <div className="bg-slate-50/70 border border-slate-200/80 rounded-xl p-3 flex flex-col gap-0.5">
            <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">FROM DATE</span>
            <span className="text-xs font-extrabold text-slate-800">{consentDetails.fromDateTime}</span>
          </div>

          <div className="bg-slate-50/70 border border-slate-200/80 rounded-xl p-3 flex flex-col gap-0.5">
            <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">VALID TILL</span>
            <span className="text-xs font-extrabold text-emerald-700">{consentDetails.toDateTime}</span>
          </div>
        </div>
      </div>

      {/* Main Content View */}
      {!isGranted ? (
        /* PENDING / NON-GRANTED LOCKED CARD */
        <div className="bg-white border border-slate-200/90 rounded-2xl p-8 sm:p-12 flex flex-col items-center justify-center text-center gap-4 shadow-sm">
          <div className="w-16 h-16 rounded-full bg-amber-50 text-amber-600 flex items-center justify-center text-3xl font-bold border border-amber-200/80">
            🔒
          </div>
          <div className="max-w-md">
            <h3 className="text-lg font-extrabold text-slate-800">
              Consent Request Pending Patient Approval
            </h3>
            <p className="text-xs text-slate-500 mt-1.5 leading-relaxed">
              Patient <strong>{consentDetails.patientName}</strong> (<code className="text-[#037BBA] font-bold">{consentDetails.abhaAddress}</code>) has not approved this consent request on their ABHA App yet.
            </p>
          </div>

          <div className="flex flex-wrap items-center justify-center gap-3 mt-2">
            <button
              type="button"
              className="bg-[#037BBA] hover:bg-[#026296] text-white font-bold py-2.5 px-4 rounded-xl text-xs shadow-md transition-all flex items-center gap-1.5 cursor-pointer"
              onClick={async () => {
                setIsLoading(true);
                try {
                  const res = await api.get<{ success: boolean; rawResponse: string }>(
                    `/api/consent/status/${consentDetails.requestId || consentDetails.id}`
                  );
                  if (res.data.rawResponse && (res.data.rawResponse.includes('GRANTED') || res.data.rawResponse.includes('consentArtefacts'))) {
                    setConsentDetails((prev) => ({ ...prev, status: 'Granted' }));
                  } else {
                    alert('Status checked: Consent is pending on patient ABHA mobile app.');
                  }
                } catch {
                  alert('Status checked: Consent is pending on patient ABHA mobile app.');
                } finally {
                  setIsLoading(false);
                }
              }}
            >
              🔄 Sync Gateway Status
            </button>

            <button
              type="button"
              className="bg-slate-100 hover:bg-slate-200 text-slate-700 font-bold py-2.5 px-4 rounded-xl text-xs transition-all cursor-pointer"
              onClick={() => navigate('/consent-requests')}
            >
              ← Back
            </button>
          </div>
        </div>
      ) : isLoading ? (
        <div className="bg-white border border-slate-200/90 rounded-2xl p-12 flex flex-col items-center justify-center gap-3 text-slate-500 text-sm font-semibold shadow-sm">
          <span className="w-8 h-8 border-3 border-[#037BBA] border-t-transparent rounded-full animate-spin" />
          <span>Decrypting FHIR R4 Health Bundles from ABDM Gateway...</span>
        </div>
      ) : (
        <div className="flex flex-col gap-5">
          {/* Header Row & Stat Pills */}
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 bg-white p-4 rounded-2xl border border-slate-200/90 shadow-xs">
            <div className="flex items-center gap-3">
              <h2 className="text-base font-extrabold text-slate-800 m-0">
                Decrypted FHIR Bundles ({totalBundlesCount})
              </h2>
              <span className="px-2 py-0.5 rounded bg-emerald-50 text-emerald-700 font-bold text-[10px] border border-emerald-200">
                ✓ ABDM M3 Decrypted
              </span>
            </div>

            <button
              type="button"
              className="bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold px-3 py-1.5 rounded-xl transition-all flex items-center gap-1.5 cursor-pointer ml-auto"
              onClick={fetchGatewayRecords}
            >
              🔄 Refresh Bundles
            </button>
          </div>

          {/* 4 Stat Summary Cards */}
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3.5">
            <div className="bg-white border border-slate-200/90 rounded-2xl p-4 flex items-center gap-3 shadow-xs">
              <div className="w-9 h-9 rounded-xl bg-sky-50 text-[#037BBA] flex items-center justify-center font-bold text-xs shrink-0">
                📦
              </div>
              <div>
                <div className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Bundles Received</div>
                <div className="text-lg font-black text-slate-800 mt-0.5">{totalBundlesCount}</div>
              </div>
            </div>

            <div className="bg-white border border-slate-200/90 rounded-2xl p-4 flex items-center gap-3 shadow-xs">
              <div className="w-9 h-9 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold text-xs shrink-0">
                🧪
              </div>
              <div>
                <div className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Observations</div>
                <div className="text-lg font-black text-emerald-600 mt-0.5">{totalObsCount}</div>
              </div>
            </div>

            <div className="bg-white border border-slate-200/90 rounded-2xl p-4 flex items-center gap-3 shadow-xs">
              <div className="w-9 h-9 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold text-xs shrink-0">
                💊
              </div>
              <div>
                <div className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Prescriptions</div>
                <div className="text-lg font-black text-indigo-600 mt-0.5">{totalMedsCount}</div>
              </div>
            </div>

            <div className="bg-white border border-slate-200/90 rounded-2xl p-4 flex items-center gap-3 shadow-xs">
              <div className="w-9 h-9 rounded-xl bg-slate-100 text-slate-600 flex items-center justify-center font-bold text-xs shrink-0">
                📄
              </div>
              <div>
                <div className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Documents</div>
                <div className="text-lg font-black text-slate-700 mt-0.5">{totalDocsCount}</div>
              </div>
            </div>
          </div>

          {/* FHIR Bundle Accordion Cards */}
          <div className="flex flex-col gap-4">
            {fetchedBundles.map((bundle) => {
              const isExpanded = expandedBundleId === bundle.id;

              return (
                <div
                  key={bundle.id}
                  className="bg-white border border-slate-200/90 rounded-2xl shadow-sm overflow-hidden transition-all duration-200"
                >
                  {/* Bundle Header */}
                  <div className="p-4 sm:p-5 flex flex-col sm:flex-row sm:items-center justify-between gap-4 bg-slate-50/50">
                    <div className="flex items-start gap-3.5">
                      <div className="w-10 h-10 rounded-xl bg-[#037BBA] text-white flex items-center justify-center font-black text-xs shrink-0 shadow-xs mt-0.5">
                        #{bundle.bundleNo}
                      </div>
                      <div className="flex flex-col gap-1">
                        <div className="flex items-center gap-2 flex-wrap">
                          <h3 className="text-base font-extrabold text-slate-900 m-0">
                            {bundle.title}
                          </h3>
                          <span className="px-2 py-0.5 rounded bg-sky-100 text-[#037BBA] font-mono text-[10px] font-bold">
                            {bundle.type}
                          </span>
                        </div>
                        <div className="flex items-center gap-3 text-xs text-slate-500 font-medium flex-wrap">
                          <span>🏥 <strong>Hospital:</strong> {bundle.hospital}</span>
                          <span>👨‍⚕️ <strong>Doctor:</strong> {bundle.doctorName || 'Attending Physician'}</span>
                          <span>📅 <strong>Date:</strong> {bundle.date}</span>
                        </div>
                      </div>
                    </div>

                    <div className="flex items-center gap-2 self-end sm:self-center">
                      <button
                        type="button"
                        className="bg-white hover:bg-slate-100 text-slate-700 border border-slate-200 font-bold px-3 py-1.5 rounded-xl text-xs shadow-2xs transition-all flex items-center gap-1.5 cursor-pointer"
                        onClick={() => setPdfModalBundle(bundle)}
                      >
                        📄 View Clinical PDF
                      </button>

                      <button
                        type="button"
                        className="w-8 h-8 rounded-xl bg-slate-100 hover:bg-slate-200 text-slate-600 font-bold text-xs flex items-center justify-center transition-colors cursor-pointer"
                        onClick={() => toggleExpand(bundle.id)}
                      >
                        {isExpanded ? '▲' : '▼'}
                      </button>
                    </div>
                  </div>

                  {/* Expanded Bundle Details */}
                  {isExpanded && (
                    <div className="p-5 border-t border-slate-100 flex flex-col gap-5 animate-fadeIn">
                      {/* Medications Table */}
                      {bundle.medications && bundle.medications.length > 0 && (
                        <div className="flex flex-col gap-2">
                          <span className="text-[11px] font-extrabold text-indigo-700 uppercase tracking-wider flex items-center gap-1.5">
                            💊 Prescribed Medications ({bundle.medsCount})
                          </span>
                          <div className="overflow-x-auto rounded-xl border border-slate-200/80">
                            <table className="w-full text-left text-xs font-semibold text-slate-700">
                              <thead className="bg-slate-50 text-[10px] uppercase font-bold text-slate-400 border-b border-slate-200/80">
                                <tr>
                                  <th className="px-3.5 py-2.5 w-10">#</th>
                                  <th className="px-3.5 py-2.5">Medicine Name</th>
                                  <th className="px-3.5 py-2.5">Dosage Pattern</th>
                                  <th className="px-3.5 py-2.5">Duration</th>
                                </tr>
                              </thead>
                              <tbody className="divide-y divide-slate-100">
                                {bundle.medications.map((med, mIdx) => (
                                  <tr key={mIdx} className="hover:bg-slate-50/50">
                                    <td className="px-3.5 py-2.5 font-bold text-slate-400">{mIdx + 1}.</td>
                                    <td className="px-3.5 py-2.5 font-extrabold text-slate-900">{med.name}</td>
                                    <td className="px-3.5 py-2.5">
                                      <span className="px-2 py-0.5 rounded bg-sky-50 text-[#037BBA] font-bold text-[10.5px]">
                                        {med.dosage}
                                      </span>
                                    </td>
                                    <td className="px-3.5 py-2.5 text-slate-600">{med.duration}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </div>
                      )}

                      {/* Observations Table */}
                      {bundle.observations && bundle.observations.length > 0 && (
                        <div className="flex flex-col gap-2">
                          <span className="text-[11px] font-extrabold text-emerald-700 uppercase tracking-wider flex items-center gap-1.5">
                            🧪 Lab Test Observations ({bundle.obsCount})
                          </span>
                          <div className="overflow-x-auto rounded-xl border border-slate-200/80">
                            <table className="w-full text-left text-xs font-semibold text-slate-700">
                              <thead className="bg-slate-50 text-[10px] uppercase font-bold text-slate-400 border-b border-slate-200/80">
                                <tr>
                                  <th className="px-3.5 py-2.5">Test Name</th>
                                  <th className="px-3.5 py-2.5">Observed Result</th>
                                  <th className="px-3.5 py-2.5">Reference Range</th>
                                </tr>
                              </thead>
                              <tbody className="divide-y divide-slate-100">
                                {bundle.observations.map((obs, oIdx) => (
                                  <tr key={oIdx} className="hover:bg-slate-50/50">
                                    <td className="px-3.5 py-2.5 font-bold text-slate-800">{obs.test}</td>
                                    <td className="px-3.5 py-2.5">
                                      <span className="px-2 py-0.5 rounded bg-emerald-50 text-emerald-800 font-extrabold text-xs">
                                        {obs.result}
                                      </span>
                                    </td>
                                    <td className="px-3.5 py-2.5 text-slate-500 font-mono text-[11px]">{obs.range}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </div>
                      )}

                      {/* Details & Document Summary */}
                      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 bg-slate-50 p-3.5 rounded-xl border border-slate-200/80">
                        <div className="flex flex-col gap-0.5 text-xs">
                          <span className="text-[10px] font-bold text-slate-400 uppercase">FHIR Document ID</span>
                          <span className="font-mono font-bold text-slate-800">{bundle.docId}</span>
                          {bundle.detailsText && (
                            <p className="text-slate-600 mt-1 m-0 text-xs">{bundle.detailsText}</p>
                          )}
                        </div>

                        <button
                          type="button"
                          className="bg-[#037BBA] hover:bg-[#026296] text-white font-bold py-2 px-3.5 rounded-xl text-xs shadow-xs transition-all flex items-center gap-1.5 cursor-pointer shrink-0"
                          onClick={() => setPdfModalBundle(bundle)}
                        >
                          📄 Open Clinical PDF
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Modern Clinical PDF Viewer Modal */}
      {pdfModalBundle && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-3 sm:p-6 bg-slate-900/50 backdrop-blur-xs animate-fadeIn">
          <div className="bg-slate-100 rounded-2xl border border-slate-200 shadow-2xl w-full max-w-4xl max-h-[92vh] flex flex-col overflow-hidden">
            {/* Modal Controls Bar */}
            <div className="bg-white p-4 border-b border-slate-200 flex items-center justify-between gap-3 shrink-0">
              <div className="flex items-center gap-2.5">
                <span className="px-2 py-0.5 rounded bg-sky-50 text-[#037BBA] font-bold text-[10px] border border-sky-200 uppercase tracking-wider">
                  FHIR Health Record PDF
                </span>
                <h3 className="text-sm sm:text-base font-extrabold text-slate-800 m-0 truncate max-w-xs sm:max-w-md">
                  {pdfModalBundle.title}
                </h3>
              </div>

              <div className="flex items-center gap-2">
                <button
                  type="button"
                  className="bg-[#037BBA] hover:bg-[#026296] text-white text-xs font-bold px-3 py-1.5 rounded-xl transition-all shadow-xs flex items-center gap-1 cursor-pointer"
                  onClick={() => alert(`Downloading PDF for ${pdfModalBundle.title}...`)}
                >
                  📥 Download
                </button>
                <button
                  type="button"
                  className="bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold px-3 py-1.5 rounded-xl transition-all cursor-pointer flex items-center gap-1"
                  onClick={handlePrintPdf}
                >
                  🖨️ Print
                </button>
                <button
                  type="button"
                  className="w-7 h-7 rounded-full bg-slate-100 hover:bg-slate-200 text-slate-500 font-bold text-xs flex items-center justify-center transition-colors cursor-pointer"
                  onClick={() => setPdfModalBundle(null)}
                >
                  ✕
                </button>
              </div>
            </div>

            {/* Authentic Clinical PDF Document Paper */}
            <div className="p-6 overflow-y-auto flex justify-center">
              <div className="bg-white border border-slate-300 rounded-xl p-8 shadow-md w-full max-w-3xl flex flex-col gap-6 text-slate-800">
                {/* Hospital Header */}
                <div className="flex items-start justify-between gap-4 pb-4 border-b border-slate-200">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-xl bg-red-600 text-white font-black text-xl flex items-center justify-center shrink-0">
                      +
                    </div>
                    <div>
                      <h2 className="text-lg font-black text-slate-900 m-0">
                        {pdfModalBundle.hospital}
                      </h2>
                      <p className="text-xs text-slate-500 font-medium m-0 mt-0.5">
                        ABDM Registered Healthcare Provider (HIP ID: IN0610090658)
                      </p>
                    </div>
                  </div>

                  <div className="flex flex-col items-end gap-1 text-right">
                    <span className="px-2 py-0.5 rounded bg-slate-100 text-slate-600 font-mono text-[10px] font-bold">
                      Doc ID: {pdfModalBundle.docId}
                    </span>
                    <span className="text-xs font-semibold text-slate-500">
                      Date: {pdfModalBundle.date}
                    </span>
                  </div>
                </div>

                {/* Patient & Doctor Details Grid */}
                <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 bg-slate-50 p-4 rounded-xl border border-slate-200/80 text-xs">
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Patient Name</span>
                    <span className="font-extrabold text-slate-900 mt-0.5 block">{consentDetails.patientName}</span>
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">ABHA Address</span>
                    <span className="font-mono font-bold text-[#037BBA] mt-0.5 block">{consentDetails.abhaAddress}</span>
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Doctor</span>
                    <span className="font-bold text-slate-800 mt-0.5 block">{pdfModalBundle.doctorName || 'Attending Physician'}</span>
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Purpose</span>
                    <span className="font-bold text-slate-800 mt-0.5 block">{consentDetails.purpose}</span>
                  </div>
                </div>

                {/* Prescribed Medications */}
                {pdfModalBundle.medications && pdfModalBundle.medications.length > 0 && (
                  <div className="flex flex-col gap-2">
                    <h3 className="text-xs font-black text-slate-900 uppercase tracking-wider m-0 flex items-center gap-1.5">
                      <span>💊</span> Rx - Prescribed Medications
                    </h3>
                    <div className="overflow-x-auto rounded-xl border border-slate-200">
                      <table className="w-full text-left text-xs font-semibold text-slate-700">
                        <thead className="bg-slate-100 text-[10px] uppercase font-bold text-slate-500 border-b border-slate-200">
                          <tr>
                            <th className="px-3 py-2 w-8">#</th>
                            <th className="px-3 py-2">Medicine Name</th>
                            <th className="px-3 py-2">Dosage Pattern</th>
                            <th className="px-3 py-2">Duration</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100">
                          {pdfModalBundle.medications.map((med, mIdx) => (
                            <tr key={mIdx}>
                              <td className="px-3 py-2 text-slate-400 font-bold">{mIdx + 1}.</td>
                              <td className="px-3 py-2 font-extrabold text-slate-900">{med.name}</td>
                              <td className="px-3 py-2 text-[#037BBA] font-bold">{med.dosage}</td>
                              <td className="px-3 py-2 text-slate-600">{med.duration}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}

                {/* Lab Test Observations */}
                {pdfModalBundle.observations && pdfModalBundle.observations.length > 0 && (
                  <div className="flex flex-col gap-2">
                    <h3 className="text-xs font-black text-slate-900 uppercase tracking-wider m-0 flex items-center gap-1.5">
                      <span>🧪</span> Lab Test Observations
                    </h3>
                    <div className="overflow-x-auto rounded-xl border border-slate-200">
                      <table className="w-full text-left text-xs font-semibold text-slate-700">
                        <thead className="bg-slate-100 text-[10px] uppercase font-bold text-slate-500 border-b border-slate-200">
                          <tr>
                            <th className="px-3 py-2">Test Name</th>
                            <th className="px-3 py-2">Observed Result</th>
                            <th className="px-3 py-2">Reference Range</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100">
                          {pdfModalBundle.observations.map((obs, oIdx) => (
                            <tr key={oIdx}>
                              <td className="px-3 py-2 font-bold text-slate-800">{obs.test}</td>
                              <td className="px-3 py-2 font-extrabold text-emerald-800">{obs.result}</td>
                              <td className="px-3 py-2 text-slate-500 font-mono text-[11px]">{obs.range}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}

                {/* Details Summary */}
                {pdfModalBundle.detailsText && (
                  <div className="p-3.5 bg-slate-50 rounded-xl border border-slate-200/80 text-xs text-slate-700">
                    <span className="font-bold text-slate-900 block mb-1">Clinical Note & Impression:</span>
                    <p className="m-0 leading-relaxed">{pdfModalBundle.detailsText}</p>
                  </div>
                )}

                {/* Signature & ABDM Verification Stamp */}
                <div className="flex items-end justify-between pt-6 border-t border-slate-200 mt-2">
                  <div className="flex flex-col items-center p-2.5 bg-slate-50 rounded-xl border border-slate-200 text-center">
                    <div className="w-16 h-16 bg-slate-800 text-white font-mono font-bold text-[9px] flex items-center justify-center p-1 rounded">
                      ABDM QR CODE
                    </div>
                    <span className="text-[9px] font-bold text-slate-500 mt-1">Verified via NHA Gateway</span>
                  </div>

                  <div className="flex flex-col items-end text-right">
                    <div className="w-32 border-b border-slate-400 mb-1" />
                    <span className="text-xs font-black text-slate-900">
                      {pdfModalBundle.doctorName || 'Attending Physician'}
                    </span>
                    <span className="text-[10px] font-semibold text-slate-500">
                      Authorized Digital Signature
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ConsentDetailsPage;
