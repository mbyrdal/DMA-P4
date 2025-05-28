import { Link, useNavigate } from "react-router-dom";
import React, { useState } from "react";
import { useUser } from "./UserContext";

function Login() {
  const { login } = useUser();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const [isHovering, setIsHovering] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isDarkMode, setIsDarkMode] = useState(true);

  const handleSubmit = async (e) => {
    e.preventDefault();

    setIsLoading(true);
    try {
      const audience = window.location.origin;

      const response = await fetch("https://localhost:7092/api/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password, audience }),
      });

      if (!response.ok) {
        if (response.status === 401) {
          setError("Forkert e-mail eller adgangskode.");
        } else {
          setError("Noget gik galt under login.");
        }
        return;
      }

      const user = await response.json();

      login(user);
      setError("");
      navigate("/");
    } catch (err) {
      console.error("Login-fejl:", err);
      setError("Der opstod en fejl ved login.");
    } finally {
      setIsLoading(false);
    }
  };

  const themeStyles = {
    backgroundColor: isDarkMode ? "#121212" : "#f4f4f4",
    textColor: isDarkMode ? "white" : "#121212",
    cardBg: isDarkMode ? "#1e1e1e" : "#fff",
    inputBg: isDarkMode ? "#2a2a2a" : "#f9f9f9",
    buttonBg: "#007BFF",
    buttonHoverBg: "#3399FF",
  };

  return (
    <div
      style={{
        fontFamily: "Arial, sans-serif",
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        position: "relative",
        backgroundColor: themeStyles.backgroundColor,
        color: themeStyles.textColor,
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

      {/* Link to admin login */}
      <div style={{ position: "absolute", top: "20px", right: "20px" }}>
        <Link to="/admin-login">
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
            Admin Login
          </button>
        </Link>
      </div>

      {/* Login card */}
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

        <p style={{ fontSize: "0.9rem", color: "#bbb", marginBottom: "1.5rem" }}>
          Velkommen tilbage - Ansatte
        </p>

        <form onSubmit={handleSubmit} style={{ marginTop: "0" }}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Indtast e-mail"
            style={{
              padding: "0.5rem",
              marginBottom: "1rem",
              width: "100%",
              boxSizing: "border-box",
              backgroundColor: themeStyles.inputBg,
              border: "1px solid #444",
              borderRadius: "5px",
              color: themeStyles.textColor,
              outline: "none",
              transition: "outline 0.2s ease",
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
              padding: "0.5rem",
              marginBottom: "1rem",
              width: "100%",
              boxSizing: "border-box",
              backgroundColor: themeStyles.inputBg,
              border: "1px solid #444",
              borderRadius: "5px",
              color: themeStyles.textColor,
              outline: "none",
              transition: "outline 0.2s ease",
            }}
            onFocus={(e) => (e.target.style.outline = "2px solid white")}
            onBlur={(e) => (e.target.style.outline = "none")}
            required
          />
          <br />
          <button
            type="submit"
            disabled={isLoading}
            style={{
              backgroundColor: isHovering
                ? themeStyles.buttonHoverBg
                : themeStyles.buttonBg,
              color: "white",
              border: "none",
              padding: "0.6rem 1.2rem",
              borderRadius: "5px",
              cursor: isLoading ? "not-allowed" : "pointer",
              width: "100%",
              boxSizing: "border-box",
              transition: "background-color 0.25s ease, box-shadow 0.25s ease",
              boxShadow: isHovering
                ? "0 4px 10px rgba(0, 123, 255, 0.25)"
                : "0 2px 5px rgba(0, 0, 0, 0.1)",
              opacity: isLoading ? 0.7 : 1,
            }}
            onMouseEnter={() => setIsHovering(true)}
            onMouseLeave={() => setIsHovering(false)}
          >
            {isLoading ? "Logger ind..." : "Log ind"}
          </button>
        </form>

        {/* This section is unchanged */}
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

        {error && (
          <p
            style={{
              color: "red",
              marginTop: "1rem",
              fontSize: "0.9rem",
            }}
          >
            {error}
          </p>
        )}
      </div>
    </div>
  );
}

export default Login;