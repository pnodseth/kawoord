import { Game, PlayerLetterHints, Round } from "../../interface";
import React from "react";
import { useCountDownTo } from "$lib/hooks/useCountDownTo";

export function RoundViewHeader(props: {
  game: Game;
  letterArr: string[];
  playerLetterHints: PlayerLetterHints | undefined;
  currentRound: Round | undefined;
}) {
  const countdown = useCountDownTo(new Date(props.currentRound?.roundEndsUtc || ""));
  return (
    <div className="relative">
      <p className="font-kawoord text-3xl mb-0 ">Round {props.game?.currentRoundNumber}</p>
      <p>{countdown}</p>
      <div className="spacer h-4" />
      <div className="spacer md:h-8" />
    </div>
  );
}
