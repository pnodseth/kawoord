import React, { useContext, useEffect, useState } from "react";
import { GameState, Player } from "../../interface";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/contexts/GameServiceContext";
import { SyncLoader } from "react-spinners";
import AppLayout from "$lib/layout/AppLayout";
import { motion } from "framer-motion";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

interface LobbyProps {
  gameState: GameState;
  player: Player;
}

export default function Lobby({ gameState, player }: LobbyProps) {
  const gameService = useContext(gameServiceContext);
  const [lobbyAudio] = useState(new Audio());
  const [loading, setLoading] = useState(false);

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
    await gameService.start(gameState.game.gameId, player);
    setLoading(false);
  }

  const playerCount = gameState.game?.players.length || 0;

  const variants = {
    open: { scale: 4, opacity: 0.7 },
    initial: { opacity: 0, scale: 0 },
    normal: { opacity: 1, scale: 1 },
  };

  const currentPlayerIndex = gameState.game?.players.findIndex((e) => e.id == player.id) as number;

  return (
    <AppLayout headerSize="small">
      <div className="font-sans">
        <h2 className="text-2xl text-gray-600">Share the game code:</h2>
        <p className="font-bold text-2xl mt-1">{gameState.game?.gameId}</p>
        <div className="spacer h-12" />
        <p className="text-lg mb-2 font-sans">
          Players joined ({playerCount}/{gameState.game?.maxPlayers}):
        </p>
        <ul className="min-h-[120px]">
          {gameState.game?.players.map((p, i) => {
            return (
              <motion.div
                key={p.id}
                className="font-bold mb-2 md:text-2xl"
                initial={i < currentPlayerIndex ? "normal" : "initial"}
                animate={i < currentPlayerIndex ? "normal" : ["open", "normal"]}
                variants={variants}
                transition={{ duration: 0.8, type: "spring" }}
              >
                {p.name} {p.id === player.id && <span className="font-light">(you)</span>}
              </motion.div>
            );
          })}
        </ul>
        <div className="spacer h-8" />
        <div>
          <FixedBottomContent>
            {playerCount === gameState.game?.maxPlayers ? (
              <h1 className="animate-bounce text-2xl">Ready to start!</h1>
            ) : (
              <p className="animate-bounce text-lg">...Waiting for more players to join...</p>
            )}
            <div className="spacer h-2" />
            {player.id === gameState.game?.hostPlayer.id && (
              <Button onClick={() => startGame()}>{loading ? <SyncLoader color="#FFF" /> : "Start game"}</Button>
            )}
          </FixedBottomContent>
        </div>
      </div>
    </AppLayout>
  );
}
