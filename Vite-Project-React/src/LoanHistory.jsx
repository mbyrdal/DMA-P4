import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

function LoanHistory() {
  const [reservation, setReservation] = useState(null);
  const [timeLeft, setTimeLeft] = useState("");
  const [loans, setLoans] = useState([]);

  // Hent seneste reservation fra API (kun nyeste)
  useEffect(() => {
    fetch("https://localhost:7092/api/reservation")
      .then((res) => res.json())
      .then((data) => {
        if (data.length > 0) {
          const latest = data[data.length - 1];
          setReservation(latest);
        }
      })
      .catch((err) => console.error("Fejl ved hentning af reservation:", err));
  }, []);

  // Countdown til reservation udløber
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

  // Her skal aktive og afsluttede lån hentes fra backend senere
  // Denne logik skal opdateres når brugere implementeres ✅
  useEffect(() => {
    // Midlertidig tomt array – fjernet dummy data
    setLoans([]);
  }, []);

  const handleDeleteReservation = () => {
    if (!reservation) return;

    fetch(`https://localhost:7092/api/reservation/${reservation.id}`, {
      method: "DELETE"
    })
      .then((res) => {
        if (!res.ok) throw new Error("Kunne ikke slette reservation");
        setReservation(null);
        setTimeLeft("");
        alert("Reservation slettet.");
      })
      .catch((err) => {
        console.error("Fejl ved sletning:", err);
        alert("Kunne ikke slette reservation.");
      });
  };

  return (
    <div style={{ padding: "2rem", fontFamily: "Arial", color: "white" }}>
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
            Tilbage til oversigten
          </button>
        </Link>
      </div>

      <h2>Lånehistorik</h2>

      <section style={{ marginTop: "2rem" }}>
        <h3>Aktiv reservation</h3>
        {reservation && reservation.items?.length > 0 ? (
          <>
            <ul>
              {reservation.items.map((item, idx) => (
                <li key={idx}>{item.equipmentName} – {item.quantity} stk.</li>
              ))}
            </ul>
            <p style={{ marginTop: "0.5rem" }}>Udløber om: {timeLeft}</p>
            <button
              onClick={handleDeleteReservation}
              style={{
                marginTop: "1rem",
                padding: "0.5rem 1rem",
                backgroundColor: "#D9534F",
                color: "white",
                border: "none",
                borderRadius: "5px",
                cursor: "pointer"
              }}
            >
              Ryd reservation
            </button>
          </>
        ) : (
          <p>Ingen aktiv reservation.</p>
        )}
      </section>

      <hr style={{ margin: "2rem 0" }} />

      <section>
        <h3>Aktive lån</h3>
        {loans.filter(l => !l.returnedAt).length > 0 ? (
          <ul>
            {loans.filter(l => !l.returnedAt).map(loan => (
              <li key={loan.id}>
                {loan.name} – Lånt den {new Date(loan.borrowedAt).toLocaleString()}
              </li>
            ))}
          </ul>
        ) : (
          <p>Ingen aktive lån.</p>
        )}
      </section>

      <section style={{ marginTop: "2rem" }}>
        <h3>Afsluttede lån</h3>
        {loans.filter(l => l.returnedAt).length > 0 ? (
          <ul>
            {loans.filter(l => l.returnedAt).map(loan => (
              <li key={loan.id}>
                Afleverede {loan.name} den {new Date(loan.returnedAt).toLocaleString()}
              </li>
            ))}
          </ul>
        ) : (
          <p>Ingen afsluttede lån endnu.</p>
        )}
      </section>
    </div>
  );
}

export default LoanHistory;
