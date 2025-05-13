import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

function LoanHistory() {
  const [reservation, setReservation] = useState(null);
  const [timeLeft, setTimeLeft] = useState("");
  const [loans, setLoans] = useState([]);

  // Hent seneste reservation
  useEffect(() => {
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => {
        const latest = data[data.length - 1];
        if (latest?.items) setReservation(latest);
      });
  }, []);

  // Nedtælling
  useEffect(() => {
    if (!reservation?.createdAt) return;

    const interval = setInterval(() => {
      const expiration = new Date(reservation.createdAt);
      expiration.setHours(expiration.getHours() + 24);
      const now = new Date();
      const diff = expiration - now;

      if (diff <= 0) {
        setTimeLeft("Reservation er udløbet");
        clearInterval(interval);
        return;
      }

      const hours = Math.floor(diff / (1000 * 60 * 60));
      const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((diff % (1000 * 60)) / 1000);

      setTimeLeft(`${hours}t ${minutes}m ${seconds}s`);
    }, 1000);

    return () => clearInterval(interval);
  }, [reservation]);

  const handleDeleteReservation = () => {
    if (!reservation) return;

    fetch(`https://localhost:7092/api/reservation/${reservation.id}`, {
      method: "DELETE"
    })
      .then(res => {
        if (!res.ok) throw new Error();
        setReservation(null);
        setTimeLeft("");
        alert("Reservation slettet.");
      })
      .catch(() => alert("Fejl ved sletning."));
  };

  return (
    <div className="tab">
      <div style={{ display: "flex", justifyContent: "space-between", marginBottom: "1rem" }}>
        <h1>📖 Din reservation</h1>
        <Link to="/">
          <button style={{ backgroundColor: "#6c757d", color: "white", borderRadius: "5px", padding: "0.5rem 1rem" }}>
            Tilbage
          </button>
        </Link>
      </div>

      <section style={{ marginBottom: "2rem" }}>
        <h2>📌 Aktiv reservation</h2>
        {reservation && reservation.items?.length > 0 ? (
          <>
            <ul style={{ listStyle: "disc", paddingLeft: "1.5rem" }}>
              {reservation.items.map((item, idx) => (
                <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
              ))}
            </ul>
            <p style={{ marginTop: "0.5rem" }}>⏳ Udløber om: {timeLeft}</p>
            <button
              onClick={handleDeleteReservation}
              style={{ marginTop: "1rem", backgroundColor: "#D9534F", color: "white", padding: "0.5rem 1rem", borderRadius: "5px", border: "none" }}
            >
              Ryd reservation
            </button>
          </>
        ) : (
          <p>Ingen aktiv reservation.</p>
        )}
      </section>

      {/* Fremtidig funktionalitet til visning af lån */}
      <section>
        <h2>📦 Lån</h2>
        <p>(Denne funktion er endnu ikke implementeret.)</p>
      </section>
    </div>
  );
}

export default LoanHistory;
