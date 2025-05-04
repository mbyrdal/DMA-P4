import React, { useEffect, useState } from 'react';

export default function AdminUsersTab() {
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({ name: "", role: "Bruger" });
  const [editMode, setEditMode] = useState(false);
  const [editUser, setEditUser] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    fetch("https://localhost:7092/api/user")
      .then(res => res.json())
      .then(data => {
        const sorted = [...data].sort((a, b) => a.role === "Admin" ? -1 : 1);
        setUsers(sorted);
      })
      .catch(err => console.error("Fejl ved hentning af brugere:", err));
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

  const handleEditClick = (user) => {
    setEditUser({ ...user });
    setEditMode(true);
  };

  const handleSaveEdit = () => {
    fetch(`https://localhost:7092/api/user/${editUser.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(editUser)
    })
      .then(() => {
        setEditMode(false);
        setEditUser(null);
        refreshUsers();
      });
  };

  const roleStyle = (role) => ({
    color: role === "Admin" ? "#4da6ff" : "#cccccc",
    fontStyle: "italic"
  });

  const filteredUsers = users.filter(u =>
    u.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div>
      <h2 style={{ fontSize: "1.5rem", fontWeight: "bold", marginBottom: "1rem" }}>Brugere</h2>

      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Søg efter bruger..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{ padding: "0.5rem", width: "300px", marginBottom: "1rem" }}
        />
      </div>

      <div style={{ marginBottom: "1.5rem" }}>
        <input
          type="text"
          placeholder="Navn"
          value={newUser.name}
          onChange={(e) => setNewUser({ ...newUser, name: e.target.value })}
          style={{ padding: "0.5rem", marginRight: "0.5rem" }}
        />
        <select
          value={newUser.role}
          onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
          style={{ padding: "0.5rem", marginRight: "0.5rem" }}
        >
          <option value="Bruger">Bruger</option>
          <option value="Admin">Admin</option>
        </select>
        <button onClick={handleAddUser}>Tilføj</button>
      </div>

      <ul style={{ listStyle: "none", paddingLeft: 0 }}>
        {filteredUsers.map(user => (
          <li key={user.id} style={{
            borderBottom: "1px solid #444",
            padding: "0.5rem 0",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center"
          }}>
            <div>
              {editMode && editUser?.id === user.id ? (
                <>
                  <input
                    type="text"
                    value={editUser.name}
                    onChange={(e) => setEditUser({ ...editUser, name: e.target.value })}
                    style={{ marginRight: "0.5rem", padding: "0.3rem" }}
                  />
                  <select
                    value={editUser.role}
                    onChange={(e) => setEditUser({ ...editUser, role: e.target.value })}
                    style={{ padding: "0.3rem", marginRight: "0.5rem" }}
                  >
                    <option value="Bruger">Bruger</option>
                    <option value="Admin">Admin</option>
                  </select>
                </>
              ) : (
                <>
                  {user.name} – <span style={roleStyle(user.role)}>{user.role}</span>
                </>
              )}
            </div>
            <div>
              {editMode && editUser?.id === user.id ? (
                <button onClick={handleSaveEdit} style={{ marginRight: "0.5rem" }}>Gem</button>
              ) : (
                <button onClick={() => handleEditClick(user)} style={{ marginRight: "0.5rem" }}>Rediger</button>
              )}
              <button
                onClick={() => handleDelete(user.id)}
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
