import { Game, PlayerLetterHints, Round } from "../../interface";
import React from "react";
import { CountDown } from "$lib/components/CountDown";

export function RoundViewHeader(props: {
  game: Game;
  letterArr: string[];
  playerLetterHints: PlayerLetterHints | undefined;
  currentRound: Round | undefined;
}) {
  return (
    <div className="relative">
      <p className="font-kawoord text-3xl mb-0 ">Round {props.game?.currentRoundNumber}</p>
      <CountDown countDownTo={props.currentRound?.roundEndsUtc}></CountDown>
      <div className="spacer h-4" />
      <div className="spacer h-8" />
    </div>
  );
}
