import React from "react";
import type { GameState, Player } from "../../interface";
import { Starting } from "$lib/components/Starting";
import { Playing } from "$lib/components/Playing";
import { GameViewEnum } from "$lib/components/constants";

interface GameBoardProps {
  player: Player;
  gameState: GameState;
}

const GameBoard = (props: GameBoardProps) => {
  if (!props.gameState.game) return;
  return (
    <>
      {props.gameState.game.gameViewEnum === GameViewEnum.Starting ? (
        <Starting game={props.gameState.game} />
      ) : (
        <Playing gameState={props.gameState} player={props.player} />
      )}
    </>
  );
};

export default GameBoard;
