import { Game, PlayerLetterHints } from "../../interface";
import { InputGrid } from "$lib/components/InputGrid";
import React from "react";

export function RoundViewHeader(props: {
  game: Game;
  countDown: string;
  letterArr: string[];
  playerLetterHints: PlayerLetterHints | undefined;
}) {
  const correctLetters =
    props.game.currentRoundNumber != props.playerLetterHints?.roundNumber ? props.playerLetterHints?.correct : [];

  return (
    <div className="relative">
      <p className="font-kawoord text-3xl mb-2 ">Round {props.game?.currentRoundNumber}</p>
      {/*<p className="mb-4">Guess the 5 letter word before the time runs out!</p>*/}
      <p className="font-kawoord absolute right-0 top-0">{props.countDown}</p>
      <div className="spacer h-4" />
      <InputGrid letterArr={props.letterArr} correctLetters={correctLetters || []} />
      <div className="wrong-placement">
        {props.playerLetterHints?.roundNumber !== props.game.currentRoundNumber && (
          <p className="text-xl pl-2 text-yellow-400">
            {props.playerLetterHints?.wrongPosition
              .map((e) => e.letter)
              .join(",")
              .toUpperCase()}
          </p>
        )}
      </div>

      <div className="spacer h-8" />
    </div>
  );
}