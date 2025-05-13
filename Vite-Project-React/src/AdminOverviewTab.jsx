import React from 'react';
import './Overview.css';

export default function AdminOverviewTab({ activeReservations, activeLoans, lowStockItems }) {
  return (
    <div className="overview">
      <h1>📊 Depot Overblik</h1>
      <div className="cards">
        <div className="card">
          <h2>Aktive reservationer</h2>
          <p className="card-value">{activeReservations}</p>
        </div>

        <div className="card">
          <h2>Aktive lån</h2>
          <p className="card-value">{activeLoans}</p>
        </div>

        <div className="card">
          <h2>Lav lagerbeholdning</h2>
          <ul style={{ marginTop: "1rem", paddingLeft: "1.2rem", listStyle: "disc" }}>
            {lowStockItems.length > 0 ? (
              lowStockItems.map((item, idx) => (
                <li key={idx}>{item.navn} – {item.antal} stk.</li>
              ))
            ) : (
              <li>Ingen varer med lav beholdning</li>
            )}
          </ul>
        </div>
      </div>
    </div>
  );
}
