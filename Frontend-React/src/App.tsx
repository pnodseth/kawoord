import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";
import { GameServiceProvider } from "$lib/contexts/GameServiceContext";
import { Home } from "$lib/views/Home";
import { PlayerEvents } from "$lib/components/PlayerEvents";
import ConnectionEvents from "$lib/components/ConnectionEvents";
import { PlayerProvider } from "$lib/contexts/PlayerContext";
import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import { msalConfig } from "./auth/authConfig";

const msalInstance = new PublicClientApplication(msalConfig);

function App() {

  return (
    <MsalProvider instance={msalInstance}>
    <div className="App font-sans" id="App">
      <main>
        <GameServiceProvider>
          <PlayerProvider>
            <ConnectionEvents />
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/play" element={<GameView />} />
            </Routes>
          </PlayerProvider>
        </GameServiceProvider>
        <PlayerEvents />
      </main>
    </div>
    </MsalProvider>
  );
}

export default App;
