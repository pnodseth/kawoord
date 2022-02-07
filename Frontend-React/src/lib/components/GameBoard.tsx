import React from "react";
import type { Game, Player } from "../../interface";

interface GameBoardProps {
  game: Game;
  player: Player;
}

const GameBoard = (props: GameBoardProps) => {
  return <h1>Game: {props.game.gameId}</h1>;
};

export default GameBoard;
