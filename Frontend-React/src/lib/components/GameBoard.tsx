import React from "react";
import type { GameserviceState, Player } from "../../interface";
import { Starting } from "$lib/components/Starting";
import { Playing } from "$lib/components/Playing";

interface GameBoardProps {
  player: Player;
  gameState: GameserviceState;
}

const GameBoard = (props: GameBoardProps) => {
  return (
    <>
      <h1>Game: {props.gameState.game?.gameId}</h1>
      <p> Game State: {props.gameState.game?.state}</p>
      <p>Round Number: {props.gameState.roundInfo?.roundNumber}</p>
      <p>Round state: {props.gameState.roundState?.value}</p>
      {props.gameState.game && (
        <>
          {props.gameState.game.state === "Starting" ? (
            <Starting game={props.gameState.game} />
          ) : (
            <Playing gameState={props.gameState} player={props.player} />
          )}
        </>
      )}
    </>
  );
};

export default GameBoard;
