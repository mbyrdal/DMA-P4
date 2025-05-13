import React from 'react';

export default function AdminOverviewTab({ activeReservations, activeLoans, lowStockItems }) {
  const cardStyle = {
    backgroundColor: "#1f1f1f",
    padding: "1.5rem",
    borderRadius: "12px",
    boxShadow: "0 2px 8px rgba(0,0,0,0.3)"
  };

  const cardTitle = {
    fontSize: "1.2rem",
    marginBottom: "1rem"
  };

  const cardNumber = {
    fontSize: "2rem"
  };

  return (
    <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))", gap: "1.5rem" }}>
      <div style={cardStyle}>
        <h2 style={cardTitle}>Aktive reservationer</h2>
        <p style={cardNumber}>{activeReservations}</p>
      </div>
      <div style={cardStyle}>
        <h2 style={cardTitle}>Lav lagerbeholdning</h2>
        <ul style={{ listStyleType: "disc", paddingLeft: "1.5rem", marginTop: "1rem" }}>
          {lowStockItems.length > 0 ? (
            lowStockItems.map((item, index) => (
              <li key={index}>{item.navn} – {item.antal} stk.</li>
            ))
          ) : (
            <li>Ingen varer med lav beholdning</li>
          )}
        </ul>
      </div>
    </div>
  );
}
