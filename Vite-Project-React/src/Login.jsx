import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Login.css";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleLogin = (e) => {
    e.preventDefault();
    if (username === "admin" && password === "admin") {
      navigate("/dashboard/overview");
    } else {
      alert("Forkert brugernavn eller adgangskode");
    }
  };

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleLogin}>
        <h2>🔐 Log ind på WEXO Depot</h2>
        <input
          type="text"
          placeholder="Brugernavn"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Adgangskode"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Log ind</button>
      </form>
    </div>
  );
}

export default Login;
