import { Link } from "react-router-dom";
import React, { useState } from "react";

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
    <div 
      style={{
        fontFamily: "Arial",
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        position: "relative",
        backgroundColor: "#121212", // evt. samme baggrundsfarve som Main Login
        color: "white",
      }}
    >
      {/* Tilbage-knap oppe i højre hjørne */}
      <div style={{ position: "absolute", top: "20px", right: "20px" }}>
        <Link to="/login">
          <button
            style={{    
              fontFamily: "Arial",
              backgroundColor: "#6c757d",
              color: "white",
              border: "none",
              padding: "0.5rem 1rem",
              borderRadius: "5px",
              cursor: "pointer",
            }}
          >
            Tilbage
          </button>
        </Link>
      </div>

      <div style={{ backgroundColor: "#1e1e1e", padding: "2rem", borderRadius: "10px" }}>
        <h2>Admin Log ind</h2>
        <form onSubmit={handleSubmit} style={{ marginTop: "1rem" }}>
          <input
            type="password"
            value={adminCode}
            onChange={(e) => setAdminCode(e.target.value)}
            placeholder="Indtast admin-kode"
            style={{ padding: "0.5rem", marginBottom: "1rem", width: "100%" }}
          />
          <br />
          <button
            type="submit"
            style={{
              fontFamily: "Arial",
              backgroundColor: "#007BFF",
              color: "white",
              border: "none",
              padding: "0.6rem 1.2rem",
              borderRadius: "5px",
              cursor: "pointer",
              width: "100%",
            }}
          >
            Log ind
          </button>
        </form>
        {error && <p style={{ color: "red", marginTop: "1rem" }}>{error}</p>}
      </div>
    </div>
  );
}

export default AdminLogin;
