import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '@/api/abhaApi';

interface PatientItem {
  patientId: number;
  uhid: string;
  abhaNumber: string;
  name: string;
  gender?: string;
  dob?: string;
  mobile?: string;
  address?: string;
  city?: string;
  state?: string;
  pincode?: string;
  rawPayloadJson?: string;
  createdAt: string;
  abhaAddresses: string[];
}

const PatientsListPage: React.FC = () => {
  const navigate = useNavigate();
  const [patients, setPatients] = useState<PatientItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [error, setError] = useState<string | null>(null);
  const [copiedIndex, setCopiedIndex] = useState<string | null>(null);

  const fetchPatients = async () => {
    try {
      setLoading(true);
      setError(null);
      const res = await api.get<{ success: boolean; data: PatientItem[] }>('/api/patient/list');
      if (res.data.success) {
        setPatients(res.data.data);
      }
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { message?: string } } };
      setError(axiosError?.response?.data?.message || 'Failed to load patients from database.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPatients();
  }, []);

  const handleCopy = (text: string, id: string) => {
    navigator.clipboard.writeText(text);
    setCopiedIndex(id);
    setTimeout(() => setCopiedIndex(null), 2000);
  };

  const filteredPatients = patients.filter((p) => {
    const q = searchQuery.toLowerCase().trim();
    if (!q) return true;
    return (
      (p.uhid && p.uhid.toLowerCase().includes(q)) ||
      (p.name && p.name.toLowerCase().includes(q)) ||
      (p.abhaNumber && p.abhaNumber.toLowerCase().includes(q)) ||
      (p.mobile && p.mobile.includes(q)) ||
      (p.gender && p.gender.toLowerCase().includes(q)) ||
      (p.dob && p.dob.toLowerCase().includes(q)) ||
      (p.address && p.address.toLowerCase().includes(q)) ||
      (p.pincode && p.pincode.toLowerCase().includes(q)) ||
      (p.city && p.city.toLowerCase().includes(q)) ||
      (p.state && p.state.toLowerCase().includes(q)) ||
      p.abhaAddresses.some((addr) => addr.toLowerCase().includes(q))
    );
  });

  return (
    <div className="w-full max-w-7xl mx-auto px-4 sm:px-6 py-6 flex flex-col gap-6">
      {/* Top Header Bar */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 bg-white p-6 rounded-2xl border border-slate-200/90 shadow-sm">
        <div className="flex items-center gap-3.5">
          <div className="w-11 h-11 rounded-2xl bg-gradient-to-br from-[#037BBA] to-[#026296] text-white flex items-center justify-center shadow-md shadow-brand/20 shrink-0">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
              <circle cx="9" cy="7" r="4" />
              <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
              <path d="M16 3.13a4 4 0 0 1 0 7.75" />
            </svg>
          </div>
          <div>
            <h1 className="text-xl font-extrabold text-slate-800 tracking-tight m-0">Registered Patients</h1>
            <p className="text-xs text-slate-500 m-0 font-medium mt-0.5">
              Live database records & linked ABHA Addresses ({patients.length} Registered)
            </p>
          </div>
        </div>

        <button
          type="button"
          className="text-white font-bold py-2.5 px-5 rounded-xl text-xs shadow-md transition-all duration-200 flex items-center justify-center gap-2 cursor-pointer hover:opacity-95 shrink-0"
          style={{ background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
          onClick={() => navigate('/abha/create')}
        >
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <line x1="12" y1="5" x2="12" y2="19" />
            <line x1="5" y1="12" x2="19" y2="12" />
          </svg>
          Create New ABHA
        </button>
      </div>

      {/* Search Input Bar */}
      <div className="relative flex items-center">
        <div className="absolute left-4 text-slate-400">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="11" cy="11" r="8" />
            <line x1="21" y1="21" x2="16.65" y2="16.65" />
          </svg>
        </div>
        <input
          type="text"
          className="w-full bg-white border border-slate-200 rounded-xl pl-11 pr-10 py-3 text-sm text-slate-800 placeholder-slate-400 font-semibold focus:outline-none focus:border-brand focus:ring-2 focus:ring-brand/10 transition-all shadow-sm"
          placeholder="Filter by Name, UHID, ABHA Number, Mobile, Address, Pincode..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
        {searchQuery && (
          <button
            type="button"
            className="absolute right-4 text-slate-400 hover:text-slate-600 font-bold text-xs"
            onClick={() => setSearchQuery('')}
          >
            ✕
          </button>
        )}
      </div>

      {/* Error Toast */}
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm font-semibold flex items-center justify-between gap-3 shadow-sm">
          <span className="text-xs md:text-sm">{error}</span>
          <button type="button" className="text-red-500 hover:text-red-700 font-bold" onClick={() => setError(null)}>✕</button>
        </div>
      )}

      {/* Sleek Patients Table Card */}
      <div className="bg-white border border-slate-200/90 rounded-2xl shadow-sm overflow-hidden">
        {loading ? (
          <div className="p-12 flex flex-col items-center justify-center gap-3 text-slate-500 text-sm font-semibold">
            <span className="w-6 h-6 border-2 border-brand border-t-transparent rounded-full animate-spin" />
            <span>Loading registered patients from database...</span>
          </div>
        ) : filteredPatients.length === 0 ? (
          <div className="p-12 text-center flex flex-col items-center justify-center gap-2">
            <div className="w-12 h-12 rounded-full bg-slate-100 text-slate-400 flex items-center justify-center mb-2">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <circle cx="12" cy="12" r="10" /><line x1="8" y1="12" x2="16" y2="12" />
              </svg>
            </div>
            <p className="text-base font-bold text-slate-800 m-0">
              {searchQuery ? 'No matching patients found' : 'No Patients Registered Yet'}
            </p>
            <p className="text-xs text-slate-500 m-0 font-medium">
              {searchQuery ? 'Try searching with another keyword or clearing filters' : 'Create an ABHA number and click Register Patient to view records here.'}
            </p>
            {!searchQuery && (
              <button
                type="button"
                className="mt-3 text-white font-bold py-2.5 px-5 rounded-xl text-xs shadow-md transition-all cursor-pointer"
                style={{ background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
                onClick={() => navigate('/abha/create')}
              >
                Create ABHA & Register Patient
              </button>
            )}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-slate-50/80 border-b border-slate-200/80 text-[11px] font-extrabold text-slate-500 uppercase tracking-wider">
                  <th className="py-3.5 px-4 font-extrabold">Patient Details</th>
                  <th className="py-3.5 px-4 font-extrabold">Demographics</th>
                  <th className="py-3.5 px-4 font-extrabold">ABHA Number</th>
                  <th className="py-3.5 px-4 font-extrabold">Linked ABHA Address</th>
                  <th className="py-3.5 px-4 font-extrabold">Mobile</th>
                  <th className="py-3.5 px-4 font-extrabold">Location / Address</th>
                  <th className="py-3.5 px-4 font-extrabold">Registered On</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100 text-xs font-semibold text-slate-700">
                {filteredPatients.map((p) => {
                  const genderCode = (p.gender || '').toUpperCase();
                  const genderText = genderCode === 'M' || genderCode === 'MALE' ? 'Male' : genderCode === 'F' || genderCode === 'FEMALE' ? 'Female' : p.gender || 'N/A';
                  
                  const anyP = p as unknown as Record<string, unknown>;
                  let pincodeVal = p.pincode || (anyP.pinCode as string) || (anyP.Pincode as string) || (anyP.zip as string) || '';
                  let dobVal = p.dob || (anyP.Dob as string) || (anyP.dateOfBirth as string) || '';
                  let fullAddr = [p.address, p.city, p.state].filter(Boolean).join(', ');

                  if ((!pincodeVal || !dobVal || !fullAddr) && p.rawPayloadJson) {
                    try {
                      const rawObj = JSON.parse(p.rawPayloadJson);
                      if (!pincodeVal) pincodeVal = rawObj.pincode || rawObj.pinCode || rawObj.Pincode || rawObj.zip || '';
                      if (!dobVal) dobVal = rawObj.dob || rawObj.Dob || rawObj.dayOfBirth || '';
                      if (!fullAddr) {
                        const rawParts = [rawObj.address || rawObj.Address, rawObj.city || rawObj.City, rawObj.state || rawObj.State].filter(Boolean);
                        if (rawParts.length > 0) fullAddr = rawParts.join(', ');
                      }
                    } catch { /* Ignore JSON parse errors */ }
                  }

                  const primaryAbhaAddr = p.abhaAddresses && p.abhaAddresses.length > 0 ? p.abhaAddresses[0] : null;

                  return (
                    <tr key={p.patientId} className="hover:bg-sky-50/50 transition-colors">
                      {/* Patient Name & UHID */}
                      <td className="py-3.5 px-4">
                        <div className="flex items-center gap-3">
                          <div className="w-8 h-8 rounded-full bg-gradient-to-br from-[#037BBA] to-[#026296] text-white flex items-center justify-center font-extrabold text-xs shrink-0 shadow-inner">
                            {p.name.charAt(0).toUpperCase()}
                          </div>
                          <div className="flex flex-col">
                            <span className="font-bold text-slate-900 text-sm leading-tight">{p.name}</span>
                            <span className="text-[11px] font-semibold text-brand tracking-wide mt-0.5">
                              {p.uhid || `ID: ${p.patientId}`}
                            </span>
                          </div>
                        </div>
                      </td>

                      {/* Gender & DOB */}
                      <td className="py-3.5 px-4">
                        <div className="flex flex-col gap-1">
                          <span
                            className={`inline-self-start px-2 py-0.5 rounded-full text-[11px] font-extrabold w-fit ${
                              genderText === 'Male'
                                ? 'bg-sky-50 text-sky-700 border border-sky-200/60'
                                : genderText === 'Female'
                                ? 'bg-rose-50 text-rose-700 border border-rose-200/60'
                                : 'bg-slate-100 text-slate-600'
                            }`}
                          >
                            {genderText}
                          </span>
                          <span className="text-[11px] font-medium text-slate-400">
                            DOB: {dobVal || 'N/A'}
                          </span>
                        </div>
                      </td>

                      {/* ABHA Number + Copy Button */}
                      <td className="py-3.5 px-4">
                        <div className="flex items-center gap-1.5 font-mono text-xs font-bold text-slate-800">
                          <span>{p.abhaNumber || 'N/A'}</span>
                          {p.abhaNumber && (
                            <button
                              type="button"
                              onClick={() => handleCopy(p.abhaNumber, `abha-${p.patientId}`)}
                              className="text-slate-400 hover:text-brand transition-colors p-1"
                              title="Copy ABHA Number"
                            >
                              {copiedIndex === `abha-${p.patientId}` ? (
                                <span className="text-emerald-600 text-[10px] font-bold">Copied!</span>
                              ) : (
                                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                  <rect x="9" y="9" width="13" height="13" rx="2" /><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1" />
                                </svg>
                              )}
                            </button>
                          )}
                        </div>
                      </td>

                      {/* ABHA Address */}
                      <td className="py-3.5 px-4">
                        {p.abhaAddresses && p.abhaAddresses.length > 1 ? (
                          <select className="bg-slate-50 border border-slate-200 rounded-lg text-xs font-semibold text-slate-700 py-1 px-2 outline-none focus:border-brand cursor-pointer">
                            {p.abhaAddresses.map((addr, idx) => (
                              <option key={idx} value={addr}>
                                {addr} {idx === 0 ? '(Primary)' : ''}
                              </option>
                            ))}
                          </select>
                        ) : primaryAbhaAddr ? (
                          <span className="inline-block bg-slate-100 border border-slate-200 text-slate-700 rounded-full px-2.5 py-0.5 text-[11px] font-bold">
                            {primaryAbhaAddr}
                          </span>
                        ) : (
                          <span className="text-slate-400 italic text-[11px]">No ABHA Address</span>
                        )}
                      </td>

                      {/* Mobile */}
                      <td className="py-3.5 px-4 font-semibold text-slate-700 whitespace-nowrap">
                        {p.mobile ? `+91 ${p.mobile}` : 'N/A'}
                      </td>

                      {/* Address & Pincode */}
                      <td className="py-3.5 px-4 max-w-[200px]">
                        <div className="flex flex-col gap-0.5">
                          <span className="truncate text-slate-700 text-xs" title={fullAddr}>
                            {fullAddr || 'N/A'}
                          </span>
                          {pincodeVal && (
                            <span className="text-[10px] font-bold text-slate-400">
                              PIN: {pincodeVal}
                            </span>
                          )}
                        </div>
                      </td>

                      {/* Registered Date */}
                      <td className="py-3.5 px-4 whitespace-nowrap text-slate-500 text-[11px] font-medium">
                        {p.createdAt ? new Date(p.createdAt).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' }) : 'N/A'}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};

export default PatientsListPage;
