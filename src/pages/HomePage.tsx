import React from 'react';
import { Link } from 'react-router-dom';
import { ROUTES } from '@/constants/app.constants';

interface PortalOption {
  id: string;
  path: string;
  icon: string;
  title: string;
  description: string;
  tag: string;
  tagIcon: string;
  comingSoon?: boolean;
}

const PORTAL_OPTIONS: PortalOption[] = [
  {
    id: 'abha-create-verify',
    path: ROUTES.ABHA_CREATE,
    icon: '🏥',
    title: 'ABHA Create & Verify',
    description:
      'Create a new Ayushman Bharat Health Account or verify an existing ABHA number using Aadhaar-based OTP authentication.',
    tag: 'Active',
    tagIcon: '✅',
  },
  {
    id: 'abha-profile',
    path: '#',
    icon: '👤',
    title: 'ABHA Profile Management',
    description:
      'View and manage ABHA profile details, linked health records, and account settings.',
    tag: 'Coming Soon',
    tagIcon: '🔜',
    comingSoon: true,
  },
  {
    id: 'health-records',
    path: '#',
    icon: '📁',
    title: 'Health Records',
    description:
      'Access and manage digitally linked health records from multiple healthcare providers.',
    tag: 'Coming Soon',
    tagIcon: '🔜',
    comingSoon: true,
  },
];

const HomePage: React.FC = () => {
  return (
    <main className="home-page animate-fade-in-up" aria-label="ABHA Portal Dashboard">
      {/* Hero Banner */}
      <section className="home-hero animate-fade-in-up" aria-labelledby="hero-title">
        <div className="home-hero-content">
          <div className="home-hero-badge" aria-label="Powered by National Health Authority">
            <span aria-hidden="true">🇮🇳</span>
            National Health Authority — ABDM
          </div>

          <h1 className="home-hero-title" id="hero-title">
            Ayushman Bharat<br />
            <span style={{ color: 'rgba(255,255,255,0.75)' }}>Health Account Portal</span>
          </h1>

          <p className="home-hero-subtitle">
            A unified digital health identity platform enabling secure, consent-based health data
            sharing across India's healthcare ecosystem.
          </p>

          <div className="home-hero-stats" role="list" aria-label="Key statistics">
            <div className="stat-item" role="listitem">
              <span className="stat-value">56 Cr+</span>
              <span className="stat-label">ABHA Numbers</span>
            </div>
            <div className="stat-item" role="listitem">
              <span className="stat-value">27K+</span>
              <span className="stat-label">Linked Facilities</span>
            </div>
            <div className="stat-item" role="listitem">
              <span className="stat-value">100%</span>
              <span className="stat-label">Secure & Encrypted</span>
            </div>
          </div>
        </div>
      </section>

      {/* Options Grid */}
      <section aria-labelledby="services-title">
        <h2 className="home-options-title" id="services-title">
          <span aria-hidden="true">⚡</span> Available Services
        </h2>

        <div className="home-options-grid" role="list">
          {PORTAL_OPTIONS.map((option, index) => {
            const CardContent = (
              <>
                <div className="option-card-icon" aria-hidden="true">
                  {option.icon}
                </div>
                <div className="option-card-body">
                  <div className="option-card-title">{option.title}</div>
                  <p className="option-card-desc">{option.description}</p>
                  <div style={{ marginTop: 'var(--spacing-3)' }}>
                    <span className="option-card-tag">
                      <span aria-hidden="true">{option.tagIcon}</span>
                      {option.tag}
                    </span>
                  </div>
                </div>
                {!option.comingSoon && (
                  <div className="option-card-arrow" aria-hidden="true">→</div>
                )}
              </>
            );

            return (
              <div
                key={option.id}
                role="listitem"
                className={`animate-fade-in-up delay-${index + 1}`}
              >
                {option.comingSoon ? (
                  <div
                    className="option-card"
                    style={{ opacity: 0.65, cursor: 'default' }}
                    aria-label={`${option.title} — coming soon`}
                  >
                    {CardContent}
                  </div>
                ) : (
                  <Link
                    to={option.path}
                    className="option-card"
                    aria-label={`Go to ${option.title}`}
                    id={`portal-option-${option.id}`}
                  >
                    {CardContent}
                  </Link>
                )}
              </div>
            );
          })}
        </div>
      </section>
    </main>
  );
};

export default HomePage;
