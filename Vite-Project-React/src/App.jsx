// src/App.jsx
import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import axios from 'axios';
import PWAPrompt from 'react-ios-pwa-prompt';

import Login from './Login';
import EquipmentOverview from './EquipmentOverview';
import LoanHistory from './LoanHistory';
import AdminLogin from './AdminLogin';
import AdminDashboard from './AdminDashboard';
import LogoutButton from './LogoutButton';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAdminLoggedIn, setIsAdminLoggedIn] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [showPrompt, setShowPrompt] = useState(false);
  const [deferredPrompt, setDeferredPrompt] = useState(null);
  const [showAndroidPrompt, setShowAndroidPrompt] = useState(false);

  const navigate = useNavigate();

  // Login states
  useEffect(() => {
    const loggedIn = localStorage.getItem("isLoggedIn") === "true";
    const adminLoggedIn = localStorage.getItem("isAdminLoggedIn") === "true";
    setIsLoggedIn(loggedIn);
    setIsAdminLoggedIn(adminLoggedIn);
    setIsLoading(false);
  }, []);

  const handleLogin = () => {
    setIsLoggedIn(true);
    localStorage.setItem("isLoggedIn", "true");
    navigate("/");
  };

  const handleAdminLogin = () => {
    setIsAdminLoggedIn(true);
    localStorage.setItem("isAdminLoggedIn", "true");
    navigate("/admin-dashboard");
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    setIsAdminLoggedIn(false);
    localStorage.setItem("isLoggedIn", "false");
    localStorage.setItem("isAdminLoggedIn", "false");
    navigate("/login");
  };

  // PWA Prompt (iOS)
  useEffect(() => {
    const isIOS = /iphone|ipad|ipod/.test(window.navigator.userAgent.toLowerCase());
    const isInStandaloneMode = 'standalone' in window.navigator && window.navigator.standalone;

    if (isIOS && !isInStandaloneMode) {
      setShowPrompt(true);
    }
  }, []);

  // Android install prompt
  useEffect(() => {
    const handler = (e) => {
      e.preventDefault();
      setDeferredPrompt(e);
      setShowAndroidPrompt(true);
    };

    window.addEventListener('beforeinstallprompt', handler);
    return () => window.removeEventListener('beforeinstallprompt', handler);
  }, []);

  const handleInstallClick = async () => {
    if (deferredPrompt) {
      deferredPrompt.prompt();
      const { outcome } = await deferredPrompt.userChoice;
      console.log('User install choice:', outcome);
      setDeferredPrompt(null);
      setShowAndroidPrompt(false);
    }
  };

  if (isLoading) return null;

  return (
    <div className="main-container">
      <h1 style={{ fontSize: window.innerWidth < 600 ? '18px' : '24px' }}>Hello World</h1>

      {showPrompt && <PWAPrompt />}
      {showAndroidPrompt && (
        <button onClick={handleInstallClick}>
          Install App
        </button>
      )}

      <Routes>
        <Route path="/login" element={<Login onLogin={handleLogin} />} />
        <Route path="/admin-login" element={<AdminLogin onAdminLogin={handleAdminLogin} />} />
        <Route
          path="/history"
          element={isLoggedIn ? <LoanHistory /> : <Navigate to="/login" />}
        />
        <Route
          path="/"
          element={
            isLoggedIn ? (
              <>
                <EquipmentOverview />
                <LogoutButton onLogout={handleLogout} />
              </>
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin-dashboard"
          element={
            isAdminLoggedIn ? (
              <>
                <AdminDashboard />
                <LogoutButton onLogout={handleLogout} />
              </>
            ) : (
              <Navigate to="/admin-login" />
            )
          }
        />
      </Routes>
    </div>
  );
}

export default App;
