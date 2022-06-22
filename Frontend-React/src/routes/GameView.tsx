import React, { FC, useState } from "react";
import { useGameState } from "$lib/hooks/useGameState";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";
import { Player } from "../interface";
import { Solved } from "./Solved";
import { EndedUnsolved } from "$lib/components/EndedUnsolved";
import { GameViewEnum } from "$lib/components/constants";

const GameView: FC = () => {
  const { gameState } = useGameState();
  const [player, setPlayer] = useState<Player>();

  function displayView() {
    if (!gameState.game) return;

    if (player) {
      if (gameState.game.gameViewEnum === GameViewEnum.Lobby) {
        return <Lobby gameState={gameState} player={player} />;
      } else if (gameState.game.gameViewEnum === GameViewEnum.Started) {
        return <GameBoard player={player} gameState={gameState} />;
      } else if (gameState.game.gameViewEnum === GameViewEnum.Solved) {
        return <Solved gameState={gameState} player={player} />;
      } else if (gameState.game?.gameViewEnum === GameViewEnum.EndedUnsolved) {
        return <EndedUnsolved solution={gameState.solution} />;
      } else {
        return <h2>Unknown game state: {gameState.game?.gameViewEnum}</h2>;
      }
    }
  }

  return (
    <section className="max-w-2xl m-auto">
      <div className="spacer lg:h-6" />
      <h1 className="text-6xl text-center font-kawoord">Kawoord</h1>
      <div className="spacer h-6" />

      {!gameState.game ? <NoGame game={gameState.game} setPlayer={setPlayer} player={player} /> : <>{displayView()}</>}
    </section>
  );
};
export default GameView;
