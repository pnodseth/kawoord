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
    <section className="text-center bg-white text-black rounded p-4 font-sans">
      <h2 className="text-2xl text-gray-600">
        Share the join code: <span className="font-bold">{gameState.game?.gameId}</span>
      </h2>
      <div className="spacer h-6"></div>
      <p className="text-lg">Players:</p>
      <ul>
        {gameState.game?.players.map((p) => {
          return <li key={p.id}>{p.name}</li>;
        })}
      </ul>
      <div className="spacer h-12"></div>
      <p>...Waiting for more players to join...</p>
      <div className="spacer h-12"></div>

      {player.id === gameState.game?.hostPlayer.id && <Button onClick={() => gameService?.start()}>Start Game</Button>}
    </section>
  );
}
