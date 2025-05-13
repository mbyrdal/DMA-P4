import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import Login from './Login';
import AdminLogin from './AdminLogin';
import EquipmentOverview from './EquipmentOverview';
import LoanHistory from './LoanHistory';
import AdminDashboard from './AdminDashboard';
import LogoutButton from './LogoutButton';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAdminLoggedIn, setIsAdminLoggedIn] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    setIsLoggedIn(localStorage.getItem("isLoggedIn") === "true");
    setIsAdminLoggedIn(localStorage.getItem("isAdminLoggedIn") === "true");
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
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("isAdminLoggedIn");
    navigate("/login");
  };

  if (isLoading) return null;

  return (
    <div className="main-container">
      <Routes>
        <Route path="/login" element={<Login onLogin={handleLogin} />} />
        <Route path="/admin-login" element={<AdminLogin onAdminLogin={handleAdminLogin} />} />

        <Route
          path="/"
          element={
            isLoggedIn ? (
              <>
                <EquipmentOverview />
                <div style={{ marginTop: "1rem" }}>
                  <LogoutButton onLogout={handleLogout} />
                </div>
              </>
            ) : (
              <Navigate to="/login" />
            )
          }
        />

        <Route
          path="/history"
          element={
            isLoggedIn ? <LoanHistory /> : <Navigate to="/login" />
          }
        />

        <Route
          path="/admin-dashboard"
          element={
            isAdminLoggedIn ? (
              <>
                <AdminDashboard />
                <div style={{ padding: "1rem" }}>
                  <LogoutButton onLogout={handleLogout} isAdmin />
                </div>
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
