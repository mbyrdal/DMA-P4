// filepath: c:\Users\palle\source\repos\DMA-P4\Vite-Project-React\src\pages\QRCodePage.jsx
import React from "react";
import QRScanner from "../components/QRScanner";

const QRCodePage = () => {
  const handleScanSuccess = async (data) => {
    console.log("Scanned QR Code:", data);

    // Send the scanned data to the backend
    try {
      const response = await fetch("http://localhost:5000/api/qrcode/decode", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ qrCodeData: data }),
      });

      const result = await response.json();
      console.log("Decoded Data from Backend:", result);
    } catch (error) {
      console.error("Error sending QR code to backend:", error);
    }
  };

  const handleScanError = (error) => {
    console.error("QR Scan Error:", error);
  };

  return (
    <div>
      <h1>QR Code Scanner</h1>
      <QRScanner onScanSuccess={handleScanSuccess} onScanError={handleScanError} />
    </div>
  );
};

export default QRCodePage;