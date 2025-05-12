import React from "react";
import "./Overview.css";

function AdminOverviewTab() {
  return (
    <div className="overview">
      <h1>📊 Depot Overblik</h1>
      <div className="cards">
        <div className="card">✅ 24 tilgængelige enheder</div>
        <div className="card">📦 10 udlånte enheder</div>
        <div className="card">🔁 2 under reparation</div>
      </div>
    </div>
  );
}

export default AdminOverviewTab;
