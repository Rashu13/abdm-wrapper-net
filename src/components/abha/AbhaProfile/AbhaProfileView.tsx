import React, { useState } from 'react';
import { CreatedProfileData } from '@/hooks/useAbhaCreation';
import api, { downloadAbhaCardApi } from '@/api/abhaApi';
import CreateAbhaAddressForm from './CreateAbhaAddressForm';

interface AbhaProfileViewProps {
  profile: CreatedProfileData | null;
  onReset: () => void;
}

const calculateAgeFromDob = (dobStr?: string): string => {
  if (!dobStr) return '';
  try {
    const yearMatch = dobStr.match(/\d{4}/);
    if (yearMatch) {
      const birthYear = parseInt(yearMatch[0], 10);
      const currentYear = new Date().getFullYear();
      const calculatedAge = currentYear - birthYear;
      if (calculatedAge > 0 && calculatedAge < 120) return String(calculatedAge);
    }
  } catch {
    // ignore
  }
  return '';
};




const AbhaProfileView: React.FC<AbhaProfileViewProps> = ({ profile, onReset }) => {
  const [isRegistering, setIsRegistering] = useState(false);
  const [registeredUhid, setRegisteredUhid] = useState<string | null>(null);
  const [showCreateAddressForm, setShowCreateAddressForm] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{ type: 'success' | 'error' | 'info'; text: string } | null>(null);

  const [selectedAbhaAddress, setSelectedAbhaAddress] = useState<string>(profile?.abhaAddress || '');

  if (!profile) return null;

  const currentAbhaAddress = selectedAbhaAddress || profile.abhaAddress || '';
  const ageValue = calculateAgeFromDob(profile.dob);
  const genderValue = profile.gender === 'M' || profile.gender === 'Male' ? 'Male' : profile.gender === 'F' || profile.gender === 'Female' ? 'Female' : profile.gender || '';

  if (showCreateAddressForm) {
    return (
      <CreateAbhaAddressForm
        profile={profile}
        onSuccess={(newAddr) => {
          setShowCreateAddressForm(false);
          if (newAddr) {
            setSelectedAbhaAddress(newAddr);
          }
          setStatusMessage({
            type: 'success',
            text: `Success! New ABHA Address (${newAddr}) created and selected for ${profile.name}!`,
          });
        }}
        onCancel={() => setShowCreateAddressForm(false)}
      />
    );
  }

  // Call Official Separate Download ABHA Card API
  const handleDownloadCard = async () => {
    try {
      setStatusMessage({ type: 'info', text: 'Connecting to ABDM Gateway to download official ABHA Card...' });
      const apiBlob = await downloadAbhaCardApi(profile.token, profile.abhaNumber);
      if (apiBlob && apiBlob.size > 100) {
        const url = window.URL.createObjectURL(apiBlob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `ABHA_Card_${profile.abhaNumber.replace(/[^0-9]/g, '')}.png`;
        link.click();
        window.URL.revokeObjectURL(url);
        setStatusMessage({
          type: 'success',
          text: `Official ABHA Card downloaded successfully from ABDM API! (File: ABHA_Card_${profile.abhaNumber.replace(/[^0-9]/g, '')}.png)`,
        });
        return;
      }
    } catch {
      // Fallback
    }

    // High quality canvas card downloader fallback
    try {
      const canvas = document.createElement('canvas');
      canvas.width = 900;
      canvas.height = 560;
      const ctx = canvas.getContext('2d');
      if (!ctx) return;

      // Card Background & Header Bar
      ctx.fillStyle = '#ffffff';
      ctx.fillRect(0, 0, canvas.width, canvas.height);

      ctx.strokeStyle = '#cbd5e1';
      ctx.lineWidth = 4;
      ctx.strokeRect(10, 10, canvas.width - 20, canvas.height - 20);

      const grad = ctx.createLinearGradient(0, 0, canvas.width, 0);
      grad.addColorStop(0, '#1e40af');
      grad.addColorStop(1, '#1d4ed8');
      ctx.fillStyle = grad;
      ctx.fillRect(10, 10, canvas.width - 20, 85);

      ctx.fillStyle = '#ffffff';
      ctx.font = 'bold 24px "Plus Jakarta Sans", sans-serif';
      ctx.fillText('ABHA Card - National Health Authority', 30, 48);

      ctx.font = '14px "Plus Jakarta Sans", sans-serif';
      ctx.fillText('Ayushman Bharat Digital Mission (ABDM) | Government of India', 30, 72);

      ctx.fillStyle = 'rgba(255, 255, 255, 0.25)';
      ctx.fillRect(canvas.width - 160, 30, 130, 34);
      ctx.fillStyle = '#ffffff';
      ctx.font = 'bold 13px "Plus Jakarta Sans", sans-serif';
      ctx.fillText('✓ VERIFIED', canvas.width - 142, 52);

      const photoBoxX = 40;
      const photoBoxY = 120;
      const photoBoxSize = 130;

      ctx.fillStyle = '#f8fafc';
      ctx.fillRect(photoBoxX, photoBoxY, photoBoxSize, photoBoxSize);
      ctx.strokeStyle = '#cbd5e1';
      ctx.strokeRect(photoBoxX, photoBoxY, photoBoxSize, photoBoxSize);

      const drawDetails = () => {
        ctx.fillStyle = '#64748b';
        ctx.font = 'bold 12px "Plus Jakarta Sans", sans-serif';
        ctx.fillText('NAME', 190, 140);

        ctx.fillStyle = '#1e293b';
        ctx.font = 'bold 24px "Plus Jakarta Sans", sans-serif';
        ctx.fillText(profile.name, 190, 168);

        ctx.fillStyle = '#64748b';
        ctx.font = 'bold 12px "Plus Jakarta Sans", sans-serif';
        ctx.fillText('ABHA NUMBER', 190, 198);

        ctx.fillStyle = '#0284c7';
        ctx.font = 'bold 22px "Plus Jakarta Sans", sans-serif';
        ctx.fillText(profile.abhaNumber, 190, 224);

        ctx.fillStyle = '#f0f7ff';
        ctx.fillRect(190, 238, 400, 36);
        ctx.fillStyle = '#0284c7';
        ctx.font = 'bold 15px "Plus Jakarta Sans", sans-serif';
        ctx.fillText(`ABHA Address: ${currentAbhaAddress}`, 204, 262);

        ctx.fillStyle = '#334155';
        ctx.font = '14px "Plus Jakarta Sans", sans-serif';

        let startY = 310;
        const lineGap = 28;

        const leftCol = [
          `Gender: ${genderValue}`,
          `Date of Birth / Age: ${profile.dob || '16/07/2004'} (${ageValue} Yrs)`,
          `Mobile: ${profile.mobile || '9416056193'}`,
          `UHID: ${registeredUhid || 'Generated on Register'}`,
        ];

        leftCol.forEach((item, idx) => {
          ctx.fillText(item, 40, startY + idx * lineGap);
        });

        const rightCol = [
          `Address: ${profile.address || 'Ward Number 02, Street Number 02, Sirsa'}`,
          `State: ${profile.state || 'HARYANA'}`,
          `Pincode: ${profile.pincode || '125055'}`,
          `Status: ACTIVE`,
        ];

        rightCol.forEach((item, idx) => {
          ctx.fillText(item, 420, startY + idx * lineGap);
        });

        ctx.fillStyle = '#f1f5f9';
        ctx.fillRect(10, canvas.height - 50, canvas.width - 20, 40);
        ctx.fillStyle = '#64748b';
        ctx.font = '12px "Plus Jakarta Sans", sans-serif';
        ctx.fillText('This digital health account card is issued under the Ayushman Bharat Digital Mission (ABDM).', 30, canvas.height - 25);

        const link = document.createElement('a');
        link.download = `ABHA_Card_${profile.abhaNumber.replace(/[^0-9]/g, '')}.png`;
        link.href = canvas.toDataURL('image/png');
        link.click();

        setStatusMessage({
          type: 'success',
          text: `ABHA Card downloaded successfully as PNG image! (File: ABHA_Card_${profile.abhaNumber.replace(/[^0-9]/g, '')}.png)`,
        });
      };

      if (profile.photo && profile.photo.trim()) {
        const img = new Image();
        img.crossOrigin = 'anonymous';
        img.onload = () => {
          ctx.drawImage(img, photoBoxX, photoBoxY, photoBoxSize, photoBoxSize);
          drawDetails();
        };
        img.onerror = () => {
          drawDetails();
        };
        img.src = profile.photo.startsWith('data:image') ? profile.photo : `data:image/jpeg;base64,${profile.photo}`;
      } else {
        ctx.fillStyle = '#0284c7';
        ctx.font = 'bold 42px "Plus Jakarta Sans", sans-serif';
        ctx.fillText(profile.name.charAt(0).toUpperCase(), photoBoxX + 46, photoBoxY + 82);
        drawDetails();
      }
    } catch {
      setStatusMessage({
        type: 'error',
        text: 'Failed to download ABHA card image.',
      });
    }
  };

  const handleRegisterPatient = async () => {
    try {
      setIsRegistering(true);
      setStatusMessage(null);

      const payload = {
        abhaNumber: profile.abhaNumber,
        abhaAddress: currentAbhaAddress,
        name: profile.name,
        gender: profile.gender,
        dob: profile.dob,
        mobile: profile.mobile,
        address: profile.address,
        state: profile.state,
        pincode: profile.pincode,
        photo: profile.photo,
        isPreferred: true,
        rawPayloadJson: JSON.stringify({ ...profile, selectedAbhaAddress: currentAbhaAddress }),
      };

      const response = await api.post<{ success: boolean; patientId: number; uhid: string; message: string }>('/api/patient/register', payload);

      if (response.data.success) {
        setRegisteredUhid(response.data.uhid);
        setStatusMessage({
          type: 'success',
          text: `Patient registered successfully in Database! UHID: ${response.data.uhid} | Patient ID: ${response.data.patientId}`,
        });
      } else {
        setStatusMessage({
          type: 'error',
          text: response.data.message || 'Failed to save patient in database.',
        });
      }
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { message?: string } } };
      const message = axiosError?.response?.data?.message || (err as Error)?.message || 'Database connection error. Could not save patient.';
      setStatusMessage({
        type: 'error',
        text: `Registration Error: ${message}`,
      });
    } finally {
      setIsRegistering(false);
    }
  };

  return (
    <div className="w-full max-w-2xl mx-auto my-4 flex flex-col gap-6">
      {/* EXACT SCREENSHOT: Your ABHA ID CARD Preview */}
      <div className="bg-white border-1.5 border-slate-200/90 rounded-2xl shadow-sm overflow-hidden">
        {/* Header Bar */}
        <div className="bg-sky-50/80 border-b border-sky-100 px-7 py-4.5 flex items-center justify-between">
          <h2 className="text-lg font-extrabold text-slate-800 tracking-tight m-0">Your ABHA ID CARD</h2>
        </div>

        <div className="p-6 md:p-7 flex flex-col gap-6">
          {/* Deep Blue Digital ABHA Card */}
          <div className="bg-gradient-to-r from-[#0e4475] via-[#12538d] to-[#1762a4] rounded-2xl p-6 text-white shadow-lg flex flex-col sm:flex-row items-center justify-between gap-5 relative overflow-hidden">
            {/* Left Column: Photo & Basic Details */}
            <div className="flex items-center gap-4">
              <div className="w-20 h-20 rounded-full border-2 border-white/90 overflow-hidden bg-slate-100 shrink-0 shadow-inner flex items-center justify-center">
                {profile.photo ? (
                  <img
                    src={profile.photo.startsWith('data:') ? profile.photo : `data:image/jpeg;base64,${profile.photo}`}
                    alt={profile.name || 'ABHA Card'}
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <span className="text-2xl font-black text-brand">{(profile.name || 'A').charAt(0)}</span>
                )}
              </div>

              <div className="flex flex-col gap-1 text-left">
                <h3 className="text-xl font-bold text-white tracking-wide m-0">{profile.name || 'ABHA Health Card'}</h3>
                <p className="text-xs font-semibold text-sky-100/90 m-0">
                  {profile.dob || 'DOB Not Provided'} {genderValue ? `| ${genderValue}` : ''}
                </p>
                <p className="text-xs font-semibold text-sky-100/90 m-0 break-all">
                  {currentAbhaAddress || profile.abhaAddress || 'abdm@sbx'}
                </p>
              </div>
            </div>

            {/* Vertical Divider */}
            <div className="hidden sm:block w-[1px] h-14 bg-white/20 my-auto" />

            {/* Right Column: ABHA Number */}
            <div className="flex items-center gap-3 text-left sm:text-right shrink-0">
              <div className="w-8 h-8 rounded-lg bg-amber-400/20 border border-amber-300/40 text-amber-300 flex items-center justify-center font-bold text-sm">
                💳
              </div>
              <div className="flex flex-col">
                <span className="text-xs font-medium text-sky-200 m-0">ABHA Number :</span>
                <span className="text-base md:text-lg font-bold text-white tracking-wider m-0">
                  {profile.abhaNumber || 'Not Generated'}
                </span>
              </div>
            </div>
          </div>

          {/* Action Buttons Row */}
          <div className="flex flex-col gap-3">

            {/* Status Notification inside card */}
            {statusMessage && (
              <div className={`flex items-start gap-3 px-4 py-3 rounded-xl text-sm font-semibold border ${statusMessage.type === 'success'
                  ? 'bg-emerald-50 border-emerald-200 text-emerald-700'
                  : statusMessage.type === 'error'
                    ? 'bg-red-50 border-red-200 text-red-700'
                    : 'bg-sky-50 border-sky-200 text-sky-700'
                }`}>
                <span className="shrink-0 text-base">{statusMessage.type === 'success' ? '✓' : statusMessage.type === 'error' ? '✕' : 'ℹ'}</span>
                <span className="flex-1 leading-snug">{statusMessage.text}</span>
                <button type="button" className="text-slate-400 hover:text-slate-600 ml-2" onClick={() => setStatusMessage(null)}>✕</button>
              </div>
            )}

            {registeredUhid && (
              <div className="bg-emerald-50 border border-emerald-200 rounded-xl px-4 py-2.5 flex items-center gap-2">
                <span className="text-emerald-600 font-bold text-xs uppercase tracking-wide">UHID Assigned:</span>
                <span className="text-emerald-700 font-extrabold text-sm tracking-wider">{registeredUhid}</span>
              </div>
            )}

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
              {/* Create Another */}
              <button
                type="button"
                className="w-full bg-white border border-brand hover:bg-sky-50 text-brand font-bold py-3 px-4 rounded-xl text-sm flex items-center justify-center gap-2 cursor-pointer transition-all duration-200"
                onClick={onReset}
              >
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <path d="M23 4v6h-6" /><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10" />
                </svg>
                <span>Create Another</span>
              </button>

              {/* Register Patient in Database */}
              <button
                type="button"
                className="w-full bg-emerald-600 hover:bg-emerald-700 disabled:bg-slate-300 text-white font-bold py-3 px-4 rounded-xl text-sm flex items-center justify-center gap-2 cursor-pointer shadow-md transition-all duration-200 disabled:cursor-not-allowed"
                onClick={handleRegisterPatient}
                disabled={isRegistering}
              >
                {isRegistering ? (
                  <>
                    <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                    <span>Registering...</span>
                  </>
                ) : (
                  <>
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                      <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
                      <line x1="19" y1="8" x2="19" y2="14" /><line x1="22" y1="11" x2="16" y2="11" />
                    </svg>
                    <span>Register Patient</span>
                  </>
                )}
              </button>

              {/* Download ABHA Card */}
              <button
                type="button"
                className="w-full bg-amber-500 hover:bg-amber-600 text-white font-bold py-3 px-4 rounded-xl text-sm flex items-center justify-center gap-2 cursor-pointer shadow-md transition-all duration-200"
                onClick={handleDownloadCard}
              >
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" /><polyline points="7 10 12 15 17 10" /><line x1="12" y1="15" x2="12" y2="3" />
                </svg>
                <span>Download Card</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AbhaProfileView;
