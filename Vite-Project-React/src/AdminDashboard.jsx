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

  // Retrieve JWT token from localStorage (adjust if your app stores it elsewhere)
  const userToken = localStorage.getItem('jwtToken');
  const backendUrl = "https://localhost:7092";

  useEffect(() => {
    if (!userToken) {
      console.warn("Ingen JWT token tilgængelig - Fetch annulleret");
      return;
    }

    // Fetch reservations with auth header
    fetch(`${backendUrl}/api/reservation`, {
      headers: {
        "Authorization": `Bearer ${userToken}`,
        "Content-Type": "application/json"
      }
    })
      .then(res => {
        if (res.status === 401) {
          throw new Error("Uautoriseret adgang ved reservation-fetch");
        }
        if (!res.ok) throw new Error("Fejl ved hentning af reservationer");
        return res.json();
      })
      .then(data => {
        const aktive = data.filter(res => res.status === "Aktiv").length;
        const inaktive = data.filter(res => res.status === "Inaktiv").length;
        setActiveReservations(aktive);
        setActiveLoans(inaktive);
      })
      .catch(err => console.error("Fejl i reservation-fetch:", err));

    // Fetch equipment with auth header
    fetch(`${backendUrl}/api/backend`, {
      headers: {
        "Authorization": `Bearer ${userToken}`,
        "Content-Type": "application/json"
      }
    })
      .then(res => {
        if (res.status === 401) {
          throw new Error("Uautoriseret adgang ved udstyrs-fetch");
        }
        if (!res.ok) throw new Error("Fejl ved hentning af udstyr");
        return res.json();
      })
      .then(data => {
        const lowStock = data.filter(item => item.antal < 6);
        setLowStockItems(lowStock);
      })
      .catch(err => console.error("Fejl i udstyrs-fetch:", err));
  }, [userToken, backendUrl]);

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