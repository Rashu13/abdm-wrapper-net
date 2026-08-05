import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import api from '@/api/abhaApi';

interface HeaderProps {
  userName: string;
  onLogout: () => void;
}

const Header: React.FC<HeaderProps> = ({ userName, onLogout }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const currentPath = location.pathname;

  const [firmName, setFirmName] = useState<string | null>(null);
  const [firmLogo, setFirmLogo] = useState<string | null>(null);
  const [liveFullName, setLiveFullName] = useState<string | null>(null);

  const fetchHeaderSettings = async () => {
    try {
      const [firmResp, userResp] = await Promise.all([
        api.get('/api/settings/firm').catch(() => null),
        api.get('/api/settings/user-profile').catch(() => null),
      ]);

      if (firmResp && firmResp.data) {
        setFirmName(firmResp.data.firmName || null);
        setFirmLogo(firmResp.data.firmLogo || null);
      }
      if (userResp && userResp.data) {
        setLiveFullName(userResp.data.fullName || null);
      }
    } catch {
      // Fallback
    }
  };

  useEffect(() => {
    fetchHeaderSettings();

    const handleSettingsUpdated = () => {
      fetchHeaderSettings();
    };

    window.addEventListener('firmSettingsUpdated', handleSettingsUpdated);
    return () => {
      window.removeEventListener('firmSettingsUpdated', handleSettingsUpdated);
    };
  }, []);

  const displayName = liveFullName !== null ? liveFullName : (userName || '');

  const navItems = [
    {
      to: '/dashboard',
      label: 'Dashboard',
      isActive: currentPath === '/dashboard' || currentPath === '/',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <rect x="3" y="3" width="7" height="7" rx="1.5" /><rect x="14" y="3" width="7" height="7" rx="1.5" />
          <rect x="14" y="14" width="7" height="7" rx="1.5" /><rect x="3" y="14" width="7" height="7" rx="1.5" />
        </svg>
      ),
    },
    {
      to: '/abha/create',
      label: 'Create ABHA',
      isActive: currentPath === '/abha/create',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <rect x="3" y="4" width="18" height="16" rx="3" />
          <circle cx="9" cy="10" r="2" />
          <line x1="15" y1="9" x2="17" y2="9" /><line x1="15" y1="12" x2="17" y2="12" />
          <line x1="7" y1="16" x2="17" y2="16" />
        </svg>
      ),
    },
    {
      to: '/patients',
      label: 'Patients',
      isActive: currentPath === '/patients',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
          <path d="M23 21v-2a4 4 0 0 0-3-3.87" /><path d="M16 3.13a4 4 0 0 1 0 7.75" />
        </svg>
      ),
    },
    {
      to: '/health-records',
      label: 'Health Records',
      isActive: currentPath === '/health-records',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
          <polyline points="14 2 14 8 20 8" /><line x1="12" y1="18" x2="12" y2="12" /><line x1="9" y1="15" x2="15" y2="15" />
        </svg>
      ),
    },
    {
      to: '/consent-requests',
      label: 'Consent Requests',
      isActive: currentPath.includes('/consent'),
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" /><polyline points="9 12 11 14 15 10" />
        </svg>
      ),
    },
    {
      to: '/token-history',
      label: 'Token History',
      isActive: currentPath === '/token-history',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <circle cx="12" cy="12" r="10" /><polyline points="12 6 12 12 16 14" />
        </svg>
      ),
    },
    {
      to: '/settings',
      label: 'Settings',
      isActive: currentPath === '/settings',
      icon: (
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
          <circle cx="12" cy="12" r="3" />
          <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z" />
        </svg>
      ),
    },
  ];

  return (
    <header className="sticky top-0 z-50 h-16 bg-white/95 backdrop-blur-md border-b border-slate-200/90 shadow-sm flex items-center justify-between px-6 transition-all">
      {/* Left Side: Brand Logo & Firm Name */}
      <div className="flex items-center gap-7">
        <div
          className="flex items-center gap-2.5 cursor-pointer group"
          onClick={() => navigate('/dashboard')}
        >
          {firmLogo ? (
            <img
              src={firmLogo}
              alt="Firm Logo"
              className="h-8 maxWidth-[130px] object-contain"
            />
          ) : (
            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-[#037BBA] to-[#026296] text-white flex items-center justify-center font-black text-lg shadow-md shadow-brand/20 group-hover:scale-105 transition-transform">
              P
            </div>
          )}

          <div className="flex flex-col">
            <span className="text-base font-extrabold text-slate-800 tracking-tight leading-none group-hover:text-brand transition-colors">
              {firmName ? firmName : 'PathoDoc'}
            </span>
            <span className="text-[10px] font-bold text-sky-600 tracking-widest uppercase mt-0.5">
              ABDM Portal
            </span>
          </div>
        </div>

        {/* Center Nav Links */}
        <nav className="hidden lg:flex items-center gap-1.5 bg-slate-100/70 p-1.5 rounded-full border border-slate-200/60 shadow-inner">
          {navItems.map((item) => (
            <Link
              key={item.to}
              to={item.to}
              className={`flex items-center gap-1.5 px-3.5 py-1.5 rounded-full text-xs font-bold transition-all duration-200 cursor-pointer ${
                item.isActive
                  ? 'text-white shadow-md'
                  : 'text-slate-600 hover:text-slate-900 hover:bg-white/80'
              }`}
              style={
                item.isActive
                  ? { background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }
                  : {}
              }
            >
              <span className={item.isActive ? 'text-white' : 'text-slate-400 group-hover:text-slate-600'}>
                {item.icon}
              </span>
              <span>{item.label}</span>
            </Link>
          ))}
        </nav>
      </div>

      {/* Right Side: Profile & Logout */}
      <div className="flex items-center gap-3">
        {displayName && (
          <div className="bg-slate-50 border border-slate-200/90 rounded-full py-1.5 px-3.5 flex items-center gap-2.5 shadow-sm">
            <div className="relative flex items-center justify-center">
              <div className="w-7 h-7 rounded-full bg-gradient-to-br from-[#037BBA] to-[#026296] text-white flex items-center justify-center font-extrabold text-xs shadow-inner">
                {displayName.charAt(0).toUpperCase()}
              </div>
              <span className="absolute -bottom-0.5 -right-0.5 w-2.5 h-2.5 bg-emerald-500 border-2 border-white rounded-full" title="Online" />
            </div>
            <div className="flex items-center gap-1">
              <span className="text-xs text-slate-400 font-medium">Hi,</span>
              <span className="text-xs font-bold text-slate-800 tracking-tight max-w-[130px] truncate">
                {displayName}
              </span>
            </div>
          </div>
        )}

        <button
          onClick={onLogout}
          title="Logout"
          className="flex items-center gap-1.5 text-xs font-bold text-slate-600 hover:text-red-600 bg-white border border-slate-200 hover:border-red-200 hover:bg-red-50/50 rounded-full px-3.5 py-1.5 transition-all duration-200 shadow-sm cursor-pointer"
        >
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
            <polyline points="16 17 21 12 16 7" />
            <line x1="21" y1="12" x2="9" y2="12" />
          </svg>
          <span>Logout</span>
        </button>
      </div>
    </header>
  );
};

export default Header;
