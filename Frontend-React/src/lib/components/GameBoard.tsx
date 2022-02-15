import React from "react";
import type { GameserviceState, Player } from "../../interface";
import { Starting } from "$lib/components/Starting";
import { Playing } from "$lib/components/Playing";

interface GameBoardProps {
  player: Player;
  gameState: GameserviceState;
}

function DebugInfo(props: { gameState: GameserviceState }) {
  return (
    <div className="absolute top-4 left-4 p-4 border-black border-2">
      <h1 className="font-bold">Debug Info:</h1>
      <p>Game: {props.gameState.game?.gameId}</p>
      <p> Game State: {props.gameState.game?.state}</p>
      <p>Round Number: {props.gameState.roundInfo?.roundNumber}</p>
      <p>Round state: {props.gameState.roundState?.value}</p>
    </div>
  );
}

const GameBoard = (props: GameBoardProps) => {
  return (
    <>
      <DebugInfo gameState={props.gameState} />
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
