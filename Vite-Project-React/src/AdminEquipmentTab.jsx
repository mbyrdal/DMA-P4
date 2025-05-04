import React, { useEffect, useState } from 'react';

export default function AdminEquipmentTab() {
  const [equipment, setEquipment] = useState([]);
  const [newItem, setNewItem] = useState({ navn: '', antal: 0, lokation: '' });
  const [editItem, setEditItem] = useState(null);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    fetch("https://localhost:7092/api/backend")
      .then(res => res.json())
      .then(data => setEquipment(data));
  }, []);

  const refreshEquipment = () => {
    fetch("https://localhost:7092/api/backend")
      .then(res => res.json())
      .then(data => setEquipment(data));
  };

  const handleAdd = () => {
    if (!newItem.navn || newItem.antal < 0 || !newItem.lokation) return;

    fetch("https://localhost:7092/api/backend", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newItem)
    })
      .then(res => res.json())
      .then(() => {
        setNewItem({ navn: '', antal: 0, lokation: '' });
        refreshEquipment();
      });
  };

  const handleDelete = (id) => {
    if (!window.confirm("Er du sikker på at du vil slette udstyret?")) return;
    fetch(`https://localhost:7092/api/backend/${id}`, { method: "DELETE" })
      .then(() => setEquipment(prev => prev.filter(e => e.id !== id)));
  };

  const handleEdit = () => {
    fetch(`https://localhost:7092/api/backend/${editItem.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(editItem)
    })
      .then(() => {
        setEditItem(null);
        refreshEquipment();
      });
  };

  const filtered = equipment.filter(e =>
    e.navn.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div>
      <h2 style={{ fontSize: "1.5rem", fontWeight: "bold", marginBottom: "1rem" }}>Udstyr</h2>

      <input
        type="text"
        placeholder="Søg udstyr..."
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        style={{ padding: "0.5rem", width: "300px", marginBottom: "1rem" }}
      />

      <div style={{ marginBottom: "1rem" }}>
        <input type="text" placeholder="Navn" value={newItem.navn} onChange={(e) => setNewItem({ ...newItem, navn: e.target.value })} style={{ padding: "0.5rem", marginRight: "0.5rem" }} />
        <input type="number" placeholder="Antal" value={newItem.antal} onChange={(e) => setNewItem({ ...newItem, antal: parseInt(e.target.value) })} style={{ padding: "0.5rem", width: "80px", marginRight: "0.5rem" }} />
        <input type="text" placeholder="Lokation" value={newItem.lokation} onChange={(e) => setNewItem({ ...newItem, lokation: e.target.value })} style={{ padding: "0.5rem", marginRight: "0.5rem" }} />
        <button onClick={handleAdd}>Tilføj</button>
      </div>

      <ul style={{ listStyle: "none", paddingLeft: 0 }}>
        {filtered.map(item => (
          <li key={item.id} style={{ borderBottom: "1px solid #444", padding: "0.5rem 0", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <div>
              {editItem?.id === item.id ? (
                <>
                  <input value={editItem.navn} onChange={(e) => setEditItem({ ...editItem, navn: e.target.value })} style={{ marginRight: "0.5rem" }} />
                  <input type="number" value={editItem.antal} onChange={(e) => setEditItem({ ...editItem, antal: parseInt(e.target.value) })} style={{ width: "60px", marginRight: "0.5rem" }} />
                  <input value={editItem.lokation} onChange={(e) => setEditItem({ ...editItem, lokation: e.target.value })} />
                </>
              ) : (
                <>
                  {item.navn} – {item.antal} stk. – {item.lokation}
                  {item.antal < 6 && <span style={{ color: "#ff6666", marginLeft: "0.5rem" }}>(Lav beholdning)</span>}
                </>
              )}
            </div>
            <div>
              {editItem?.id === item.id ? (
                <button onClick={handleEdit} style={{ marginRight: "0.5rem" }}>Gem</button>
              ) : (
                <button onClick={() => setEditItem(item)} style={{ marginRight: "0.5rem" }}>Rediger</button>
              )}
              <button
                onClick={() => handleDelete(item.id)}
                style={{ backgroundColor: "#D9534F", color: "white", border: "none", padding: "0.3rem 0.6rem", borderRadius: "4px" }}
              >
                Slet
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
