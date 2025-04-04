import React from "react";
import { useNavigate } from "react-router-dom"; // Change to useNavigate for v6+

function LogoutButton({ onLogout }) {
  const navigate = useNavigate(); // useNavigate instead of useHistory

  const handleLogout = () => {
    onLogout();
    localStorage.removeItem("isLoggedIn");
    navigate("/login"); // Redirect to login page after logout
  };

  return (
    <button
      onClick={handleLogout}
      style={{
        padding: "0.6rem 1.2rem",
        backgroundColor: "#D9534F",
        color: "white",
        border: "none",
        borderRadius: "5px",
        cursor: "pointer",
      }}
    >
      Log ud
    </button>
  );
}

export default LogoutButton;