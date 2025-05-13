import React, { useState, useEffect } from 'react';
import './Tab.css';

export default function AdminHistoryTab() {
  const [reservations, setReservations] = useState([]);
  const [loans, setLoans] = useState([]);
  const [filter, setFilter] = useState("all");

  useEffect(() => {
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => setReservations(data))
      .catch(err => console.error("Fejl ved hentning af reservationer:", err));

    fetch("https://localhost:7092/api/loan")
      .then(res => res.json())
      .then(data => setLoans(data))
      .catch(err => console.error("Fejl ved hentning af lån:", err));
  }, []);

  const filteredReservations = reservations.filter(r => !r.isConvertedToLoan);
  const activeLoans = loans.filter(l => !l.returned);
  const completedLoans = loans.filter(l => l.returned);

  const renderReservation = (res) => (
    <li key={res.id} className="history-entry">
      <strong>📌 Reservation #{res.id}</strong> – {res.userName || "Ukendt bruger"} – {new Date(res.createdAt).toLocaleDateString()}
      <ul className="history-sublist">
        {res.items.map((item, idx) => (
          <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
        ))}
      </ul>
    </li>
  );

  const renderLoan = (loan) => (
    <li key={loan.id} className="history-entry">
      <strong>📦 Lån #{loan.id}</strong> – {loan.userName || "Ukendt bruger"} – {new Date(loan.createdAt).toLocaleDateString()}
      <ul className="history-sublist">
        {loan.items.map((item, idx) => (
          <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
        ))}
      </ul>
    </li>
  );

  return (
    <div className="tab">
      <h1>📜 Udlånshistorik</h1>

      <div style={{ marginBottom: "1rem" }}>
        <label htmlFor="filter" style={{ marginRight: "0.5rem", fontWeight: "bold" }}>Filtrer visning:</label>
        <select
          id="filter"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          style={{ padding: "0.5rem", borderRadius: "6px", minWidth: "180px" }}
        >
          <option value="all">Vis alt</option>
          <option value="reservations">Kun reservationer</option>
          <option value="activeLoans">Kun aktive lån</option>
          <option value="completedLoans">Kun afsluttede lån</option>
        </select>
      </div>

      <ul style={{ listStyle: "none", paddingLeft: 0 }}>
        {filter === "all" && (
          <>
            {filteredReservations.map(renderReservation)}
            {activeLoans.map(renderLoan)}
            {completedLoans.map(renderLoan)}
          </>
        )}
        {filter === "reservations" && filteredReservations.map(renderReservation)}
        {filter === "activeLoans" && activeLoans.map(renderLoan)}
        {filter === "completedLoans" && completedLoans.map(renderLoan)}
      </ul>
    </div>
  );
}
