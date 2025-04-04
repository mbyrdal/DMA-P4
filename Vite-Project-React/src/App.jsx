import React, { useState, useEffect } from 'react';
import EquipmentOverview from './EquipmentOverview';
import Login from './Login';
import LogoutButton from './LogoutButton';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    const loggedIn = localStorage.getItem("isLoggedIn") === "true";
    setIsLoggedIn(loggedIn);
  }, []);

  const handleLogin = () => {
    setIsLoggedIn(true);
    localStorage.setItem("isLoggedIn", "true");
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    localStorage.setItem("isLoggedIn", "false");
  };

  return (
    <div className="main-container">
      {isLoggedIn ? (
        <>
          <EquipmentOverview />
          <LogoutButton onLogout={handleLogout} />
        </>
      ) : (
        <Login onLogin={handleLogin} />
      )}
    </div>
  );
}

export default App;