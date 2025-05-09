import React, { useState, useEffect } from 'react';

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
    <li key={res.id} className="border-b py-2">
      <strong>Reservation #{res.id}</strong> – {res.userName || "Ukendt bruger"} – {new Date(res.createdAt).toLocaleDateString()}
      <ul className="ml-4 list-disc">
        {res.items.map((item, idx) => (
          <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
        ))}
      </ul>
    </li>
  );

  const renderLoan = (loan) => (
    <li key={loan.id} className="border-b py-2">
      <strong>Lån #{loan.id}</strong> – {loan.userName || "Ukendt bruger"} – {new Date(loan.createdAt).toLocaleDateString()}
      <ul className="ml-4 list-disc">
        {loan.items.map((item, idx) => (
          <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
        ))}
      </ul>
    </li>
  );

  return (
    <div>
      <h2 className="text-xl font-semibold mb-4">Lånehistorik</h2>

      <div className="mb-6">
        <label htmlFor="filter" className="mr-2 font-medium">Filtrer visning:</label>
        <select
          id="filter"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          className="p-2 rounded text-black"
        >
          <option value="all">Vis alt</option>
          <option value="reservations">Kun reservationer</option>
          <option value="activeLoans">Kun aktive lån</option>
          <option value="completedLoans">Kun afsluttede lån</option>
        </select>
      </div>

      <ul className="space-y-4">
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
  