import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useUser } from "./UserContext"; // Sørg for korrekt sti

function AdminLogin() {
  const { login } = useUser();
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
        setError("Forkert e-mail eller adgangskode.");
        return;
      }

      const user = await response.json();
        user.role = user.role.toLowerCase(); // sikrer case-matching

        if (user.role !== "admin") {
          setError("Kun administratorer har adgang til dette område.");
          return;
        }



      login(user); // Gem bruger i context
      navigate("/admin-dashboard");
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
        backgroundColor: "#121212",
        color: "white",
      }}
    >
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
            Bruger Login
          </button>
        </Link>
      </div>

      <div style={{ backgroundColor: "#1e1e1e", padding: "2rem", borderRadius: "10px" }}>
        <h2>Admin Login</h2>
        <form onSubmit={handleSubmit} style={{ marginTop: "1rem" }}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Indtast admin e-mail"
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
            Log ind som admin
          </button>
        </form>
        {error && <p style={{ color: "red", marginTop: "1rem" }}>{error}</p>}
      </div>
    </div>
  );
}

export default AdminLogin;
