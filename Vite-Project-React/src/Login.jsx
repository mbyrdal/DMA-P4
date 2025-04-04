import React, { useState } from 'react';

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
    <div className="login-box">
      <h2>Log ind</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="password"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          placeholder="Indtast kode"
        />
        <button type="submit">Log ind</button>
      </form>
      {error && <p>{error}</p>}
    </div>
  );
}

export default Login;