import React, { useEffect, useState } from "react";
import { Routes, Route, Link } from "react-router-dom";
import AdminUsersTab from "./AdminUsersTab";
import AdminEquipmentTab from "./AdminEquipmentTab";
import AdminHistoryTab from "./AdminHistoryTab";
import AdminOverviewTab from "./AdminOverviewTab";
import "./Dashboard.css";

function AdminDashboard() {
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    document.body.classList.toggle("dark-mode", darkMode);
  }, [darkMode]);

  return (
    <div className="dashboard">
      <aside className="sidebar">
        <h2>WEXO Depot</h2>
        <nav>
          <Link to="overview">📊 Overblik</Link>
          <Link to="equipment">🔧 Udstyr</Link>
          <Link to="users">👥 Brugere</Link>
          <Link to="history">📜 Historik</Link>
        </nav>
        <button
          onClick={() => setDarkMode(!darkMode)}
          style={{
            marginTop: "auto",
            padding: "10px",
            borderRadius: "6px",
            backgroundColor: "#16a085",
            color: "white",
            border: "none",
            cursor: "pointer"
          }}
        >
          {darkMode ? "☀️ Lys tilstand" : "🌙 Mørk tilstand"}
        </button>
      </aside>
      <main className="content">
        <Routes>
          <Route path="overview" element={<AdminOverviewTab />} />
          <Route path="equipment" element={<AdminEquipmentTab />} />
          <Route path="users" element={<AdminUsersTab />} />
          <Route path="history" element={<AdminHistoryTab />} />
        </Routes>
      </main>
    </div>
  );
}

export default AdminDashboard;
