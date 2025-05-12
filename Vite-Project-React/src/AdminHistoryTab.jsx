import React from "react";
import "./Tab.css";

function AdminHistoryTab() {
  return (
    <div className="tab">
      <h1>📜 Udlånshistorik</h1>
      <table className="styled-table">
        <thead>
          <tr>
            <th>Bruger</th>
            <th>Udstyr</th>
            <th>Dato</th>
          </tr>
        </thead>
        <tbody>
          <tr><td>Anna Jensen</td><td>Boremaskine</td><td>2025-05-01</td></tr>
          <tr><td>Peter Sørensen</td><td>iPad</td><td>2025-04-28</td></tr>
        </tbody>
      </table>
    </div>
  );
}

export default AdminHistoryTab;
