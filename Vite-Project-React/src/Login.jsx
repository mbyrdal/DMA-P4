import { Link } from "react-router-dom";
import React, { useState } from "react";
import "./Login.css";

function Login({ onLogin }) {
  const [code, setCode] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (code === "1234") {
      onLogin();
      localStorage.setItem("isLoggedIn", "true");
      setError("");
    } else {
      setError("Forkert kode. Prøv igen.");
    }
  };

  return (
    <div className="login-container">
      <div style={{ position: "absolute", top: "20px", right: "20px" }}>
        <Link to="/admin-login">
          <button style={{ backgroundColor: "#6c757d", color: "white" }}>
            Admin Login
          </button>
        </Link>
      </div>

      <form className="login-form" onSubmit={handleSubmit}>
        <h2>🔐 Bruger Login</h2>

        <input
          type="password"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          placeholder="Indtast kode"
          required
        />

        <button type="submit">Log ind</button>

        {error && <p style={{ color: "red", marginTop: "0.5rem" }}>{error}</p>}
      </form>
    </div>
  );
}

export default Login;
