import React, { useContext } from "react";
import { GameserviceState, Player } from "../../interface";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";

interface LobbyProps {
  gameState: GameserviceState;
  player: Player;
}

export default function Lobby({ gameState, player }: LobbyProps) {
  const gameService = useContext(gameServiceContext);

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
