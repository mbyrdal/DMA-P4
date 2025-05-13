import { Link } from "react-router-dom";
import React, { useState } from "react";
import "./Login.css";

function AdminLogin({ onAdminLogin }) {
  const [adminCode, setAdminCode] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (adminCode === "admin1234") {
      onAdminLogin();
      localStorage.setItem("isAdminLoggedIn", "true");
      setError("");
    } else {
      setError("Forkert admin-kode. Prøv igen.");
    }
  };

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit}>
        <h2>🔐 Admin Login</h2>

        <input
          type="password"
          placeholder="Indtast admin-kode"
          value={adminCode}
          onChange={(e) => setAdminCode(e.target.value)}
          required
        />
        
        <button type="submit">Log ind</button>

        {error && <p style={{ color: "red", marginTop: "0.5rem" }}>{error}</p>}
        <Link to="/login">
          <button type="button" style={{ marginTop: "1rem", backgroundColor: "#6c757d" }}>
            Tilbage
          </button>
        </Link>
      </form>
    </div>
  );
}

export default AdminLogin;
