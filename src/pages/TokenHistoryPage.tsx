import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '@/api/abhaApi';

export interface TokenHistoryItem {
  tokenId: number;
  tokenNumber: string;
  patientName: string;
  abhaAddress: string;
  tokenType: string;
  careContextRef: string;
  status: string;
  issuedAt: string;
  expiresAt?: string;
}

const ALL_TOKEN_TYPES = [
  'Care Context Token',
  'OPD Registration Token',
  'Auth OTP Token',
  'Subscription Token',
];

const TokenHistoryPage: React.FC = () => {
  const navigate = useNavigate();

  const [tokensList, setTokensList] = useState<TokenHistoryItem[]>([]);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [typeFilter, setTypeFilter] = useState<string>('All Types');
  const [statusFilter, setStatusFilter] = useState<string>('All Status');

  // Modal State
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [patientNameInput, setPatientNameInput] = useState<string>('');
  const [abhaAddressInput, setAbhaAddressInput] = useState<string>('');
  const [tokenTypeInput, setTokenTypeInput] = useState<string>('Care Context Token');
  const [careContextRefInput, setCareContextRefInput] = useState<string>('');
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [copyNotice, setCopyNotice] = useState<string | null>(null);

  // Fetch tokens from backend API
  const fetchTokens = async () => {
    try {
      const res = await api.get<{ success: boolean; data: TokenHistoryItem[] }>('/api/token/list');
      if (res.data.success && Array.isArray(res.data.data)) {
        setTokensList(res.data.data);
      }
    } catch (err) {
      console.error('Failed to fetch tokens from backend API', err);
    }
  };

  useEffect(() => {
    fetchTokens();
  }, []);

  const handleCreateToken = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!patientNameInput.trim() || !abhaAddressInput.trim()) {
      alert('Patient Name and ABHA Address are required!');
      return;
    }

    try {
      setIsSubmitting(true);
      const res = await api.post<{ success: boolean; message: string; data: TokenHistoryItem }>(
        '/api/token/create',
        {
          patientName: patientNameInput.trim(),
          abhaAddress: abhaAddressInput.trim(),
          tokenType: tokenTypeInput,
          careContextRef: careContextRefInput.trim() || `OPD-ENC-${Math.floor(1000 + Math.random() * 9000)}`,
        }
      );

      if (res.data.success && res.data.data) {
        setTokensList((prev) => [res.data.data, ...prev]);
        setIsModalOpen(false);
        setPatientNameInput('');
        setAbhaAddressInput('');
        setCareContextRefInput('');
        alert(`✓ Token generated successfully: ${res.data.data.tokenNumber}`);
      } else {
        alert(`Failed to create token: ${res.data.message}`);
      }
    } catch (err: any) {
      alert(`Error creating token: ${err.message || 'Server Error'}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCopyToken = (tokenNum: string) => {
    navigator.clipboard.writeText(tokenNum);
    setCopyNotice(`Copied ${tokenNum} to clipboard!`);
    setTimeout(() => setCopyNotice(null), 2500);
  };

  const handleClearFilters = () => {
    setSearchQuery('');
    setTypeFilter('All Types');
    setStatusFilter('All Status');
  };

  // Filter Tokens
  const filteredTokens = tokensList.filter((item) => {
    const q = searchQuery.toLowerCase().trim();
    const matchesSearch =
      !q ||
      item.tokenNumber.toLowerCase().includes(q) ||
      item.patientName.toLowerCase().includes(q) ||
      item.abhaAddress.toLowerCase().includes(q) ||
      item.careContextRef.toLowerCase().includes(q);

    const matchesType = typeFilter === 'All Types' || item.tokenType === typeFilter;
    const matchesStatus = statusFilter === 'All Status' || item.status === statusFilter;

    return matchesSearch && matchesType && matchesStatus;
  });

  const totalTokens = tokensList.length;
  const linkedTokens = tokensList.filter((t) => t.status.toLowerCase() === 'linked').length;
  const activeTokens = tokensList.filter((t) => t.status.toLowerCase() === 'active').length;
  const expiredTokens = tokensList.filter((t) => t.status.toLowerCase() === 'expired').length;

  return (
    <div className="consent-requests-page-wrapper">
      {/* Copy Alert Banner */}
      {copyNotice && (
        <div className="custom-status-banner success" style={{ marginBottom: '16px' }}>
          <span className="status-banner-icon">✓</span>
          <span className="status-banner-text">{copyNotice}</span>
          <button className="status-banner-close" onClick={() => setCopyNotice(null)}>✕</button>
        </div>
      )}

      {/* Header Banner */}
      <div className="consent-header-banner">
        <div className="banner-left-content">
          <span className="abdm-pill-badge">ABDM Token Manager</span>
          <h1 className="banner-main-title">Token History & Audit</h1>
          <p className="banner-subtitle-text">
            Track all ABHA OPD registration tokens, Care Context linking tokens & Auth OTP tokens
          </p>
        </div>
        <div className="banner-right-content">
          <button
            type="button"
            className="new-consent-req-btn"
            onClick={() => setIsModalOpen(true)}
          >
            + Generate New Token
          </button>
        </div>
      </div>

      {/* 4 Summary Stat Cards */}
      <div className="summary-cards-grid">
        <div className="summary-card-box">
          <span className="card-box-label">TOTAL TOKENS</span>
          <span className="card-box-value">{totalTokens}</span>
        </div>

        <div className="summary-card-box">
          <span className="card-box-label">ACTIVE LINKED</span>
          <span className="card-box-value green">{linkedTokens}</span>
        </div>

        <div className="summary-card-box">
          <span className="card-box-label">ACTIVE OPD</span>
          <span className="card-box-value purple">{activeTokens}</span>
        </div>

        <div className="summary-card-box">
          <span className="card-box-label">EXPIRED / USED</span>
          <span className="card-box-value red">{expiredTokens}</span>
        </div>
      </div>

      {/* Filter Strip */}
      <div className="consent-filters-strip">
        <div className="search-field-container">
          <svg className="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <circle cx="11" cy="11" r="8" />
            <line x1="21" y1="21" x2="16.65" y2="16.65" />
          </svg>
          <input
            type="text"
            className="consent-search-input"
            placeholder="Search by Token No / Patient Name / ABHA Address / Care Context..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>

        <div className="filter-controls-group">
          <select
            className="consent-status-select select-arrow"
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
          >
            <option value="All Types">All Token Types</option>
            {ALL_TOKEN_TYPES.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>

          <select
            className="consent-status-select select-arrow"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
          >
            <option value="All Status">All Status</option>
            <option value="Linked">Linked</option>
            <option value="Active">Active</option>
            <option value="Expired">Expired</option>
          </select>

          <button
            type="button"
            className="clear-filters-btn"
            onClick={fetchTokens}
            title="Fetch updated tokens from database"
            style={{ background: '#7c3aed', color: '#ffffff', border: 'none' }}
          >
            🔄 Sync Tokens
          </button>

          <button
            type="button"
            className="clear-filters-btn"
            onClick={handleClearFilters}
          >
            Clear Filters
          </button>
        </div>
      </div>

      {/* Tokens Table */}
      <div className="consent-table-container">
        <table className="consent-data-table">
          <thead>
            <tr>
              <th style={{ width: '120px' }}>TOKEN #</th>
              <th style={{ width: '180px' }}>PATIENT</th>
              <th style={{ width: '220px' }}>TOKEN TYPE</th>
              <th style={{ width: '180px' }}>CARE CONTEXT REF</th>
              <th style={{ width: '130px' }}>STATUS</th>
              <th style={{ width: '180px' }}>ISSUED AT</th>
              <th style={{ width: '160px' }}>ACTION</th>
            </tr>
          </thead>
          <tbody>
            {filteredTokens.length === 0 ? (
              <tr>
                <td colSpan={7} style={{ textAlign: 'center', padding: '48px 24px' }}>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '8px' }}>
                    <div style={{ fontSize: '36px', marginBottom: '4px' }}>🎟️</div>
                    <h3 style={{ fontSize: '16px', fontWeight: 700, color: '#0f172a', margin: 0 }}>No ABDM Tokens Found</h3>
                    <p style={{ fontSize: '13px', color: '#64748b', margin: 0, maxWidth: '420px' }}>
                      No OPD or Care Context linking tokens match your search criteria. Click <strong>"+ Generate New Token"</strong> to issue a new token.
                    </p>
                    <button
                      type="button"
                      className="new-consent-req-btn"
                      style={{ marginTop: '12px', fontSize: '12.5px', padding: '8px 16px' }}
                      onClick={() => setIsModalOpen(true)}
                    >
                      + Generate New Token
                    </button>
                  </div>
                </td>
              </tr>
            ) : (
              filteredTokens.map((t) => (
                <tr key={t.tokenId}>
                  {/* TOKEN NUMBER */}
                  <td className="patient-cell">
                    <div className="patient-name-purple-badge" style={{ background: '#7c3aed' }}>
                      {t.tokenNumber}
                    </div>
                  </td>

                  {/* PATIENT */}
                  <td className="patient-cell">
                    <div style={{ fontWeight: 800, color: '#0f172a' }}>{t.patientName}</div>
                    <span className="patient-abha-addr-text">{t.abhaAddress}</span>
                  </td>

                  {/* TOKEN TYPE */}
                  <td className="hi-types-cell">
                    <span className="dark-hi-type-pill granted">{t.tokenType}</span>
                  </td>

                  {/* CARE CONTEXT REF */}
                  <td className="requested-date-cell">
                    <code style={{ background: '#f1f5f9', padding: '2px 8px', borderRadius: '4px', color: '#475569', fontWeight: 700 }}>
                      {t.careContextRef}
                    </code>
                  </td>

                  {/* STATUS */}
                  <td className="status-cell">
                    <span className={`status-badge-pill ${t.status.toLowerCase()}`}>
                      {t.status}
                    </span>
                  </td>

                  {/* ISSUED AT */}
                  <td className="requested-date-cell">
                    <span style={{ fontSize: '12.5px', color: '#334155' }}>{t.issuedAt}</span>
                  </td>

                  {/* ACTION */}
                  <td className="action-cell" style={{ textAlign: 'center' }}>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '6px' }}>
                      <button
                        type="button"
                        onClick={() => handleCopyToken(t.tokenNumber)}
                        title="Copy Token to Clipboard"
                        style={{
                          background: '#f1f5f9',
                          color: '#475569',
                          border: '1px solid #cbd5e1',
                          padding: '5px 10px',
                          borderRadius: '6px',
                          fontSize: '11.5px',
                          fontWeight: 700,
                          cursor: 'pointer',
                        }}
                      >
                        📋 Copy
                      </button>

                      <button
                        type="button"
                        onClick={() => navigate(`/consent-details/${t.tokenId}`, { state: { id: String(t.tokenId), patientName: t.patientName, abhaAddress: t.abhaAddress } })}
                        title="View Token Audit Details"
                        style={{
                          background: '#7c3aed',
                          color: '#ffffff',
                          border: 'none',
                          padding: '5px 10px',
                          borderRadius: '6px',
                          fontSize: '11.5px',
                          fontWeight: 700,
                          cursor: 'pointer',
                        }}
                      >
                        👁️ View
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* MODAL: + Generate New Token */}
      {isModalOpen && (
        <div className="modal-backdrop">
          <div className="consent-modal-card" style={{ maxWidth: '520px' }}>
            <div className="modal-header-row">
              <h3 className="modal-title-text">+ Generate ABDM Token</h3>
              <button
                type="button"
                className="modal-close-btn"
                onClick={() => setIsModalOpen(false)}
              >
                ✕
              </button>
            </div>

            <form onSubmit={handleCreateToken} className="modal-form-body">
              <div className="input-field-block">
                <label className="uppercase-field-label">PATIENT NAME</label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. Ravi Kumar"
                  value={patientNameInput}
                  onChange={(e) => setPatientNameInput(e.target.value)}
                  required
                />
              </div>

              <div className="input-field-block">
                <label className="uppercase-field-label">ABHA ADDRESS / NUMBER</label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. user.5682@sbx"
                  value={abhaAddressInput}
                  onChange={(e) => setAbhaAddressInput(e.target.value)}
                  required
                />
              </div>

              <div className="input-field-block">
                <label className="uppercase-field-label">TOKEN TYPE</label>
                <select
                  className="rounded-form-input select-arrow"
                  value={tokenTypeInput}
                  onChange={(e) => setTokenTypeInput(e.target.value)}
                >
                  {ALL_TOKEN_TYPES.map((type) => (
                    <option key={type} value={type}>{type}</option>
                  ))}
                </select>
              </div>

              <div className="input-field-block">
                <label className="uppercase-field-label">CARE CONTEXT REFERENCE (OPTIONAL)</label>
                <input
                  type="text"
                  className="rounded-form-input"
                  placeholder="e.g. OPD-ENC-9948"
                  value={careContextRefInput}
                  onChange={(e) => setCareContextRefInput(e.target.value)}
                />
              </div>

              <div className="modal-footer-row">
                <button
                  type="button"
                  className="footer-cancel-btn"
                  onClick={() => setIsModalOpen(false)}
                  disabled={isSubmitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="footer-link-primary-btn"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? 'Generating Token...' : 'Generate Token'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default TokenHistoryPage;
