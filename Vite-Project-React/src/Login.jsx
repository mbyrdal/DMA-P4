import { Link, useNavigate } from "react-router-dom";
import React, { useState } from "react";
import { useUser } from "./UserContext"; // sørg for korrekt sti

function Login() {
  const { login } = useUser(); // fra context
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch("https://localhost:7092/api/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        if (response.status === 401) {
          setError("Forkert e-mail eller adgangskode.");
        } else {
          setError("Noget gik galt under login.");
        }
        return;
      }

      const user = await response.json(); // { email, role, token }

      login(user);              // Store token and user info in context and localStorage
      setError("");
      navigate("/");            // Redirect to homepage or dashboard
    } catch (err) {
      console.error("Login-fejl:", err);
      setError("Der opstod en fejl ved login.");
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
        backgroundColor: "#121212",
        color: "white",
      }}
    >
      <div style={{ position: "absolute", top: "20px", right: "20px" }}>
        <Link to="/admin-login">
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
            Admin Login
          </button>
        </Link>
      </div>

      <div style={{ backgroundColor: "#1e1e1e", padding: "2rem", borderRadius: "10px" }}>
        <h2>Log ind</h2>
        <form onSubmit={handleSubmit} style={{ marginTop: "1rem" }}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Indtast e-mail"
            style={{ padding: "0.5rem", marginBottom: "1rem", width: "100%" }}
            required
          />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Indtast adgangskode"
            style={{ padding: "0.5rem", marginBottom: "1rem", width: "100%" }}
            required
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

export default Login;
