import React, { useState, useEffect } from 'react';
import AdminOverviewTab from './AdminOverviewTab';
import AdminUsersTab from './AdminUsersTab';
import AdminEquipmentTab from './AdminEquipmentTab';
import AdminHistoryTab from './AdminHistoryTab';

export default function AdminDashboard() {
  const [activeReservations, setActiveReservations] = useState(0);
  const [activeLoans, setActiveLoans] = useState(0);
  const [lowStockItems, setLowStockItems] = useState([]);
  const [selectedTab, setSelectedTab] = useState("overview");

  useEffect(() => {
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

  const buttonClass = (tab) => ({
    backgroundColor: selectedTab === tab ? "#007bff" : "#333",
    color: "white",
    border: "none",
    padding: "0.8rem 1.2rem",
    borderRadius: "6px",
    cursor: "pointer",
    fontSize: "1rem",
    marginRight: "0.5rem"
  });

  return (
    <div style={{ padding: "2rem", fontFamily: "Arial, sans-serif", color: "white", backgroundColor: "#121212", minHeight: "100vh" }}>
      <h1 style={{ fontSize: "2rem", fontWeight: "bold", marginBottom: "2rem" }}>Admin Dashboard</h1>

      <div style={{ marginBottom: "2rem" }}>
        <button style={buttonClass("overview")} onClick={() => setSelectedTab("overview")}>Overblik</button>
        <button style={buttonClass("users")} onClick={() => setSelectedTab("users")}>Brugere</button>
        <button style={buttonClass("equipment")} onClick={() => setSelectedTab("equipment")}>Udstyr</button>
        <button style={buttonClass("history")} onClick={() => setSelectedTab("history")}>Historik</button>
      </div>

      {/* Fanernes indhold */}
      {selectedTab === "overview" && (
        <AdminOverviewTab 
          activeReservations={activeReservations} 
          activeLoans={activeLoans} 
          lowStockItems={lowStockItems}
        />
      )}
      {selectedTab === "users" && <AdminUsersTab />}
      {selectedTab === "equipment" && <AdminEquipmentTab />}
      {selectedTab === "history" && <AdminHistoryTab />}
    </div>
  );
}
