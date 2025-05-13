import React, { useState, useEffect } from 'react';
import { Routes, Route, Link, useNavigate } from 'react-router-dom';
import AdminOverviewTab from './AdminOverviewTab';
import AdminUsersTab from './AdminUsersTab';
import AdminEquipmentTab from './AdminEquipmentTab';
import AdminHistoryTab from './AdminHistoryTab';
import './Dashboard.css';

function AdminDashboard() {
  const [activeReservations, setActiveReservations] = useState(0);
  const [activeLoans, setActiveLoans] = useState(0);
  const [lowStockItems, setLowStockItems] = useState([]);
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    // API-kald
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => {
        const active = data.filter(res => !res.isConvertedToLoan).length;
        setActiveReservations(active);
      });

    fetch("https://localhost:7092/api/loan")
      .then(res => res.json())
      .then(data => {
        const active = data.filter(loan => !loan.returned).length;
        setActiveLoans(active);
      });

    fetch("https://localhost:7092/api/backend")
      .then(res => res.json())
      .then(data => {
        const lowStock = data.filter(item => item.antal < 6);
        setLowStockItems(lowStock);
      });
  }, []);

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
          <Route path="overview" element={
            <AdminOverviewTab
              activeReservations={activeReservations}
              activeLoans={activeLoans}
              lowStockItems={lowStockItems}
            />
          } />
          <Route path="equipment" element={<AdminEquipmentTab />} />
          <Route path="users" element={<AdminUsersTab />} />
          <Route path="history" element={<AdminHistoryTab />} />
        </Routes>
      </main>
    </div>
  );
}

export default AdminDashboard;
