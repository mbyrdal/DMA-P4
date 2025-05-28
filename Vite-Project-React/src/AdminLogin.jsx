import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useUser } from "./UserContext";

function AdminLogin() {
  const { login } = useUser();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isHovering, setIsHovering] = useState(false);
  const [isDarkMode, setIsDarkMode] = useState(true);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const audience = window.location.origin;

      const response = await fetch("https://localhost:7092/api/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, audience }),
      });

      if (!response.ok) {
        setError("Forkert e-mail eller adgangskode.");
        return;
      }

      const user = await response.json();
      user.role = user.role.toLowerCase();

      if (user.role !== "admin") {
        setError("Kun administratorer har adgang til dette område.");
        return;
      }

      login(user);
      navigate("/admin-dashboard");
    } catch (err) {
      console.error("Login-fejl:", err);
      setError("Der opstod en fejl ved login.");
    }
  };

  const themeStyles = {
    backgroundColor: isDarkMode ? "#121212" : "#f4f4f4",
    textColor: isDarkMode ? "#ffffff" : "#121212",
    cardBg: isDarkMode ? "#1e1e1e" : "#ffffff",
    inputBg: isDarkMode ? "#2a2a2a" : "#f9f9f9",
    linkColor: isDarkMode ? "#9ecfff" : "#007BFF",
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
        backgroundColor: themeStyles.backgroundColor,
        color: themeStyles.textColor,
        position: "relative",
        transition: "background-color 0.3s ease, color 0.3s ease",
      }}
    >
      {/* Theme toggle */}
      <button
        onClick={() => setIsDarkMode(!isDarkMode)}
        style={{
          position: "absolute",
          top: 20,
          left: 20,
          background: "transparent",
          border: `1px solid ${isDarkMode ? "#ccc" : "#333"}`,
          color: themeStyles.textColor,
          padding: "0.4rem 0.8rem",
          borderRadius: "20px",
          cursor: "pointer",
          fontSize: "0.8rem",
          transition: "all 0.3s ease", // smooth transition
        }}
        onMouseEnter={e => {
          e.currentTarget.style.backgroundColor = isDarkMode ? "#ccc" : "#333";
          e.currentTarget.style.color = isDarkMode ? "#121212" : "#fff";
          e.currentTarget.style.borderColor = isDarkMode ? "#bbb" : "#222";
        }}
         onMouseLeave={e => {
          e.currentTarget.style.backgroundColor = "transparent";
          e.currentTarget.style.color = themeStyles.textColor;
          e.currentTarget.style.borderColor = isDarkMode ? "#ccc" : "#333";
         }}
      >
        {isDarkMode ? "☀️ Light Mode" : "🌙 Dark Mode"}
      </button>

      {/* Back to user login */}
      <div style={{ position: "absolute", top: "20px", right: "20px" }}>
        <Link to="/login">
          <button
            style={{
              backgroundColor: "#6c757d",
              color: "white",
              border: "none",
              padding: "0.5rem 1rem",
              borderRadius: "5px",
              cursor: "pointer",
            }}
          >
            Bruger Login
          </button>
        </Link>
      </div>

      {/* Login Card */}
      <div
        style={{
          backgroundColor: themeStyles.cardBg,
          padding: "2rem 2.5rem",
          borderRadius: "10px",
          width: "320px",
          boxSizing: "border-box",
          textAlign: "center",
          boxShadow: "0 0 10px rgba(0, 0, 0, 0.5)",
          transition: "background-color 0.3s ease",
        }}
      >
        {/* Logo */}
        <img
          src="/wexo_logo.svg"
          alt="Wexo Company Logo"
          style={{
            width: "160px",
            marginBottom: "1.5rem",
            filter: isDarkMode
              ? "drop-shadow(0 0 4px rgba(255,255,255,0.15))"
              : "drop-shadow(0 0 2px rgba(0,0,0,0.1))"
          }}
        />

        {/* Paragraph text */}
        <p style={{ fontSize: "0.9rem", color: "#bbb", marginBottom: "1.5rem" }}>
          Velkommen tilbage - Administratorer
        </p>

        {/* Form */}
        <form onSubmit={handleSubmit}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Indtast e-mail"
            style={{
              backgroundColor: themeStyles.inputBg,
              color: themeStyles.textColor,
              border: "1px solid #444",
              borderRadius: "4px",
              padding: "0.5rem",
              marginBottom: "1rem",
              width: "100%",
              boxSizing: "border-box",
              outline: "none",
            }}
            onFocus={(e) => (e.target.style.outline = "2px solid white")}
            onBlur={(e) => (e.target.style.outline = "none")}
            required
          />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Indtast adgangskode"
            style={{
              backgroundColor: themeStyles.inputBg,
              color: themeStyles.textColor,
              border: "1px solid #444",
              borderRadius: "4px",
              padding: "0.5rem",
              marginBottom: "1rem",
              width: "100%",
              boxSizing: "border-box",
              outline: "none",
            }}
            onFocus={(e) => (e.target.style.outline = "2px solid white")}
            onBlur={(e) => (e.target.style.outline = "none")}
            required
          />
          <button
            type="submit"
            style={{
              backgroundColor: isHovering ? "#3399FF" : "#007BFF",
              color: "white",
              border: "none",
              padding: "0.6rem 1.2rem",
              borderRadius: "5px",
              cursor: "pointer",
              width: "100%",
              boxSizing: "border-box",
              transition: "background-color 0.25s ease, box-shadow 0.25s ease",
              boxShadow: isHovering
                ? "0 4px 10px rgba(0, 123, 255, 0.25)"
                : "0 2px 5px rgba(0, 0, 0, 0.1)",
            }}
            onMouseEnter={() => setIsHovering(true)}
            onMouseLeave={() => setIsHovering(false)}
          >
            Log ind
          </button>
        </form>

        {/* Error */}
        {error && (
          <p style={{ color: "red", marginTop: "1rem", fontSize: "0.9rem" }}>
            {error}
          </p>
        )}

        {/* Forgot password link */}
        <div style={{ marginTop: "1rem" }}>
          <Link
          to="/forgot-password"
          style={{
            color: "#888",
            fontSize: "0.85rem",
            textDecoration: "none",
            }}>
            Glemt adgangskode?
          </Link>
        </div>
      </div>
    </div>
  );
}

export default AdminLogin;