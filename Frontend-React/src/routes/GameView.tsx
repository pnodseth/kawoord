import React, { FC, useContext, useState } from "react";
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
        return <h2>Unknown game state: {gameState.game?.state}</h2>;
      }
    }
  }

  return (
    <section className="max-w-2xl m-auto">
      <div className="spacer h-6" />
      <h1 className="text-6xl text-center font-kawoord">Kawoord</h1>
      <div className="spacer h-6" />

      {!gameState.game ? (
        <NoGame
          onClick={() => gameService.createGame(player)}
          onJoin={(gameId) => player && gameService.joinGame(player, gameId)}
          setPlayer={setPlayer}
          player={player}
        />
      ) : (
        <>{displayView()}</>
      )}
    </section>
  );
};
export default GameView;
