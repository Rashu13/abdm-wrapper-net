import React, { useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { GoogleReCaptchaProvider } from 'react-google-recaptcha-v3';
import Header from '@/components/layout/Header';
import LoginPage from '@/pages/LoginPage';
import DashboardPage from '@/pages/DashboardPage';
import AbhaPage from '@/pages/AbhaPage';
import PatientsListPage from '@/pages/PatientsListPage';
import HealthRecordsPage from '@/pages/HealthRecordsPage';
import ConsentRequestsPage from '@/pages/ConsentRequestsPage';
import ConsentDetailsPage from '@/pages/ConsentDetailsPage';
import TokenHistoryPage from '@/pages/TokenHistoryPage';
import SettingsPage from '@/pages/SettingsPage';

const RECAPTCHA_SITE_KEY = import.meta.env.VITE_RECAPTCHA_SITE_KEY as string;

const App: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(() => {
    return localStorage.getItem('isLoggedIn') === 'true';
  });

  const [userName, setUserName] = useState<string>(() => {
    return localStorage.getItem('userName') || 'Dr. Rahul Sharma';
  });

  const handleLoginSuccess = (name: string) => {
    localStorage.setItem('isLoggedIn', 'true');
    localStorage.setItem('userName', name);
    setUserName(name);
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    localStorage.removeItem('isLoggedIn');
    localStorage.removeItem('userName');
    setIsAuthenticated(false);
  };

  if (!isAuthenticated) {
    return <LoginPage onLoginSuccess={handleLoginSuccess} />;
  }

  return (
    <GoogleReCaptchaProvider
      reCaptchaKey={RECAPTCHA_SITE_KEY}
      scriptProps={{
        async: true,
        defer: true,
        appendTo: 'head',
      }}
      language="en"
    >
      <BrowserRouter>
        <div className="app-layout">
          <Header userName={userName} onLogout={handleLogout} />
          <main className="app-content">
            <Routes>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/abha/create" element={<AbhaPage />} />
              <Route path="/patients" element={<PatientsListPage />} />
              <Route path="/health-records" element={<HealthRecordsPage />} />
              <Route path="/consent-requests" element={<ConsentRequestsPage />} />
              <Route path="/consent-details" element={<ConsentDetailsPage />} />
              <Route path="/consent-details/:id" element={<ConsentDetailsPage />} />
              <Route path="/token-history" element={<TokenHistoryPage />} />
              <Route path="/settings" element={<SettingsPage />} />
              <Route path="/" element={<Navigate to="/dashboard" replace />} />
              <Route path="*" element={<Navigate to="/dashboard" replace />} />
            </Routes>
          </main>
        </div>
      </BrowserRouter>
    </GoogleReCaptchaProvider>
  );
};

export default App;
