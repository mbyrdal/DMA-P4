import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import axios from 'axios';
import PWAPrompt from 'react-ios-pwa-prompt'
import React, { useState, useEffect } from 'react';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})
const isIOS = () => {
    return /iphone|ipad|ipod/.test(window.navigator.userAgent.toLowerCase());
};

const isAndroid = () => {
    return /android/.test(window.navigator.userAgent.toLowerCase());
};

const isSmallScreen = window.innerWidth < 600;

<h1 style={{ fontSize: isSmallScreen ? '18px' : '24px' }}>Hello World</h1>


function App() {
        const [showPrompt, setShowPrompt] = useState(false);
        const [platform, setPlatform] = useState('unknown');
        const [deferredPrompt, setDeferredPrompt] = useState(null);
        const [showAndroidPrompt, setShowAndroidPrompt] = useState(false);

        // Detect platform
        useEffect(() => {
            const userAgent = window.navigator.userAgent.toLowerCase();
            const isIOS = /iphone|ipad|ipod/.test(userAgent);
            const isAndroid = /android/.test(userAgent);
            const isStandalone = window.matchMedia('(display-mode: standalone)').matches || window.navigator.standalone;

            if (isIOS && !isStandalone) {
                setPlatform('ios');
            } else if (isAndroid && !isStandalone) {
                setPlatform('android');
            } else {
                setPlatform('desktop');
            }
        }, []);

        // Listen for Android install prompt
        useEffect(() => {
            const handler = (e) => {
                e.preventDefault();
                setDeferredPrompt(e);
                setShowAndroidPrompt(true);
            };

            window.addEventListener('beforeinstallprompt', handler);
            return () => window.removeEventListener('beforeinstallprompt', handler);
        }, []);

        const handleInstallClick = async () => {
            if (deferredPrompt) {
                deferredPrompt.prompt();
                const { outcome } = await deferredPrompt.userChoice;
                console.log('User install choice:', outcome);
                setDeferredPrompt(null);
                setShowAndroidPrompt(false);
            }
        };

    useEffect(() => {
        const isIOS = /iphone|ipad|ipod/.test(window.navigator.userAgent.toLowerCase());
        const isInStandaloneMode = 'standalone' in window.navigator && window.navigator.standalone;

        if (isIOS && !isInStandaloneMode) {
            setShowPrompt(true);
        }
    }, []);

    return (
        <>
            <h1>Hello World</h1>
            {showPrompt && <ReactIOSPWAPrompt />}
        </>
    );
}