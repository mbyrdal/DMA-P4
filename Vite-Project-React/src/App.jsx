import React from 'react';
import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { useUser } from './UserContext';
import Login from './Login';
import EquipmentOverview from './EquipmentOverview';
import LoanHistory from './LoanHistory';
import AdminLogin from './AdminLogin';
import AdminDashboard from './AdminDashboard';
import LogoutButton from './LogoutButton';

function App() {
  const { user, logout } = useUser();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const isLoggedIn = user !== null;
  const isAdmin = user?.role === "admin";

  return (
    <div className="main-container">
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/admin-login" element={<AdminLogin />} />

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
            isLoggedIn && isAdmin ? (
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
