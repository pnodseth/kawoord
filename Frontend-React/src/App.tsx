import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";
import { GameServiceProvider } from "$lib/components/GameServiceContext";
import { Home } from "$lib/views/Home";
import { usePlayer } from "$lib/hooks/usePlayer";
import { PlayerEvents } from "$lib/components/PlayerEvents";
import ConnectionEvents from "$lib/components/ConnectionEvents";

function App() {
  const player = usePlayer();

  return (
    <div className="App font-sans" id="App">
      <main>
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
  );
}

export default App;
