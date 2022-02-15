import React, { FC, useContext, useEffect, useState } from "react";
import { PlayerSection } from "$lib/components/PlayerSection";
import { useGameServiceState } from "$lib/hooks/useGameServiceState";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";
import { gameServiceContext } from "$lib/components/GameServiceContext";
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

  function displayView() {
    if (player) {
      if (gameState.game?.state === "Lobby") {
        return <Lobby gameState={gameState} player={player} />;
      } else if (gameState.game?.state === "Started") {
        return <GameBoard player={player} gameState={gameState} />;
      } else if (gameState.game?.state === "Solved") {
        return <h2>Solved!</h2>;
      } else if (gameState.game?.state === "EndedUnsolved") {
        return <h3>Ended Unsolved</h3>;
      } else {
        return <h2>Unknown gamestate: {gameState.game?.state}</h2>;
      }
    }
  }

  return (
    <>
      <h1 className="text-xl text-center font-bold">Kawoord</h1>
      <PlayerSection player={player} setPlayer={setPlayer} />
      {!gameState.game ? (
        <NoGame
          onClick={() => gameService.createGame(player)}
          onJoin={(gameId) => gameService.joinGame(player, gameId)}
        />
      ) : (
        <>{displayView()}</>
      )}
    </>
  );
};
export default GameView;
