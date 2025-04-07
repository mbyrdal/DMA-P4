import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function EquipmentOverview() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("alle");
  const [equipmentList, setEquipmentList] = useState([]);
  const [myReservation, setMyReservation] = useState([]);
  const [isModified, setIsModified] = useState(false);

  // Stilvalg til tabelvisning
  const thStyle = {
    borderBottom: "1px solid white",
    padding: "0.5rem",
    textAlign: "left"
  };

  const tdStyle = {
    padding: "0.5rem",
    borderBottom: "1px solid #555"
  };

  // Hent udstyrsliste fra backend ved komponent-mount
  useEffect(() => {
    fetch("https://localhost:7092/api/backend")
      .then(response => {
        if (!response.ok) {
          throw new Error("Fejl ved hentning af udstyr");
        }
        return response.json();
      })
      .then(data => {
        setEquipmentList(data);
      })
      .catch(error => {
        console.error("Hentning fejlede:", error);
      });
  }, []);

  // Hent reservation fra localStorage ved komponent-mount (hvis den stadig er gyldig)
  useEffect(() => {
    const saved = localStorage.getItem("myReservation");
    if (!saved) return;

    try {
      const parsed = JSON.parse(saved);

      if (!parsed.createdAt || !parsed.items) {
        console.warn("Gemte data har ikke forventet struktur.");
        return;
      }

      const createdAt = new Date(parsed.createdAt);
      const now = new Date();
      const diffHours = (now - createdAt) / (1000 * 60 * 60);

      if (diffHours >= 24) {
        console.log("Reservation er mere end 24 timer gammel. Den fjernes.");
        localStorage.removeItem("myReservation");
      } else {
        setMyReservation(parsed.items);
      }
    } catch (error) {
      console.error("Fejl ved parsing af reservation fra localStorage:", error);
    }
  }, []);

  // Hjælpefunktion til at gemme reservation i localStorage med timestamp
  const updateLocalStorageReservation = (updatedReservation) => {
    const reservationWithTimestamp = {
      createdAt: new Date().toISOString(),
      items: updatedReservation
    };
    localStorage.setItem("myReservation", JSON.stringify(reservationWithTimestamp));
  };

  // Håndter klik på udstyr for at reservere én enhed
  const handleReserve = (id) => {
    const updatedList = equipmentList.map(item => {
      if (item.id === id && item.antal > 0) {
        return { ...item, antal: item.antal - 1 };
      }
      return item;
    });

    const selectedItem = equipmentList.find(item => item.id === id);
    const existing = myReservation.find(item => item.id === id);

    const updatedReservation = existing
      ? myReservation.map(item =>
          item.id === id ? { ...item, reserved: item.reserved + 1 } : item
        )
      : [...myReservation, { id: selectedItem.id, name: selectedItem.navn, reserved: 1 }];

    setEquipmentList(updatedList);
    setMyReservation(updatedReservation);
    setIsModified(true);
    updateLocalStorageReservation(updatedReservation);
  };

  // Håndter fjernelse af én reserveret enhed
  const handleRemoveItem = (id) => {
    const itemToRemove = myReservation.find(item => item.id === id);
    if (!itemToRemove) return;

    const updatedReservation = itemToRemove.reserved > 1
      ? myReservation.map(item =>
          item.id === id ? { ...item, reserved: item.reserved - 1 } : item
        )
      : myReservation.filter(item => item.id !== id);

    const updatedEquipment = equipmentList.map(item =>
      item.id === id ? { ...item, antal: item.antal + 1 } : item
    );

    setMyReservation(updatedReservation);
    setEquipmentList(updatedEquipment);
    setIsModified(true);
    updateLocalStorageReservation(updatedReservation);
  };

  // Håndter "ryd alt"-funktion for reservationer
  const handleClearReservation = () => {
    const updatedEquipment = equipmentList.map(equip => {
      const match = myReservation.find(res => res.id === equip.id);
      if (match) {
        return { ...equip, antal: equip.antal + match.reserved };
      }
      return equip;
    });

    localStorage.removeItem("myReservation");

    setEquipmentList(updatedEquipment);
    setMyReservation([]);
    setIsModified(false);
  };

  // Bekræft reservation og gem i localStorage
  const handleConfirm = () => {
    if (myReservation.length === 0) {
      alert("Du har ikke reserveret noget.");
      return;
    }

    updateLocalStorageReservation(myReservation);

    alert("Reservation bekræftet og gemt.");
    console.log("Reservation gemt i localStorage:", myReservation);

    //setMyReservation([]); (Dette tager vi ud, så man ikke skal refresh siden for at se ændringerne)
    setIsModified(false);
  };

  // Filtrer udstyr baseret på søgning og valgt filter
  const filteredItems = equipmentList.filter(item => {
    const matchesSearch =
      item.navn && typeof item.navn === "string"
        ? item.navn.toLowerCase().includes(search.toLowerCase())
        : false;
    const matchesFilter = filter === "alle" || item.status === filter;
    return matchesSearch && matchesFilter;
  });

  return (
    <div style={{ padding: "1rem", fontFamily: "Arial", color: "white", textAlign: "center" }}>
      {/* Knappen øverst til højre til lånehistorik */}
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: "1rem" }}>
        <Link to="/history">
          <button style={{
            backgroundColor: "#007BFF",
            color: "white",
            border: "none",
            padding: "0.5rem 1rem",
            borderRadius: "5px",
            cursor: "pointer"
          }}>
            Se lånehistorik
          </button>
        </Link>
      </div>

      <h2>Lageroversigt</h2>

      {/* Søg og filter */}
      <input
        type="text"
        placeholder="Søg udstyr..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        style={{ padding: "0.5rem" }}
      />

      <select
        value={filter}
        onChange={(e) => setFilter(e.target.value)}
        style={{ marginLeft: "1rem", padding: "0.5rem" }}
      >
        <option value="alle">Alle</option>
        <option value="tilgængelig">Tilgængelig</option>
        <option value="udlånt">Udlånt</option>
        <option value="reserveret">Reserveret</option>
      </select>

      {/* Lageroversigt tabel */}
      <table style={{ width: "80%", margin: "1rem auto", borderCollapse: "collapse", color: "white" }}>
        <thead>
          <tr>
            <th style={thStyle}>ID</th>
            <th style={thStyle}>Navn</th>
            <th style={thStyle}>Antal</th>
            <th style={thStyle}>Lokation</th>
          </tr>
        </thead>
        <tbody>
          {filteredItems.map(item => (
            <tr
              key={item.id}
              onClick={() => item.antal > 0 && handleReserve(item.id)}
              style={{
                cursor: item.antal > 0 ? "pointer" : "not-allowed",
                backgroundColor: item.antal === 0 ? "#444" : "transparent",
                transition: "background-color 0.2s"
              }}
              onMouseEnter={(e) => {
                if (item.antal > 0) e.currentTarget.style.backgroundColor = "#333";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = item.antal === 0 ? "#444" : "transparent";
              }}
            >
              <td style={tdStyle}>{item.id}</td>
              <td style={tdStyle}>{item.navn}</td>
              <td style={tdStyle}>{item.antal}</td>
              <td style={tdStyle}>{item.lokation}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <hr style={{ margin: "2rem 0" }} />

      {/* Reservationssektion */}
      <h3>Din reservation</h3>
      {myReservation.length === 0 ? (
        <p>Ingen varer reserveret endnu.</p>
      ) : (
        <>
          <ul style={{ listStyle: "none", paddingLeft: 0, marginTop: "1rem" }}>
            {myReservation.map(item => (
              <li
                key={item.id}
                style={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  marginBottom: "0.8rem"
                }}
              >
                <div style={{ width: "200px", textAlign: "right", paddingRight: "1rem" }}>
                  <strong>{item.name}</strong> – {item.reserved} stk.
                </div>

                <button
                  onClick={() => handleRemoveItem(item.id)}
                  style={{
                    padding: "0.3rem 0.8rem",
                    backgroundColor: "#D9534F",
                    color: "white",
                    border: "none",
                    borderRadius: "4px",
                    cursor: "pointer",
                    fontSize: "0.9rem"
                  }}
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
                padding: "0.6rem 1.2rem",
                backgroundColor: !isModified ? "#999" : "#007BFF",
                color: "white",
                border: "none",
                borderRadius: "5px",
                marginRight: "1rem",
                cursor: !isModified ? "not-allowed" : "pointer"
              }}
            >
              Bekræft reservation
            </button>

            <button
              onClick={handleClearReservation}
              style={{
                padding: "0.6rem 1.2rem",
                backgroundColor: "#6c757d",
                color: "white",
                border: "none",
                borderRadius: "5px",
                cursor: "pointer"
              }}
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
