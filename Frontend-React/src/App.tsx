import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";
import { GameServiceProvider } from "$lib/components/GameServiceContext";
import { Home } from "$lib/views/Home";
import { usePlayer } from "$lib/hooks/usePlayer";

function App() {
  const player = usePlayer();

  return (
    <div className="App font-sans">
      <main className="p-4">
        <GameServiceProvider>
          <Routes>
            <Route path="/" element={<Home player={player} />} />
            <Route path="/play" element={<GameView player={player} />} />
          </Routes>
        </GameServiceProvider>
      </main>
    </div>
  );
}

export default App;
