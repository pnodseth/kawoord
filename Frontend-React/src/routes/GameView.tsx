import React, { FC } from "react";
import { useGameState } from "$lib/hooks/useGameState";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";
import { Solved } from "./Solved";
import { EndedUnsolved } from "$lib/components/EndedUnsolved";
import { GameViewEnum } from "$lib/components/constants";
import { Player } from "../interface";
import { Playing } from "$lib/components/Playing";

interface IGameView {
  player: Player;
}

const GameView: FC<IGameView> = ({ player }: IGameView) => {
  const { gameState } = useGameState();

  if (!gameState.game) return <NoGame game={gameState.game} player={player} />;
  if (gameState.game.gameViewEnum === GameViewEnum.Lobby) {
    return <Lobby gameState={gameState} player={player} />;
  } else if (gameState.game.gameViewEnum === GameViewEnum.Started) {
    return <Playing gameState={gameState} player={player} />;
  } else if (gameState.game.gameViewEnum === GameViewEnum.Solved) {
    return <Solved gameState={gameState} player={player} />;
  } else if (gameState.game?.gameViewEnum === GameViewEnum.EndedUnsolved) {
    return <EndedUnsolved solution={gameState.solution} />;
  } else {
    return <h2>Unknown game state: {gameState.game?.gameViewEnum}</h2>;
  }
};
export default GameView;
