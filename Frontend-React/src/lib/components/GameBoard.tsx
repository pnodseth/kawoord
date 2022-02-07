import React from "react";
import type { Game, GameserviceState, Player } from "../../interface";

interface GameBoardProps {
  game: Game;
  player: Player;
  gameState: GameserviceState;
}

const GameBoard = (props: GameBoardProps) => {
  return <h1>Game: {props.game.gameId}</h1>;
};

export default GameBoard;
