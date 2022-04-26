import React from "react";
import type { GameState, Player } from "../../interface";
import { Starting } from "$lib/components/Starting";
import { Playing } from "$lib/components/Playing";
import { DebugInfo } from "$lib/components/DebugInfo";

interface GameBoardProps {
  player: Player;
  gameState: GameState;
}

const GameBoard = (props: GameBoardProps) => {
  return (
    <>
      {/*<DebugInfo gameState={props.gameState} />*/}
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
