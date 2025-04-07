import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

function LoanHistory() {
  const [reservation, setReservation] = useState(null);
  const [timeLeft, setTimeLeft] = useState("");
  const [loans, setLoans] = useState([]);

  // Hent reservation fra localStorage
  useEffect(() => {
    const saved = localStorage.getItem("myReservation");
    if (!saved) return;

    try {
      const parsed = JSON.parse(saved);
      setReservation(parsed);
    } catch (err) {
      console.error("Fejl ved parsing af reservation:", err);
    }
  }, []);

  // Countdown til reservation udløber efter 24 timer
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

  // Dummy-data til aktive og afsluttede lån – skal erstattes med backend- eller localStorage-data
  useEffect(() => {
    const dummyLoans = [
      {
        id: 1,
        name: "Mus",
        borrowedAt: "2024-04-04T10:00:00",
        returnedAt: null,
      },
      {
        id: 2,
        name: "Skærm",
        borrowedAt: "2024-03-20T14:20:00",
        returnedAt: "2024-03-22T11:00:00",
      },
    ];
    setLoans(dummyLoans);
  }, []);

  return (
    <div style={{ padding: "2rem", fontFamily: "Arial", color: "white" }}>
      {/* Tilbage-knap */}
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: "1rem" }}>
        <Link to="/">
          <button style={{
            backgroundColor: "#6c757d",
            color: "white",
            border: "none",
            padding: "0.5rem 1rem",
            borderRadius: "5px",
            cursor: "pointer"
          }}>
            Tilbage til oversigt
          </button>
        </Link>
      </div>

      <h2 style={{ fontSize: "2rem", marginBottom: "1.5rem", textAlign: "center" }}>Lånehistorik</h2>

      {/* Reservation */}
      <section style={{ backgroundColor: "#222", padding: "1rem", borderRadius: "10px", marginBottom: "2rem" }}>
        <h3 style={{ fontSize: "1.4rem", marginBottom: "1rem" }}>Nuværende reservation</h3>
        {reservation && reservation.items?.length > 0 ? (
          <div>
            {reservation.items.map((item) => (
              <div key={item.id} style={{ padding: "0.5rem 0" }}>
                <strong>{item.name}</strong> – {item.reserved} stk.
              </div>
            ))}
            <p style={{ marginTop: "0.8rem", fontStyle: "italic" }}>Udløber om: {timeLeft}</p>
          </div>
        ) : (
          <p>Ingen aktiv reservation.</p>
        )}
      </section>

      {/* Aktive lån */}
      <section style={{ marginBottom: "2rem" }}>
        <h3 style={{ fontSize: "1.4rem", marginBottom: "1rem" }}>Aktive lån</h3>
        {loans.filter(l => !l.returnedAt).length > 0 ? (
          <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
            {loans.filter(l => !l.returnedAt).map(loan => (
              <div key={loan.id} style={{ backgroundColor: "#2a2a2a", padding: "1rem", borderRadius: "8px" }}>
                <strong>{loan.name}</strong><br />
                Lånt den {new Date(loan.borrowedAt).toLocaleString()}
              </div>
            ))}
          </div>
        ) : (
          <p>Ingen aktive lån.</p>
        )}
      </section>

      {/* Afsluttede lån */}
      <section>
        <h3 style={{ fontSize: "1.4rem", marginBottom: "1rem" }}>Afsluttede lån</h3>
        {loans.filter(l => l.returnedAt).length > 0 ? (
          <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
            {loans.filter(l => l.returnedAt).map(loan => (
              <div key={loan.id} style={{ backgroundColor: "#2a2a2a", padding: "1rem", borderRadius: "8px" }}>
                <strong>{loan.name}</strong><br />
                Lånt den {new Date(loan.borrowedAt).toLocaleString()}<br />
                Afleveret den {new Date(loan.returnedAt).toLocaleString()}
              </div>
            ))}
          </div>
        ) : (
          <p>Ingen afsluttede lån endnu.</p>
        )}
      </section>
    </div>
  );
}

export default LoanHistory;