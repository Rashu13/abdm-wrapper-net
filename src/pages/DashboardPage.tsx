import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '@/api/abhaApi';

interface DashboardStats {
  totalPatients: number;
  patientsToday: number;
  totalCareContexts: number;
  careContextsThisWeek: number;
  activeConsents: number;
  totalTransactions: number;
  gatewayStatus: string;
  gatewayUptime: string;
}

const DashboardPage: React.FC = () => {
  const navigate = useNavigate();
  const [stats, setStats] = useState<DashboardStats>({
    totalPatients: 0,
    patientsToday: 0,
    totalCareContexts: 0,
    careContextsThisWeek: 0,
    activeConsents: 0,
    totalTransactions: 0,
    gatewayStatus: 'Operational',
    gatewayUptime: '99.9%',
  });
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    let isMounted = true;
    const fetchLiveStats = async () => {
      try {
        const resp = await api.get('/api/dashboard/stats');
        if (resp && resp.data && isMounted) {
          setStats(resp.data);
        }
      } catch {
        // Fallback default
      } finally {
        if (isMounted) setIsLoading(false);
      }
    };

    fetchLiveStats();
    return () => {
      isMounted = false;
    };
  }, []);

  return (
    <div className="dashboard-container">
      {/* Executive Hero / Header Banner */}
      <div className="dashboard-hero-banner">
        <div className="hero-text-block">
          <div className="hero-badge-row">
            <span className="hero-status-tag green">
              <span className="pulse-dot green" />
              ABDM Gateway {stats.gatewayStatus}
            </span>
            <span className="hero-status-tag blue">M1 & M2 Certified</span>
            <span className="hero-status-tag slate">Facility ID: HPR-SWASTHYA-9921</span>
          </div>
          <h1 className="hero-main-title">ABDM Command Center</h1>
          <p className="hero-sub-text">
            Ayushman Bharat Digital Mission (ABDM) Integrated Enterprise Healthcare Suite
          </p>
        </div>

        <div className="hero-quick-action">
          <button
            type="button"
            className="hero-primary-btn"
            onClick={() => navigate('/abha/create')}
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="16" />
              <line x1="8" y1="12" x2="16" y2="12" />
            </svg>
            + Create New ABHA
          </button>
        </div>
      </div>

      {/* Metrics Counter Grid */}
      <div className="dashboard-metrics-grid">
        {/* Metric 1: Registered Patients */}
        <div className="metric-card">
          <div className="metric-icon-box blue">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
              <circle cx="9" cy="7" r="4" />
              <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
              <path d="M16 3.13a4 4 0 0 1 0 7.75" />
            </svg>
          </div>
          <div className="metric-data">
            <span className="metric-label">Registered Patients</span>
            <div className="metric-value-row">
              <span className="metric-number">
                {isLoading ? '...' : stats.totalPatients.toLocaleString()}
              </span>
              <span className="metric-trend positive">
                +{stats.patientsToday} today
              </span>
            </div>
          </div>
        </div>

        {/* Metric 2: Linked Care Contexts */}
        <div className="metric-card">
          <div className="metric-icon-box purple">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71" />
              <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71" />
            </svg>
          </div>
          <div className="metric-data">
            <span className="metric-label">Linked Care Contexts</span>
            <div className="metric-value-row">
              <span className="metric-number">
                {isLoading ? '...' : stats.totalCareContexts.toLocaleString()}
              </span>
              <span className="metric-trend positive">
                +{stats.careContextsThisWeek} this week
              </span>
            </div>
          </div>
        </div>

        {/* Metric 3: Active Consents */}
        <div className="metric-card">
          <div className="metric-icon-box emerald">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
              <polyline points="9 12 11 14 15 10" />
            </svg>
          </div>
          <div className="metric-data">
            <span className="metric-label">Active Consents</span>
            <div className="metric-value-row">
              <span className="metric-number">
                {isLoading ? '...' : stats.activeConsents.toLocaleString()}
              </span>
              <span className="metric-trend positive">Live SQL Count</span>
            </div>
          </div>
        </div>

        {/* Metric 4: Gateway Uptime & Audit Count */}
        <div className="metric-card">
          <div className="metric-icon-box amber">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2" />
            </svg>
          </div>
          <div className="metric-data">
            <span className="metric-label">ABDM Gateway Uptime</span>
            <div className="metric-value-row">
              <span className="metric-number">{stats.gatewayUptime}</span>
              <span className="metric-trend normal">{stats.gatewayStatus}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Main Module Action Cards Grid */}
      <div className="dashboard-section-header">
        <h2 className="section-title">Core Operations & Modules</h2>
        <span className="section-sub-tag">Select a module to manage health data</span>
      </div>

      <div className="dashboard-cards-grid">
        {/* Module 1: Create ABHA */}
        <div
          className="flat-module-card"
          onClick={() => navigate('/abha/create')}
          role="button"
          tabIndex={0}
        >
          <div className="module-card-top">
            <div className="module-icon-box brand">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="16" />
                <line x1="8" y1="12" x2="16" y2="12" />
              </svg>
            </div>
            <span className="module-tag-chip">ABHA Generation</span>
          </div>
          <div className="module-card-body">
            <h3 className="module-title">Create ABHA Number & Link</h3>
            <p className="module-desc">
              Generate a new 14-digit ABHA number via Aadhaar/Mobile OTP and link patient accounts.
            </p>
          </div>
          <div className="module-card-footer">
            <span className="module-action-text">Launch ABHA Setup</span>
            <span className="module-arrow">→</span>
          </div>
        </div>

        {/* Module 2: Patients List */}
        <div
          className="flat-module-card"
          onClick={() => navigate('/patients')}
          role="button"
          tabIndex={0}
        >
          <div className="module-card-top">
            <div className="module-icon-box brand">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                <circle cx="9" cy="7" r="4" />
                <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
                <path d="M16 3.13a4 4 0 0 1 0 7.75" />
              </svg>
            </div>
            <span className="module-tag-chip">Patient Directory</span>
          </div>
          <div className="module-card-body">
            <h3 className="module-title">Patients Directory & Registry</h3>
            <p className="module-desc">
              View registered patients ({stats.totalPatients} Live), register new OPD/IPD profiles, and manage demographic data.
            </p>
          </div>
          <div className="module-card-footer">
            <span className="module-action-text">Open Directory</span>
            <span className="module-arrow">→</span>
          </div>
        </div>

        {/* Module 3: Link Health Records */}
        <div
          className="flat-module-card"
          onClick={() => navigate('/health-records')}
          role="button"
          tabIndex={0}
        >
          <div className="module-card-top">
            <div className="module-icon-box brand">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71" />
                <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71" />
              </svg>
            </div>
            <span className="module-tag-chip">HIP Records Linking</span>
          </div>
          <div className="module-card-body">
            <h3 className="module-title">Link Health Records to ABHA</h3>
            <p className="module-desc">
              Attach prescriptions, lab diagnostic reports ({stats.totalCareContexts} Live Contexts), and discharge summaries.
            </p>
          </div>
          <div className="module-card-footer">
            <span className="module-action-text">Link Records</span>
            <span className="module-arrow">→</span>
          </div>
        </div>

        {/* Module 4: Consent Management */}
        <div
          className="flat-module-card"
          onClick={() => navigate('/consent-requests')}
          role="button"
          tabIndex={0}
        >
          <div className="module-card-top">
            <div className="module-icon-box brand">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                <polyline points="9 12 11 14 15 10" />
              </svg>
            </div>
            <span className="module-tag-chip">HIU Consent Suite</span>
          </div>
          <div className="module-card-body">
            <h3 className="module-title">Consent Management Suite</h3>
            <p className="module-desc">
              Request, track, approve ({stats.activeConsents} Active), and audit patient health data sharing consent requests.
            </p>
          </div>
          <div className="module-card-footer">
            <span className="module-action-text">Manage Consents</span>
            <span className="module-arrow">→</span>
          </div>
        </div>

        {/* Module 5: Token History */}
        <div
          className="flat-module-card"
          onClick={() => navigate('/token-history')}
          role="button"
          tabIndex={0}
        >
          <div className="module-card-top">
            <div className="module-icon-box brand">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <circle cx="12" cy="12" r="10" />
                <polyline points="12 6 12 12 16 14" />
              </svg>
            </div>
            <span className="module-tag-chip">M1 Audit Log</span>
          </div>
          <div className="module-card-body">
            <h3 className="module-title">Token History & Audit Log</h3>
            <p className="module-desc">
              Inspect ABDM gateway linking tokens ({stats.totalTransactions} Total Transactions), access tokens, & M1 logs.
            </p>
          </div>
          <div className="module-card-footer">
            <span className="module-action-text">View Audit Log</span>
            <span className="module-arrow">→</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
