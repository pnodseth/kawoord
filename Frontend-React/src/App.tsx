import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";
import { GameServiceProvider } from "$lib/components/GameServiceContext";
import { Home } from "$lib/views/Home";
import { usePlayer } from "$lib/hooks/usePlayer";
import { PlayerEvents } from "$lib/components/PlayerEvents";
import ConnectionEvents from "$lib/components/ConnectionEvents";
import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import { msalConfig } from "./auth/authConfig";

const msalInstance = new PublicClientApplication(msalConfig);

function App() {
  const player = usePlayer();

  return (
    <MsalProvider instance={msalInstance}>
      <div className="App font-sans" id="App">
        <main className="p-4">
          <GameServiceProvider>
            <ConnectionEvents />
            <Routes>
              <Route path="/" element={<Home player={player} />} />
              <Route path="/play" element={<GameView player={player} />} />
            </Routes>
          </GameServiceProvider>
          <PlayerEvents />
        </main>
      </div>
    </MsalProvider>
  );
}

export default App;
