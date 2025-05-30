import React, { useEffect, useState } from 'react';

export default function AdminUsersTab() {
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({ name: "", role: "Bruger" });
  const [editMode, setEditMode] = useState(false);
  const [editUser, setEditUser] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");

  const userToken = localStorage.getItem('jwtToken');
  const backendUrl = "https://localhost:7092";

  const headersWithAuth = {
    "Authorization": `Bearer ${userToken}`,
    "Content-Type": "application/json"
  };

  useEffect(() => {
    if (!userToken) {
      console.warn("Ingen JWT token tilgængelig - Fetch annulleret");
      return;
    }

    fetch(`${backendUrl}/api/user`, { headers: headersWithAuth })
      .then(res => {
        if (res.status === 401) throw new Error("Uautoriseret adgang ved hentning af brugere");
        return res.json();
      })
      .then(data => {
        const sorted = [...data].sort((a, b) => a.role === "Admin" ? -1 : 1);
        setUsers(sorted);
      })
      .catch(err => console.error("Fejl ved hentning af brugere:", err));
  }, [userToken, backendUrl]);

  const refreshUsers = () => {
    fetch(`${backendUrl}/api/user`, { headers: headersWithAuth })
      .then(res => res.json())
      .then(data => {
        const sorted = [...data].sort((a, b) => a.role === "Admin" ? -1 : 1);
        setUsers(sorted);
      });
  };

  const handleAddUser = () => {
    if (!newUser.name) return;
    fetch(`${backendUrl}/api/user`, {
      method: "POST",
      headers: headersWithAuth,
      body: JSON.stringify(newUser)
    })
      .then(res => {
        if (!res.ok) throw new Error("Fejl ved oprettelse af bruger");
        return res.json();
      })
      .then(() => {
        setNewUser({ name: "", role: "Bruger" });
        refreshUsers();
      })
      .catch(err => console.error(err));
  };

  const handleDelete = (id, role, rowVersion) => {
    if (role === "Admin") return;
    if (!window.confirm("Er du sikker på at du vil slette brugeren?")) return;

    fetch(`${backendUrl}/api/user/${id}`, {
      method: "DELETE",
      headers: {
        ...headersWithAuth, // ✅ Use spread instead of nesting
        "RowVersion": rowVersion // ✅ Send RowVersion header
      }
    })
      .then(res => {
        if (!res.ok) throw new Error("Fejl ved sletning af bruger");
        setUsers(prev => prev.filter(u => u.id !== id));
      })
      .catch(err => console.error(err));
  };

  const handleEditClick = (user) => {
    if (user.role === "Admin") return;
    setEditUser({ ...user }); // ✅ Preserve rowVersion for concurrency
    setEditMode(true);
  };

  const handleSaveEdit = () => {
    fetch(`${backendUrl}/api/user/${editUser.id}`, {
      method: "PUT",
      headers: headersWithAuth,
      body: JSON.stringify(editUser) // ✅ Includes rowVersion in body
    })
      .then(res => {
        if (res.status === 409) {
          throw new Error("Conflict");
        }
        if (!res.ok) throw new Error("Fejl ved opdatering af bruger");
        setEditMode(false);
        setEditUser(null);
        refreshUsers();
      })
      .catch(err => {
        if (err.message === "Conflict") {
          alert("Brugeren er blevet ændret af en anden. Genindlæs venligst og prøv igen.");
        } else {
          console.error(err);
        }
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
              {user.role !== "Admin" && (
                editMode && editUser?.id === user.id ? (
                  <button onClick={handleSaveEdit} style={{ marginRight: "0.5rem" }}>Gem</button>
                ) : (
                  <button onClick={() => handleEditClick(user)} style={{ marginRight: "0.5rem" }}>Rediger</button>
                )
              )}
              <button
                onClick={() => handleDelete(user.id, user.role, user.rowVersion)}
                disabled={user.role === "Admin"}
                style={{
                  backgroundColor: "#D9534F",
                  color: "white",
                  border: "none",
                  padding: "0.3rem 0.6rem",
                  borderRadius: "4px",
                  opacity: user.role === "Admin" ? 0.5 : 1,
                  cursor: user.role === "Admin" ? "not-allowed" : "pointer"
                }}
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