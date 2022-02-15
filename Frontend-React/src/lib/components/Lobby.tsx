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
    <section className="text-center bg-white text-black rounded p-8  font-sans h-[70vh] flex flex-col justify-between">
      <div>
        <h2 className="text-2xl text-gray-600">Share the game code:</h2>
        <p className="font-bold text-2xl mt-1">{gameState.game?.gameId}</p>
        <div className="spacer h-6" />
        <p className="text-lg mb-2">Players joined ({gameState.game?.players.length}/5):</p>
        <ul>
          {gameState.game?.players.map((p) => {
            return (
              <li key={p.id} className="font-bold">
                {p.name}
              </li>
            );
          })}
        </ul>
        <div className="spacer h-12" />
      </div>
      <div>
        <p className="animate-bounce text-lg">...Waiting for more players to join...</p>
        <div className="spacer h-12" />
        {player.id === gameState.game?.hostPlayer.id && (
          <Button onClick={() => gameService?.start()}>Start Game</Button>
        )}
      </div>
    </section>
  );
}
