// filepath: c:\Users\palle\source\repos\DMA-P4\Vite-Project-React\src\components\QRScanner.jsx
import React, { useEffect, useRef } from "react";
import { Html5Qrcode } from "html5-qrcode";

const QRScanner = ({ onScanSuccess, onScanError }) => {
  const qrCodeRegionId = "qr-code-region";
  const html5QrCodeRef = useRef(null);

  useEffect(() => {
    html5QrCodeRef.current = new Html5Qrcode(qrCodeRegionId);

    const startScanner = async () => {
      try {
        await html5QrCodeRef.current.start(
          { facingMode: "environment" }, // Use the environment-facing camera
          {
            fps: 10, // Frames per second
            qrbox: { width: 250, height: 250 }, // Scanning box size
          },
          (decodedText) => {
            onScanSuccess(decodedText); // Pass the scanned QR code to the parent
          },
          (errorMessage) => {
            if (onScanError) onScanError(errorMessage);
          }
        );
      } catch (error) {
        console.error("Error starting QR scanner:", error);
      }
    };

    startScanner();

    return () => {
      html5QrCodeRef.current.stop().catch((err) => console.error("Error stopping QR scanner:", err));
    };
  }, [onScanSuccess, onScanError]);

  return <div id={qrCodeRegionId} style={{ width: "100%", height: "300px" }} />;
};

export default QRScanner;