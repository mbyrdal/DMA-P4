import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import Login from './Login';
import EquipmentOverview from './EquipmentOverview';
import LoanHistory from './LoanHistory';
import AdminLogin from './AdminLogin'; // Tilføjet AdminLogin
import AdminDashboard from './AdminDashboard'; // Tilføjet AdminDashboard
import LogoutButton from './LogoutButton';
import QRCodePage from "./pages/QRCodePage";


function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAdminLoggedIn, setIsAdminLoggedIn] = useState(false); // Ny admin login state
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

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

  if (isLoading) return null;

  return (
    <div className="main-container">
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

  // QR Code Scanner route

  return (
    <Routes>
      {/* ...existing routes... */}
      <Route
        path="/qr-scanner"
        element={
          isLoggedIn ? (
            <QRCodePage />
          ) : (
            <Navigate to="/login" />
          )
        }
      />
    </Routes>
  );
}

export default App;
