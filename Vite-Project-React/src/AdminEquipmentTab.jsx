import React from "react";
import "./Tab.css";

function AdminEquipmentTab() {
  return (
    <div className="tab">
      <h1>🔧 Udstyr</h1>
      <table className="styled-table">
        <thead>
          <tr>
            <th>Navn</th>
            <th>Tilstand</th>
            <th>Lokation</th>
          </tr>
        </thead>
        <tbody>
          <tr><td>Boremaskine</td><td>Tilgængelig</td><td>Depot A</td></tr>
          <tr><td>iPad</td><td>Udlånt</td><td>IT-afdeling</td></tr>
        </tbody>
      </table>
    </div>
  );
}

export default AdminEquipmentTab;
