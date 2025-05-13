import React from "react";
import { useNavigate } from "react-router-dom";

function LogoutButton({ onLogout, isAdmin = false }) {
  const navigate = useNavigate();

  const handleLogout = () => {
    onLogout();
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("isAdminLoggedIn");
    navigate(isAdmin ? "/admin-login" : "/login");
  };

  return (
    <button
      onClick={handleLogout}
      style={{
        padding: "0.6rem 1.2rem",
        backgroundColor: "#D9534F",
        color: "white",
        border: "none",
        borderRadius: "6px",
        cursor: "pointer",
        fontWeight: "bold"
      }}
    >
      Log ud
    </button>
  );
}

export default LogoutButton;
