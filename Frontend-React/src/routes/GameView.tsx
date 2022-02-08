import React, { FC, useContext, useEffect, useState } from "react";
import { PlayerSection } from "$lib/components/PlayerSection";
import { useGameServiceState } from "$lib/hooks/useGameServiceState";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import Button from "$lib/components/Button";
import { Player } from "../interface";

const GameView: FC = () => {
  const { gameState } = useGameServiceState();
  const gameService = useContext(gameServiceContext);
  const [player, setPlayer] = useState<Player>();

  /*get cached Player on first mount*/
  useEffect(() => {
    const cachedPlayerString = localStorage.getItem("player");
    if (cachedPlayerString) {
      setPlayer(JSON.parse(cachedPlayerString));
    }
  }, []);

  /*Store updated player in local storage*/
  useEffect(() => {
    if (player) {
      localStorage.setItem("player", JSON.stringify(player));
    }
  }, [player]);

  if (!player) {
    return (
      <>
        <h1 className="text-xl text-center font-bold">Kawoord</h1>
        <PlayerSection player={player} setPlayer={setPlayer} />
      </>
    );
  }

  return (
    <>
      <Button onClick={() => setPlayer({ name: "hei", id: "123" })}>Set Player</Button>
      <h1 className="text-xl text-center font-bold">Kawoord</h1>
      <PlayerSection player={player} setPlayer={setPlayer} />
      {!gameState.game ? (
        <NoGame
          onClick={() => gameService.createGame(player)}
          onJoin={(gameId) => gameService.joinGame(player, gameId)}
        />
      ) : (
        <>
          {player && gameState.game?.state === "Lobby" ? (
            <Lobby gameState={gameState} player={player} />
          ) : (
            <GameBoard player={player} gameState={gameState} />
          )}
        </>
      )}
    </>
  );
};
export default GameView;
