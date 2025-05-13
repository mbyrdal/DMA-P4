import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useUser } from './UserContext';

function LoanHistory() {
  const { user } = useUser();
  const [reservations, setReservations] = useState([]);
  const [timeLeft, setTimeLeft] = useState({});

  const pendingPickups = reservations.filter(r => r.status === "Aktiv" && !r.isCollected);
  const completedPickups = reservations.filter(r => r.status === "Aktiv" && r.isCollected);
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
    if (pendingPickups.length === 0) return;

    const intervals = pendingPickups.map((reservation, index) => {
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
  }, [pendingPickups]);

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

  const handleReturnReservation = (reservationId) => {
  fetch(`https://localhost:7092/api/reservation/${reservationId}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ status: "Inaktiv" })
  })
    .then(res => {
      if (!res.ok) throw new Error("Kunne ikke opdatere status til Inaktiv");
      
      return fetch(`https://localhost:7092/api/reservation/createHistory/${reservationId}`, {
        method: "POST"
      });
    })
    .then(res => {
      if (!res.ok) throw new Error("Kunne ikke gemme historik");
      
      setReservations(prev =>
        prev.map(r =>
          r.id === reservationId ? { ...r, status: "Inaktiv" } : r
        )
      );
      alert("Reservation afleveret og historik oprettet.");
    })
    .catch((err) => {
      console.error("Fejl ved aflevering:", err);
      alert("Aflevering fejlede.");
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

      {/* Afventer afhentning */}
      <section style={{ marginTop: "2rem" }}>
        <h3>Afventer afhentning</h3>
        {pendingPickups.length > 0 ? (
          pendingPickups.map((reservation) => (
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
            </div>
          ))
        ) : (
          <p>Ingen reservationer der afventer afhentning.</p>
        )}
      </section>

      {/* Allerede afhentet */}
      <section style={{ marginTop: "2rem" }}>
        <h3>Afhentede reservationer</h3>
        {completedPickups.length > 0 ? (
          completedPickups.map(res => (
            <div key={res.id} style={{
              backgroundColor: "#1e1e1e",
              padding: "1rem",
              marginBottom: "1rem",
              borderRadius: "8px",
              boxShadow: "0 0 5px rgba(255, 255, 255, 0.05)"
            }}>
              <p><strong>Oprettet:</strong> {new Date(res.createdAt).toLocaleString()}</p>
              {res.items.map((item, idx) => (
                <div key={idx}>
                  <strong>{item.equipment}</strong> – {item.quantity} stk.
                </div>
              ))}
              <p style={{ color: "#5CB85C", marginTop: "0.5rem" }}>
                Reservationen er afhentet.
              </p>
                 {res.status === "Aktiv" && (
                  <button
                  onClick={() => handleReturnReservation(res.id)}
                    style={{
                    marginTop: "0.5rem",
                    marginLeft: "0.5rem",
                    padding: "0.5rem 1rem",
                    backgroundColor: "#0275d8",
                    color: "white",
                    border: "none",
                    borderRadius: "5px",
                    cursor: "pointer"
                }}
              >
                Aflever
              </button>
        )}
            </div>
          ))
        ) : (
          <p>Ingen afhentede reservationer endnu.</p>
        )}
      </section>

      {/* Afsluttede/annullerede reservationer */}
      <section style={{ marginTop: "2rem" }}>
        <h3>Afsluttede reservationer</h3>
        {previousReservations.length > 0 ? (
          previousReservations.map((res) => (
            <div key={res.id} style={{
              backgroundColor: "#1e1e1e",
              padding: "1rem",
              marginBottom: "1rem",
              borderRadius: "8px",
              boxShadow: "0 0 5px rgba(255, 255, 255, 0.05)"
            }}>
              <p><strong>Oprettet:</strong> {new Date(res.createdAt).toLocaleString()}</p>
              {res.items.map((item, idx) => (
                <div key={idx}>
                  <strong>{item.equipment}</strong> – {item.quantity} stk.
                </div>
              ))}
              <p style={{ marginTop: "0.5rem", color: "#ccc" }}>
                Status: <span style={{ fontWeight: "bold", color: "#aaa" }}>{res.status}</span>
              </p>
            </div>
          ))
        ) : (
          <p>Ingen afsluttede reservationer endnu.</p>
        )}
      </section>
    </div>
  );
}

export default LoanHistory;
