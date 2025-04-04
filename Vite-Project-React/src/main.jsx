import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import App from './App.jsx';
import { BrowserRouter } from 'react-router-dom';  // Make sure this is imported

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>  {/* Wrap App with BrowserRouter */}
      <App />
    </BrowserRouter>
  </StrictMode>,
);