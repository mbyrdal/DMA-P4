import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function EquipmentOverview() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("alle");
  const [equipmentList, setEquipmentList] = useState([]);
  const [myReservation, setMyReservation] = useState([]);
  const [isModified, setIsModified] = useState(false);

  const thStyle = {
    borderBottom: "1px solid white",
    padding: "0.5rem",
    textAlign: "left",
    width: "25%"
  };

  const tdStyle = {
    padding: "0.5rem",
    borderBottom: "1px solid #555",
    textAlign: "left",
    width: "25%"
  };

  useEffect(() => {
    fetch("https://localhost:7092/api/reservation")
      .then((response) => {
        if (!response.ok) {
          throw new Error("Kunne ikke hente reservation");
        }
        return response.json();
      })
      .then((data) => {
        if (data.length > 0) {
          const latest = data[data.length - 1];
          if (latest && latest.items && Array.isArray(latest.items)) {
            setMyReservation(latest.items);
          }
        }
      })
      .catch((error) => {
        console.error("Fejl ved hentning af reservation:", error);
      });
  }, []);

  useEffect(() => {
    fetch("https://localhost:7092/api/backend")
      .then((response) => {
        if (!response.ok) {
          throw new Error("Kunne ikke hente udstyr");
        }
        return response.json();
      })
      .then((data) => setEquipmentList(data))
      .catch((error) => console.error("Fejl ved hentning af udstyr:", error));
  }, []);

  const handleReserve = (id) => {
    const updatedList = equipmentList.map(item => {
      if (item.id === id && item.antal > 0) {
        return { ...item, antal: item.antal - 1 };
      }
      return item;
    });

    const selectedItem = equipmentList.find(item => item.id === id);
    const existing = myReservation.find(item => item.equipmentName === selectedItem.navn);

    const updatedReservation = existing
      ? myReservation.map(item =>
          item.equipmentName === selectedItem.navn
            ? { ...item, quantity: item.quantity + 1 }
            : item
        )
      : [...myReservation, { equipmentName: selectedItem.navn, quantity: 1 }];

    setEquipmentList(updatedList);
    setMyReservation(updatedReservation);
    setIsModified(true);
  };

  const handleRemoveItem = (equipmentName) => {
    const itemToRemove = myReservation.find(item => item.equipmentName === equipmentName);
    if (!itemToRemove) return;

    const updatedReservation = itemToRemove.quantity > 1
      ? myReservation.map(item =>
          item.equipmentName === equipmentName
            ? { ...item, quantity: item.quantity - 1 }
            : item
        )
      : myReservation.filter(item => item.equipmentName !== equipmentName);

    const updatedEquipment = equipmentList.map(item =>
      item.navn === equipmentName ? { ...item, antal: item.antal + 1 } : item
    );

    setMyReservation(updatedReservation);
    setEquipmentList(updatedEquipment);
    setIsModified(true);
  };

  const handleClearReservation = () => {
    fetch("https://localhost:7092/api/reservation")
      .then(res => res.json())
      .then(data => {
        if (data.length === 0) return;
        const latest = data[data.length - 1];
  
        return fetch(`https://localhost:7092/api/reservation/${latest.id}`, {
          method: "DELETE"
        });
      })
      .then(() => {
        setMyReservation([]);
        setIsModified(false);
        alert("Reservation slettet.");
      })
      .catch(err => {
        console.error("Fejl ved sletning af reservation:", err);
        alert("Kunne ikke slette reservation.");
      });
  };

  const handleConfirm = () => {
    if (myReservation.length === 0) {
      alert("Du har ikke reserveret noget.");
      return;
    }
  
    const reservationData = {
      userName: "MidlertidigBruger",
      items: myReservation
    };
  
    fetch("https://localhost:7092/api/reservation", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(reservationData)
    })
      .then(res => {
        if (!res.ok) throw new Error("Noget gik galt ved oprettelse af reservation.");
        return res.json();
      })
      .then(() => {
        alert("Reservation oprettet!");
        setIsModified(false);
  
        // Den nye reservation hentes for at opdatere visningen
        fetch("https://localhost:7092/api/reservation")
          .then(res => res.json())
          .then(data => {
            if (data.length > 0) {
              const latest = data[data.length - 1];
              if (latest?.items && Array.isArray(latest.items)) {
                setMyReservation(latest.items);
              }
            }
          });
      })
      .catch(err => {
        console.error("Fejl ved API-kald:", err);
        alert("Kunne ikke oprette reservation.");
      });
  };
   

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

      <table style={{ width: "80%", margin: "1rem auto", borderCollapse: "collapse", color: "white", tableLayout: "fixed" }}>
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

      <h3>Din reservation</h3>
      {myReservation.length === 0 ? (
        <p>Ingen varer reserveret endnu.</p>
      ) : (
        <>
          <ul style={{ listStyle: "none", paddingLeft: 0, marginTop: "1rem" }}>
            {myReservation.map(item => (
              <li
                key={item.equipmentName}
                style={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  marginBottom: "0.8rem"
                }}
              >
                <div style={{ width: "200px", textAlign: "right", paddingRight: "1rem" }}>
                  <strong>{item.equipmentName}</strong> – {item.quantity} stk.
                </div>

                <button
                  onClick={() => handleRemoveItem(item.equipmentName)}
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
