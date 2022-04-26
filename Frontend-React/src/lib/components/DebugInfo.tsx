import { GameState } from "../../interface";
import React from "react";

export function DebugInfo(props: { gameState: GameState }) {
  return (
    <div className="absolute bottom-4 left-4 p-4 border-black border-2">
      <h1 className="font-bold">Debug Info:</h1>
      <p>Game: {props.gameState.game?.gameId}</p>
      <p> Game State: {props.gameState.game?.state}</p>
      <p>Round Number: {props.gameState.roundInfo?.roundNumber}</p>
      <p>Round state: {props.gameState.roundState?.value}</p>
    </div>
  );
}
