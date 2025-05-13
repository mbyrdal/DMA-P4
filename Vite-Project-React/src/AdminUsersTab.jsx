import React, { useEffect, useState } from 'react';
import './Tab.css';

export default function AdminUsersTab() {
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({ name: "", role: "Bruger" });
  const [editUser, setEditUser] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    refreshUsers();
  }, []);

  const refreshUsers = () => {
    fetch("https://localhost:7092/api/user")
      .then(res => res.json())
      .then(data => {
        const sorted = [...data].sort((a, b) => a.role === "Admin" ? -1 : 1);
        setUsers(sorted);
      });
  };

  const handleAddUser = () => {
    if (!newUser.name) return;
    fetch("https://localhost:7092/api/user", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newUser)
    })
      .then(res => res.json())
      .then(() => {
        setNewUser({ name: "", role: "Bruger" });
        refreshUsers();
      });
  };

  const handleDelete = (id) => {
    if (!window.confirm("Er du sikker på at du vil slette brugeren?")) return;
    fetch(`https://localhost:7092/api/user/${id}`, { method: "DELETE" })
      .then(() => setUsers(prev => prev.filter(u => u.id !== id)));
  };

  const handleSaveEdit = () => {
    fetch(`https://localhost:7092/api/user/${editUser.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(editUser)
    })
      .then(() => {
        setEditUser(null);
        refreshUsers();
      });
  };

  const filteredUsers = users.filter(u =>
    u.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="tab">
      <h1>👥 Brugere</h1>

      <input
        type="text"
        placeholder="Søg bruger..."
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        style={{ marginBottom: "1rem", padding: "0.5rem", width: "300px" }}
      />

      <div style={{ marginBottom: "1rem", display: "flex", gap: "0.5rem", flexWrap: "wrap" }}>
        <input
          type="text"
          placeholder="Navn"
          value={newUser.name}
          onChange={(e) => setNewUser({ ...newUser, name: e.target.value })}
        />
        <select
          value={newUser.role}
          onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
        >
          <option value="Bruger">Bruger</option>
          <option value="Admin">Admin</option>
        </select>
        <button onClick={handleAddUser}>Tilføj</button>
      </div>

      <table className="styled-table">
        <thead>
          <tr>
            <th>Navn</th>
            <th>Rolle</th>
            <th>Handling</th>
          </tr>
        </thead>
        <tbody>
          {filteredUsers.map(user => (
            <tr key={user.id}>
              <td>
                {editUser?.id === user.id
                  ? <input value={editUser.name} onChange={(e) => setEditUser({ ...editUser, name: e.target.value })} />
                  : user.name}
              </td>
              <td>
                {editUser?.id === user.id
                  ? (
                    <select
                      value={editUser.role}
                      onChange={(e) => setEditUser({ ...editUser, role: e.target.value })}
                    >
                      <option value="Bruger">Bruger</option>
                      <option value="Admin">Admin</option>
                    </select>
                  )
                  : <span style={{ fontStyle: "italic", color: user.role === "Admin" ? "#4da6ff" : "#999" }}>{user.role}</span>}
              </td>
              <td>
                {editUser?.id === user.id ? (
                  <button onClick={handleSaveEdit}>Gem</button>
                ) : (
                  <button onClick={() => setEditUser(user)}>Rediger</button>
                )}
                <button
                  onClick={() => handleDelete(user.id)}
                  style={{ marginLeft: "0.5rem", backgroundColor: "#D9534F", color: "white" }}
                >
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
