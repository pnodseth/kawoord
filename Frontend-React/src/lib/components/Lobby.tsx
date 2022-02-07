import React from "react";
import { GameserviceState, Player } from "../../interface";
import Button from "$lib/components/Button";
import { GameService } from "$lib/services/GameService";

interface LobbyProps {
  gameState: GameserviceState;
  player: Player;
  gameService: GameService | undefined;
}

export default function Lobby({ gameState, player, gameService }: LobbyProps) {
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
      {player.id === gameState.game?.hostPlayer.id && <Button onClick={() => gameService?.start()}>Start Game</Button>}
    </>
  );
}
