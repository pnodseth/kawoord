import { GameState } from "../../interface";
import React from "react";

export function DebugInfo(props: { gameState: GameState }) {
  const currentRound = props.gameState.game?.rounds.find(
    (round) => round.roundNumber === props.gameState.game?.currentRoundNumber
  );

  return (
    <div className="absolute bottom-4 left-4 p-4 border-black border-2">
      <h1 className="font-bold">Debug Info:</h1>
      <p>Game: {props.gameState.game?.gameId}</p>
      <p> Game State: {props.gameState.game?.gameViewEnum}</p>
      <p>Round Number: {props.gameState.game?.currentRoundNumber}</p>
      <p>Round state: {props.gameState.game?.roundViewEnum}</p>
    </div>
  );
}
