import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useUser } from './UserContext';

function LoanHistory() {
  const { user } = useUser();
  const [reservations, setReservations] = useState([]);
  const [timeLeft, setTimeLeft] = useState({});
  const [loans, setLoans] = useState([]);

  const activeReservations = reservations.filter(r => r.status === "Aktiv");
  const previousReservations = reservations.filter(r => r.status !== "Aktiv");


  useEffect(() => {
    if (!user?.email) return;

    fetch(`https://localhost:7092/api/reservation/user/${user.email}`)
      .then(res => res.json())
      .then(data => {
        const sorted = [...data].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        setReservations(sorted);
      })
      .catch(err => console.error("Fejl ved hentning af reservationer:", err));
  }, [user?.email]);

  useEffect(() => {
    if (activeReservations.length === 0) return;

    const intervals = activeReservations.map((reservation, index) => {
      return setInterval(() => {
        const expiration = new Date(reservation.createdAt);
        expiration.setHours(expiration.getHours() + 24);
        const now = new Date();
        const diff = expiration - now;

        if (diff <= 0) {
          setTimeLeft(prev => ({
            ...prev,
            [reservation.id]: "Reservation er udløbet"
          }));
          clearInterval(intervals[index]);
          return;
        }

        const hours = Math.floor(diff / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((diff % (1000 * 60)) / 1000);

        setTimeLeft(prev => ({
          ...prev,
          [reservation.id]: `${hours}t ${minutes}m ${seconds}s`
        }));
      }, 1000);
    });

    return () => intervals.forEach(clearInterval);
  }, [activeReservations]);

  const handleDeleteReservation = (reservationId) => {
    fetch(`https://localhost:7092/api/reservation/${reservationId}`, {
      method: "DELETE"
    })
      .then((res) => {
        if (!res.ok) throw new Error("Kunne ikke slette reservation");
        setReservations(prev => prev.filter(r => r.id !== reservationId));
        setTimeLeft(prev => {
          const copy = { ...prev };
          delete copy[reservationId];
          return copy;
        });
        alert("Reservation slettet.");
      })
      .catch((err) => {
        console.error("Fejl ved sletning:", err);
        alert("Kunne ikke slette reservation.");
      });
  };

  const handleMarkCollected = (reservationId) => {
    fetch(`https://localhost:7092/api/reservation/markcollected/${reservationId}`, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json"
      }
    })
      .then((res) => {
        if (!res.ok) throw new Error("Kunne ikke markere som afhentet");
        setReservations(prev =>
          prev.map(r =>
            r.id === reservationId ? { ...r, isCollected: true, status: "Aktiv" } : r
          )
        );
        alert("Reservation markeret som afhentet.");
      })
      .catch((err) => {
        console.error("Fejl ved afhentning:", err);
        alert("Kunne ikke markere som afhentet.");
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
        <h3>Aktive reservationer</h3>
        {activeReservations.length > 0 ? (
          activeReservations.map((reservation) => (
            <div key={reservation.id} style={{ marginBottom: "1rem", borderBottom: "1px solid #444", paddingBottom: "1rem" }}>
              <p><strong>Oprettet:</strong> {new Date(reservation.createdAt).toLocaleString()}</p>
              {reservation.items.map((item, idx) => (
                <div key={idx}>
                  <strong>{item.equipment}</strong> – {item.quantity} stk.
                </div>
              ))}
              <p style={{ marginTop: "0.5rem" }}>
                Udløber om: {timeLeft[reservation.id] || "Beregner..."}
              </p>

              <button
                onClick={() => handleDeleteReservation(reservation.id)}
                style={{
                  marginTop: "0.5rem",
                  padding: "0.5rem 1rem",
                  backgroundColor: "#D9534F",
                  color: "white",
                  border: "none",
                  borderRadius: "5px",
                  cursor: "pointer"
                }}
              >
                Ryd denne reservation
              </button>

              {!reservation.isCollected && (
                <button
                  onClick={() => handleMarkCollected(reservation.id)}
                  style={{
                    marginTop: "0.5rem",
                    marginLeft: "0.5rem",
                    padding: "0.5rem 1rem",
                    backgroundColor: "#5CB85C",
                    color: "white",
                    border: "none",
                    borderRadius: "5px",
                    cursor: "pointer"
                  }}
                >
                  Markér som afhentet
                </button>
              )}

              {reservation.isCollected && (
                <p style={{ color: "#5CB85C", marginTop: "0.5rem" }}>
                  Reservationen er afhentet.
                </p>
              )}
            </div>
          ))
        ) : (
          <p>Ingen aktive reservationer.</p>
        )}
      </section>

      <hr style={{ margin: "2rem 0" }} />

      <section style={{ marginTop: "2rem" }}>
        <h3>Afsluttede reservationer</h3>
        {loans.filter(l => l.returnedAt).length > 0 ? (
          <ul>
            {loans.filter(l => l.returnedAt).map(loan => (
              <li key={loan.id}>
                Afleverede {loan.name} den {new Date(loan.returnedAt).toLocaleString()}
              </li>
            ))}
          </ul>
        ) : (
          <p>Ingen afsluttede reservationer endnu.</p>
        )}
      </section>
    </div>
  );
}

export default LoanHistory;