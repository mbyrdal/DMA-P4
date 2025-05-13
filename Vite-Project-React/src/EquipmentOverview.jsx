import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function EquipmentOverview() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("alle");
  const [equipmentList, setEquipmentList] = useState([]);
  const [myReservation, setMyReservation] = useState([]);
  const [isModified, setIsModified] = useState(false);

  useEffect(() => {
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => {
        const latest = data[data.length - 1];
        if (latest?.items) setMyReservation(latest.items);
      })
      .catch(err => console.error("Fejl ved reservation:", err));

    fetch("https://localhost:7092/api/backend")
      .then(res => res.json())
      .then(setEquipmentList)
      .catch(err => console.error("Fejl ved udstyr:", err));
  }, []);

  const handleReserve = (id) => {
    const item = equipmentList.find(i => i.id === id);
    if (!item || item.antal <= 0) return;

    const updatedList = equipmentList.map(i =>
      i.id === id ? { ...i, antal: i.antal - 1 } : i
    );

    const updatedReservation = [...myReservation];
    const existing = updatedReservation.find(r => r.equipmentName === item.navn);
    if (existing) {
      existing.quantity += 1;
    } else {
      updatedReservation.push({ equipmentName: item.navn, quantity: 1 });
    }

    setEquipmentList(updatedList);
    setMyReservation(updatedReservation);
    setIsModified(true);
  };

  const handleRemoveItem = (name) => {
    const updatedReservation = myReservation.map(r =>
      r.equipmentName === name ? { ...r, quantity: r.quantity - 1 } : r
    ).filter(r => r.quantity > 0);

    const updatedEquipment = equipmentList.map(i =>
      i.navn === name ? { ...i, antal: i.antal + 1 } : i
    );

    setMyReservation(updatedReservation);
    setEquipmentList(updatedEquipment);
    setIsModified(true);
  };

  const handleConfirm = () => {
    if (myReservation.length === 0) return alert("Du har ikke reserveret noget.");

    fetch("https://localhost:7092/api/reservation", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userName: "MidlertidigBruger", items: myReservation })
    })
      .then(res => {
        if (!res.ok) throw new Error();
        return res.json();
      })
      .then(() => {
        alert("Reservation oprettet!");
        setIsModified(false);
      })
      .catch(() => alert("Fejl under oprettelse af reservation"));
  };

  const handleClearReservation = () => {
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => {
        const latest = data[data.length - 1];
        return fetch(`https://localhost:7092/api/reservation/${latest.id}`, { method: "DELETE" });
      })
      .then(() => {
        setMyReservation([]);
        setIsModified(false);
        alert("Reservation slettet.");
      })
      .catch(() => alert("Kunne ikke slette reservation."));
  };

  const filtered = equipmentList.filter(i =>
    i.navn?.toLowerCase().includes(search.toLowerCase()) &&
    (filter === "alle" || i.status === filter)
  );

  return (
    <div className="tab">
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: "1rem" }}>
        <Link to="/history">
          <button className="btn-primary">Se lånehistorik</button>
        </Link>
      </div>

      <h1>📦 Lageroversigt</h1>

      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Søg udstyr..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: "0.5rem", width: "200px" }}
        />
        <select value={filter} onChange={e => setFilter(e.target.value)} style={{ marginLeft: "1rem" }}>
          <option value="alle">Alle</option>
          <option value="tilgængelig">Tilgængelig</option>
          <option value="udlånt">Udlånt</option>
          <option value="reserveret">Reserveret</option>
        </select>
      </div>

      <table className="styled-table">
        <thead>
          <tr>
            <th>Navn</th>
            <th>Antal</th>
            <th>Lokation</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map(item => (
            <tr
              key={item.id}
              onClick={() => handleReserve(item.id)}
              style={{ cursor: item.antal > 0 ? "pointer" : "not-allowed" }}
            >
              <td>{item.navn}</td>
              <td>{item.antal}</td>
              <td>{item.lokation}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <hr />

      <h2>📝 Din reservation</h2>
      {myReservation.length === 0 ? (
        <p>Ingen varer reserveret endnu.</p>
      ) : (
        <>
          <ul style={{ listStyle: "none", paddingLeft: 0 }}>
            {myReservation.map(item => (
              <li key={item.equipmentName} style={{ marginBottom: "0.5rem" }}>
                {item.equipmentName} – {item.quantity} stk.
                <button
                  onClick={() => handleRemoveItem(item.equipmentName)}
                  style={{ marginLeft: "1rem", backgroundColor: "#D9534F", color: "white" }}
                >
                  Fjern én
                </button>
              </li>
            ))}
          </ul>

          <div style={{ marginTop: "1rem" }}>
            <button
              onClick={handleConfirm}
              disabled={!isModified}
              style={{
                marginRight: "1rem",
                backgroundColor: isModified ? "#28a745" : "#999",
                color: "white",
                cursor: isModified ? "pointer" : "not-allowed"
              }}
            >
              Bekræft reservation
            </button>

            <button
              onClick={handleClearReservation}
              style={{ backgroundColor: "#6c757d", color: "white" }}
            >
              Ryd reservation
            </button>
          </div>
        </>
      )}
    </div>
  );
}

export default EquipmentOverview;
