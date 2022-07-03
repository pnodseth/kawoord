import React, { useContext, useEffect, useState } from "react";
import { GameState, Player } from "../../interface";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { SyncLoader } from "react-spinners";
import AppLayout from "$lib/layout/AppLayout";

interface LobbyProps {
  gameState: GameState;
  player: Player;
}

export default function Lobby({ gameState, player }: LobbyProps) {
  const gameService = useContext(gameServiceContext);
  const [lobbyAudio] = useState(new Audio());
  const [playerJoinAudio] = useState(new Audio("/audio/player_join.wav")); //convert to mp3
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    gameService.registerCallbacks({
      onNotification: (player, type) => {
        playerJoinAudio.play().then();
        // todo show toaster
      },
    });
  }, [gameService, playerJoinAudio]);

  /*Audio*/
  useEffect(() => {
    lobbyAudio.src = "/audio/lobby3.m4a";
    lobbyAudio.loop = true;
    lobbyAudio.play().then();

    return function () {
      lobbyAudio.pause();
    };
  }, [lobbyAudio]);

  async function startGame() {
    if (!gameState.game) return;
    setLoading(true);
    await gameService.start(gameState.game.gameId);
    setLoading(false);
  }

  const playerCount = gameState.game?.players.length || 0;

  return (
    <AppLayout>
      <div className="font-sans">
        <h2 className="text-2xl text-gray-600">Share the game code:</h2>
        <p className="font-bold text-2xl mt-1">{gameState.game?.gameId}</p>
        <div className="spacer h-6" />
        <p className="text-lg mb-2 font-sans">
          Players joined ({playerCount}/{gameState.game?.maxPlayers}):
        </p>
        <ul>
          {gameState.game?.players.map((p) => {
            return (
              <li key={p.id} className="font-bold mb-2">
                {p.name}
              </li>
            );
          })}
        </ul>
        <div className="spacer h-8" />
      </div>
      <div>
        {playerCount === gameState.game?.maxPlayers ? (
          <h1 className="animate-bounce text-2xl">Ready to start!</h1>
        ) : (
          <p className="animate-bounce text-lg">...Waiting for more players to join...</p>
        )}
        <div className="spacer h-8" />
        {player.id === gameState.game?.hostPlayer.id && (
          <Button onClick={() => startGame()}>{loading ? <SyncLoader color="#FFF" /> : "Start game"}</Button>
        )}
      </div>
    </AppLayout>
  );
}
