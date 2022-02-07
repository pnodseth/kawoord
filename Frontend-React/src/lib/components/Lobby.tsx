import React from "react";
import { Game } from "../../interface";

export default function Lobby(game: Game) {
  return (
    <>
      <h1>Lobby</h1>
      <p>Game state: {game.state}</p>
    </>
  );
}
