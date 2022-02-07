import React from "react";
import { Route, Routes } from "react-router-dom";
import GameView from "./routes/GameView";
import "./App.css";

function App() {
  return (
    <div className="App">
      <main className="border-2 border-black p-4 max-w-lg m-auto mt-4 h-96">
        <Routes>
          <Route path="/" element={<GameView />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
