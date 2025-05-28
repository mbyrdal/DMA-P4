import React, { useEffect, useState } from 'react';

export default function AdminHistoryTab() {
  const [reservations, setReservations] = useState([]);
  const [filter, setFilter] = useState("all");

  const userToken = localStorage.getItem('jwtToken');
  const backendUrl = "https://localhost:7092";

  useEffect(() => {
    if (!userToken) {
      console.warn("Ingen JWT token tilgængelig - Fetch annulleret");
      return;
    }

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
        const sorted = [...data].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        setReservations(sorted);
      })
      .catch(err => console.error("Fejl ved hentning af reservationer:", err));
  }, [userToken, backendUrl]);

  const activeReservations = reservations.filter(r => r.status === "Aktiv");
  const completedReservations = reservations.filter(r => r.status !== "Aktiv");

  const renderReservation = (res) => (
    <div key={res.id} style={{
      backgroundColor: "#1e1e1e",
      padding: "1rem",
      marginBottom: "1rem",
      borderRadius: "8px",
      boxShadow: "0 0 5px rgba(255, 255, 255, 0.05)"
    }}>
      <h4 style={{ marginBottom: "0.5rem", color: "#4da6ff" }}>
        Reservation #{res.id} – {res.email || "Ukendt bruger"} – {new Date(res.createdAt).toLocaleString()}
      </h4>
      <ul style={{ paddingLeft: "1.2rem", color: "#ccc", marginTop: "0.5rem" }}>
        {res.items.map((item, idx) => (
          <li key={idx}>{item.equipment} – {item.quantity} stk.</li>
        ))}
      </ul>
    </div>
  );

  const filteredList = {
    all: [...activeReservations, ...completedReservations],
    active: activeReservations,
    completed: completedReservations
  };

  return (
    <div style={{ padding: "2rem", fontFamily: "Arial, sans-serif", color: "white" }}>
      <h2 style={{ fontSize: "1.8rem", fontWeight: "bold", marginBottom: "1.5rem" }}>Reservationshistorik</h2>

      <div style={{ marginBottom: "1.5rem" }}>
        <label htmlFor="filter" style={{ fontWeight: "bold", marginRight: "0.5rem" }}>Filtrer visning:</label>
        <select
          id="filter"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          style={{
            padding: "0.4rem",
            borderRadius: "5px",
            border: "1px solid #ccc",
            color: "#000"
          }}
        >
          <option value="all">Vis alt</option>
          <option value="active">Aktive reservationer</option>
          <option value="completed">Afsluttede reservationer</option>
        </select>
      </div>

      {filteredList[filter].length === 0 ? (
        <p style={{ color: "#aaa" }}>Ingen reservationer fundet.</p>
      ) : (
        filteredList[filter].map(renderReservation)
      )}
    </div>
  );
}