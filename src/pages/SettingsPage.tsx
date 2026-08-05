import React, { useEffect, useState } from 'react';
import api from '@/api/abhaApi';

interface FirmSettings {
  firmId?: number;
  firmName: string;
  firmLogo: string;
  facilityId: string;
  contactMobile: string;
  contactEmail: string;
  address: string;
}

interface UserProfile {
  userId: number;
  username: string;
  fullName: string;
  role: string;
  isActive: boolean;
}

const SettingsPage: React.FC = () => {
  const [firmSettings, setFirmSettings] = useState<FirmSettings>({
    firmName: '',
    firmLogo: '',
    facilityId: '',
    contactMobile: '',
    contactEmail: '',
    address: '',
  });

  const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [toastMessage, setToastMessage] = useState<{ text: string; type: 'success' | 'error' } | null>(null);

  useEffect(() => {
    let isMounted = true;

    const fetchInitialData = async () => {
      setIsLoading(true);
      try {
        const [firmResp, userResp] = await Promise.all([
          api.get('/api/settings/firm').catch(() => null),
          api.get('/api/settings/user-profile').catch(() => null),
        ]);

        if (isMounted) {
          if (firmResp && firmResp.data) {
            setFirmSettings({
              firmId: firmResp.data.firmId || 0,
              firmName: firmResp.data.firmName || '',
              firmLogo: firmResp.data.firmLogo || '',
              facilityId: firmResp.data.facilityId || '',
              contactMobile: firmResp.data.contactMobile || '',
              contactEmail: firmResp.data.contactEmail || '',
              address: firmResp.data.address || '',
            });
          }
          if (userResp && userResp.data) {
            setUserProfile(userResp.data);
          }
        }
      } catch {
        // ignore
      } finally {
        if (isMounted) setIsLoading(false);
      }
    };

    fetchInitialData();
    return () => {
      isMounted = false;
    };
  }, []);

  const handleLogoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.size > 2 * 1024 * 1024) {
        setToastMessage({ text: 'Logo image size should be under 2MB', type: 'error' });
        return;
      }
      const reader = new FileReader();
      reader.onloadend = () => {
        setFirmSettings((prev) => ({ ...prev, firmLogo: reader.result as string }));
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSaveFirmSettings = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    setToastMessage(null);

    try {
      const resp = await api.post('/api/settings/firm', firmSettings);
      if (resp && resp.data && resp.data.success) {
        setToastMessage({ text: 'Firm settings saved successfully to SQL Server database!', type: 'success' });
        window.dispatchEvent(new Event('firmSettingsUpdated'));
      } else {
        setToastMessage({ text: 'Failed to save firm settings. Please try again.', type: 'error' });
      }
    } catch {
      setToastMessage({ text: 'Error connecting to database server.', type: 'error' });
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="table-loading-box" style={{ padding: '60px', textAlign: 'center' }}>
        <div className="btn-spinner-sm" style={{ width: '28px', height: '28px', borderTopColor: '#037BBA' }} />
        <p style={{ marginTop: '12px', fontWeight: 600, color: '#64748b' }}>Loading Settings...</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container" style={{ maxWidth: '960px' }}>
      {/* Page Header */}
      <div className="dashboard-hero-banner" style={{ padding: '24px 28px' }}>
        <div className="hero-text-block">
          <div className="hero-badge-row">
            <span className="hero-status-tag blue">⚙️ System Configuration</span>
            <span className="hero-status-tag slate">SQL Table: tblFirmSettings</span>
          </div>
          <h1 className="hero-main-title">Firm & User Settings</h1>
          <p className="hero-sub-text">
            Configure your hospital/firm brand name, logo, facility ID, and user profile information.
          </p>
        </div>
      </div>

      {toastMessage && (
        <div
          className={`custom-status-banner ${toastMessage.type}`}
          style={{
            background: toastMessage.type === 'success' ? '#ecfdf5' : '#fef2f2',
            borderColor: toastMessage.type === 'success' ? '#a7f3d0' : '#fecaca',
            color: toastMessage.type === 'success' ? '#059669' : '#dc2626',
            padding: '12px 16px',
            borderRadius: '10px',
            fontWeight: 600,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <span>{toastMessage.text}</span>
          <button
            type="button"
            onClick={() => setToastMessage(null)}
            style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'currentColor' }}
          >
            ✕
          </button>
        </div>
      )}

      {/* Main Grid: Firm Settings & User Profile */}
      <div className="hr-main-grid" style={{ gridTemplateColumns: '1fr' }}>
        {/* Card 1: Firm & Hospital Branding */}
        <div className="flat-module-card" style={{ cursor: 'default' }}>
          <div className="details-section-header" style={{ marginBottom: '16px', borderBottom: '1px solid #f1f5f9', paddingBottom: '10px' }}>
            <span className="section-title">🏥 Hospital / Firm Branding</span>
            <span className="module-tag-chip">Live Brand Logo & Name</span>
          </div>

          <form onSubmit={handleSaveFirmSettings} className="nha-verify-form">
            <div className="grid-two-col" style={{ gap: '16px' }}>
              {/* Firm Name */}
              <div className="input-field-block">
                <label className="uppercase-field-label">
                  Firm / Hospital Name <span className="required-star">*</span>
                </label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. DOCTOR 24|7 or SwasthyaCare Clinic"
                  value={firmSettings.firmName}
                  onChange={(e) => setFirmSettings({ ...firmSettings, firmName: e.target.value })}
                />
                <span style={{ fontSize: '11px', color: '#94a3b8', marginTop: '2px', display: 'block' }}>
                  If NULL or empty in SQL table, header shows blank space.
                </span>
              </div>

              {/* ABDM Facility ID */}
              <div className="input-field-block">
                <label className="uppercase-field-label">
                  ABDM Facility HPR ID
                </label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. HPR-SWASTHYA-9921"
                  value={firmSettings.facilityId}
                  onChange={(e) => setFirmSettings({ ...firmSettings, facilityId: e.target.value })}
                />
              </div>
            </div>

            {/* Logo Upload & Live Preview Row */}
            <div className="grid-two-col" style={{ gap: '16px', marginTop: '16px' }}>
              <div className="input-field-block">
                <label className="uppercase-field-label">Firm Logo (PNG / JPG / SVG)</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleLogoUpload}
                  className="rounded-form-input"
                  style={{ padding: '8px' }}
                />
                <span style={{ fontSize: '11px', color: '#94a3b8', marginTop: '2px', display: 'block' }}>
                  Select file to convert to Base64 image and save in SQL Server.
                </span>
              </div>

              {/* Logo Preview */}
              <div className="input-field-block">
                <label className="uppercase-field-label">Logo Preview</label>
                <div
                  style={{
                    height: '52px',
                    border: '1.5px dashed #cbd5e1',
                    borderRadius: '10px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    background: '#f8fafc',
                    padding: '6px',
                  }}
                >
                  {firmSettings.firmLogo ? (
                    <img
                      src={firmSettings.firmLogo}
                      alt="Firm Logo Preview"
                      style={{ maxHeight: '100%', maxWidth: '100%', objectFit: 'contain' }}
                    />
                  ) : (
                    <span style={{ fontSize: '12px', color: '#94a3b8' }}>[ No Logo Uploaded - Shows Default Symbol ]</span>
                  )}
                </div>
              </div>
            </div>

            {/* Contact Information */}
            <div className="grid-two-col" style={{ gap: '16px', marginTop: '16px' }}>
              <div className="input-field-block">
                <label className="uppercase-field-label">Contact Phone / Mobile</label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. +91 98765 43210"
                  value={firmSettings.contactMobile}
                  onChange={(e) => setFirmSettings({ ...firmSettings, contactMobile: e.target.value })}
                />
              </div>

              <div className="input-field-block">
                <label className="uppercase-field-label">Contact Email</label>
                <input
                  type="email"
                  className="rounded-form-input"
                  placeholder="e.g. contact@doctor247.com"
                  value={firmSettings.contactEmail}
                  onChange={(e) => setFirmSettings({ ...firmSettings, contactEmail: e.target.value })}
                />
              </div>
            </div>

            {/* Address */}
            <div className="input-field-block" style={{ marginTop: '16px' }}>
              <label className="uppercase-field-label">Clinic / Hospital Address</label>
              <input
                type="text"
                className="rounded-form-input"
                placeholder="Full address of hospital/clinic"
                value={firmSettings.address}
                onChange={(e) => setFirmSettings({ ...firmSettings, address: e.target.value })}
              />
            </div>

            {/* Save Button */}
            <div style={{ marginTop: '20px', display: 'flex', justifyContent: 'flex-end' }}>
              <button
                type="submit"
                className="hero-primary-btn"
                disabled={isSaving}
                style={{ background: '#037BBA' }}
              >
                {isSaving ? (
                  <>
                    <span className="btn-spinner-sm" style={{ width: '14px', height: '14px' }} />
                    Saving to SQL...
                  </>
                ) : (
                  '💾 Save Firm Settings'
                )}
              </button>
            </div>
          </form>
        </div>

        {/* Card 2: Live Logged-in User Info from tblUsers */}
        <div className="flat-module-card" style={{ cursor: 'default', marginTop: '20px' }}>
          <div className="details-section-header" style={{ marginBottom: '14px', borderBottom: '1px solid #f1f5f9', paddingBottom: '10px' }}>
            <span className="section-title">👤 Logged-in User Info (Live from tblUsers)</span>
            <span className="module-tag-chip">SQL Table: dbo.tblUsers</span>
          </div>

          {userProfile ? (
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px' }}>
              <div className="metric-card" style={{ padding: '14px' }}>
                <div className="metric-data">
                  <span className="metric-label">Full Name</span>
                  <span className="metric-number" style={{ fontSize: '15px', color: '#037BBA' }}>
                    {userProfile.fullName || '(Empty)'}
                  </span>
                </div>
              </div>

              <div className="metric-card" style={{ padding: '14px' }}>
                <div className="metric-data">
                  <span className="metric-label">Username</span>
                  <span className="metric-number" style={{ fontSize: '15px' }}>
                    {userProfile.username || '(Empty)'}
                  </span>
                </div>
              </div>

              <div className="metric-card" style={{ padding: '14px' }}>
                <div className="metric-data">
                  <span className="metric-label">Role</span>
                  <span className="metric-number" style={{ fontSize: '15px', color: '#7c3aed' }}>
                    {userProfile.role || 'Doctor'}
                  </span>
                </div>
              </div>

              <div className="metric-card" style={{ padding: '14px' }}>
                <div className="metric-data">
                  <span className="metric-label">Account Status</span>
                  <span className="metric-number" style={{ fontSize: '14px', color: userProfile.isActive ? '#059669' : '#dc2626' }}>
                    {userProfile.isActive ? '● Active' : '○ Inactive'}
                  </span>
                </div>
              </div>
            </div>
          ) : (
            <p style={{ color: '#94a3b8', fontSize: '13px' }}>User profile not found in SQL database.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default SettingsPage;
