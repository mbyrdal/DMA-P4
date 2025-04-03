import React, { useState, useEffect } from 'react';

const initialData = [
  { id: 1, name: "Tastatur", status: "tilgængelig", quantity: 3 },
  { id: 2, name: "HDMI-kabel", status: "udlånt", quantity: 0 },
  { id: 3, name: "Dockingstation", status: "reserveret", quantity: 1 },
  { id: 4, name: "Mus", status: "tilgængelig", quantity: 5 }
];

function EquipmentOverview() {
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("alle");
  const [equipmentList, setEquipmentList] = useState(initialData);
  const [myReservation, setMyReservation] = useState([]);
  const [isModified, setIsModified] = useState(false);


  useEffect(() => {
    const storedReservation = localStorage.getItem("myReservation");
    if (storedReservation) {
      setMyReservation(JSON.parse(storedReservation));
      console.log("Tidligere reservationer hentet fra localStorage");
    }
  }, []);
  
  useEffect(() => {
    const saved = localStorage.getItem("myReservation");
    if (!saved) {
      setIsModified(myReservation.length > 0);
      return;
    }
  
    try {
      const parsed = JSON.parse(saved);
      const same = JSON.stringify(parsed) === JSON.stringify(myReservation);
      setIsModified(!same);
    } catch (err) {
      setIsModified(true);
    }
  }, [myReservation]);
  

  const handleReserve = (id) => {
    const updatedList = equipmentList.map(item => {
      if (item.id === id && item.quantity > 0) {
        return { ...item, quantity: item.quantity - 1 };
      }
      return item;
    });

    const selectedItem = equipmentList.find(item => item.id === id);
    const existing = myReservation.find(item => item.id === id);

    const updatedReservation = existing
      ? myReservation.map(item =>
          item.id === id ? { ...item, reserved: item.reserved + 1 } : item
        )
      : [...myReservation, { id: selectedItem.id, name: selectedItem.name, reserved: 1 }];

    setEquipmentList(updatedList);
    setMyReservation(updatedReservation);
  };

  const handleRemoveItem = (id) => {
    const itemToRemove = myReservation.find(item => item.id === id);
    if (!itemToRemove) return;

    const updatedReservation = itemToRemove.reserved > 1
      ? myReservation.map(item =>
          item.id === id ? { ...item, reserved: item.reserved - 1 } : item
        )
      : myReservation.filter(item => item.id !== id);

    const updatedEquipment = equipmentList.map(item =>
      item.id === id ? { ...item, quantity: item.quantity + 1 } : item
    );

    setMyReservation(updatedReservation);
    setEquipmentList(updatedEquipment);
  };

  const handleClearReservation = () => {
    // Genskab alle reserverede mængder til equipment
    const updatedEquipment = equipmentList.map(equip => {
      const match = myReservation.find(res => res.id === equip.id);
      if (match) {
        return { ...equip, quantity: equip.quantity + match.reserved };
      }
      return equip;
    });

    localStorage.removeItem("myReservation");

    setEquipmentList(updatedEquipment);
    setMyReservation([]);
  };

  const handleConfirm = () => {
    if (myReservation.length === 0) {
      alert("Du har ikke reserveret noget.");
      return;
    }
  
    // Gem reservationen i localStorage
    localStorage.setItem("myReservation", JSON.stringify(myReservation));
  
    alert("Reservation bekræftet og gemt.");
    console.log("Reservation gemt i localStorage:", myReservation);
  
    setMyReservation([]);
  };
  

  const filteredItems = equipmentList.filter(item => {
    const matchesSearch = item.name.toLowerCase().includes(search.toLowerCase());
    const matchesFilter = filter === "alle" || item.status === filter;
    return matchesSearch && matchesFilter;
  });

  return (
    <div style={{ padding: "1rem", fontFamily: "Arial", color: "white", textAlign: "center" }}>
      <h2>Lageroversigt</h2>

      <input
        type="text"
        placeholder="Søg udstyr..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        style={{ padding: "0.5rem" }}
      />

      <select value={filter} onChange={(e) => setFilter(e.target.value)} style={{ marginLeft: "1rem", padding: "0.5rem" }}>
        <option value="alle">Alle</option>
        <option value="tilgængelig">Tilgængelig</option>
        <option value="udlånt">Udlånt</option>
        <option value="reserveret">Reserveret</option>
      </select>

      <ul style={{ marginTop: "1rem", listStyle: "none", paddingLeft: 0 }}>
        {filteredItems.map(item => (
          <li key={item.id} style={{ marginBottom: "1rem" }}>
            <strong>{item.name}</strong> ({item.status}) – Tilgængelige: {item.quantity}
            <br />
            <button
              onClick={() => handleReserve(item.id)}
              disabled={item.quantity === 0}
              style={{
                marginTop: "0.5rem",
                padding: "0.4rem 0.8rem",
                cursor: item.quantity === 0 ? "not-allowed" : "pointer",
                backgroundColor: item.quantity === 0 ? "#777" : "#4CAF50",
                color: "white",
                border: "none",
                borderRadius: "4px"
              }}
            >
              Reservér
            </button>
          </li>
        ))}
      </ul>

      <hr style={{ margin: "2rem 0" }} />

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


            <button
                onClick={handleConfirm}
                disabled={!isModified}
                style={{
                marginTop: "1rem",
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

        </>
      )}
    </div>
  );
}

export default EquipmentOverview;
