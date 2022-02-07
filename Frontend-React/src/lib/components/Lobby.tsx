import React from "react";
import { GameserviceState, Player } from "../../interface";
import Button from "$lib/components/Button";

interface LobbyProps {
  gameState: GameserviceState;
  player: Player;
}

export default function Lobby({ gameState, player }: LobbyProps) {
  return (
    <>
      <h1>Lobby</h1>
      <p>Join with id {gameState.game?.gameId}</p>
      <p>Game state: {gameState.game?.state}</p>
      <p>Players:</p>
      <ul>
        {gameState.game?.players.map((p) => {
          return <li key={p.id}>{p.name}</li>;
        })}
      </ul>
      {player.id === gameState.game?.hostPlayer.id && <Button>Start Game</Button>}
    </>
  );
}
