import React from "react";
import "./Tab.css";

function AdminUsersTab() {
  return (
    <div className="tab">
      <h1>👥 Brugere</h1>
      <table className="styled-table">
        <thead>
          <tr>
            <th>Navn</th>
            <th>Rolle</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr><td>Anna Jensen</td><td>Admin</td><td>Aktiv</td></tr>
          <tr><td>Peter Sørensen</td><td>Bruger</td><td>Inaktiv</td></tr>
        </tbody>
      </table>
    </div>
  );
}

export default AdminUsersTab;
