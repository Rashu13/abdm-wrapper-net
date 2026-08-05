import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { ROUTES } from '@/constants/app.constants';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  section?: string;
}

const NAV_ITEMS: NavItem[] = [
  {
    path: ROUTES.HOME,
    label: 'Dashboard',
    icon: '🏠',
    section: 'Main',
  },
  {
    path: ROUTES.ABHA_CREATE,
    label: 'ABHA Create & Verify',
    icon: '🏥',
    section: 'ABHA Services',
  },
];

const Sidebar: React.FC = () => {
  const location = useLocation();

  const sections = NAV_ITEMS.reduce<Record<string, NavItem[]>>((acc, item) => {
    const section = item.section || 'General';
    if (!acc[section]) acc[section] = [];
    acc[section].push(item);
    return acc;
  }, {});

  return (
    <aside className="app-sidebar" role="navigation" aria-label="Main navigation">
      {/* Logo */}
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon" aria-hidden="true">
          🩺
        </div>
        <div className="sidebar-logo-text">
          <span className="sidebar-logo-title">ABHA</span>
          <span className="sidebar-logo-subtitle">NHA Portal</span>
        </div>
      </div>

      {/* Navigation */}
      <nav className="sidebar-nav">
        {Object.entries(sections).map(([section, items]) => (
          <div key={section}>
            <div className="sidebar-section-label">{section}</div>
            {items.map((item) => {
              const isActive =
                item.path === ROUTES.HOME
                  ? location.pathname === ROUTES.HOME
                  : location.pathname.startsWith(item.path);

              return (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`sidebar-nav-item ${isActive ? 'active' : ''}`}
                  aria-current={isActive ? 'page' : undefined}
                >
                  <span className="nav-icon" aria-hidden="true">
                    {item.icon}
                  </span>
                  {item.label}
                </Link>
              );
            })}
          </div>
        ))}
      </nav>

      {/* Footer */}
      <div className="sidebar-footer">
        <div className="sidebar-footer-text">
          <div>Powered by</div>
          <div style={{ color: 'rgba(255,255,255,0.55)', marginTop: 2 }}>
            National Health Authority
          </div>
          <div style={{ marginTop: 6, fontSize: 10, color: 'rgba(255,255,255,0.25)' }}>
            © {new Date().getFullYear()} NHA. All rights reserved.
          </div>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;
