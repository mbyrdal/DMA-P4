import React, { useEffect, useState } from 'react';
import './Tab.css';

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
    <div className="tab">
      <h1>🔧 Udstyr</h1>

      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Søg udstyr..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{ padding: "0.5rem", width: "300px", marginBottom: "1rem" }}
        />
      </div>

      <div style={{ marginBottom: "1rem", display: "flex", gap: "0.5rem", flexWrap: "wrap" }}>
        <input type="text" placeholder="Navn" value={newItem.navn} onChange={(e) => setNewItem({ ...newItem, navn: e.target.value })} />
        <input type="number" placeholder="Antal" value={newItem.antal} onChange={(e) => setNewItem({ ...newItem, antal: parseInt(e.target.value) })} />
        <input type="text" placeholder="Lokation" value={newItem.lokation} onChange={(e) => setNewItem({ ...newItem, lokation: e.target.value })} />
        <button onClick={handleAdd}>Tilføj</button>
      </div>

      <table className="styled-table">
        <thead>
          <tr>
            <th>Navn</th>
            <th>Antal</th>
            <th>Lokation</th>
            <th>Handling</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map(item => (
            <tr key={item.id}>
              <td>
                {editItem?.id === item.id
                  ? <input value={editItem.navn} onChange={(e) => setEditItem({ ...editItem, navn: e.target.value })} />
                  : item.navn}
                {item.antal < 6 && <span style={{ color: "#ff6666", marginLeft: "0.5rem" }}>(Lav beholdning)</span>}
              </td>
              <td>
                {editItem?.id === item.id
                  ? <input type="number" value={editItem.antal} onChange={(e) => setEditItem({ ...editItem, antal: parseInt(e.target.value) })} />
                  : item.antal}
              </td>
              <td>
                {editItem?.id === item.id
                  ? <input value={editItem.lokation} onChange={(e) => setEditItem({ ...editItem, lokation: e.target.value })} />
                  : item.lokation}
              </td>
              <td>
                {editItem?.id === item.id ? (
                  <button onClick={handleEdit}>Gem</button>
                ) : (
                  <button onClick={() => setEditItem(item)}>Rediger</button>
                )}
                <button onClick={() => handleDelete(item.id)} style={{ marginLeft: "0.5rem", backgroundColor: "#D9534F", color: "white" }}>
                  Slet
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
