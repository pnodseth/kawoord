import React from "react";
import type { Game, GameserviceState, Player } from "../../interface";

interface GameBoardProps {
  game: Game;
  player: Player;
  gameState: GameserviceState;
}

const GameBoard = ({ game: { gameId }, gameState: { game, roundInfo, roundState } }: GameBoardProps) => {
  return (
    <>
      <h1>Game: {gameId}</h1>
      <p> Game State: {game?.state}</p>
      <p>Round Number: {roundInfo?.roundNumber}</p>
      <p>Round state: {roundState?.value}</p>
    </>
  );
};

export default GameBoard;
