import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";
import { GameServiceProvider } from "$lib/components/GameServiceContext";

function App() {
  return (
    <div className="App font-sans">
      <main className="p-4 max-w-lg m-auto mt-4 h-96">
        <GameServiceProvider>
          <Routes>
            <Route path="/" element={<GameView />} />
          </Routes>
        </GameServiceProvider>
      </main>
    </div>
  );
}

export default App;
