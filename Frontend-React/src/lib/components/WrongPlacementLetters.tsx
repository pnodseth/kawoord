import { Game, LetterEvaluation, PlayerLetterHints } from "../../interface";
import React, { useEffect, useState } from "react";

export function WrongPlacementLetters(props: {
  currentPlayerLetterHints: PlayerLetterHints | undefined;
  game: Game;
  letterArr: string[];
}) {
  const [displayedLetters, setDisplayedLetters] = useState<LetterEvaluation[]>([]);
  useEffect(() => {
    if (!props.currentPlayerLetterHints) return;

    const letterArrCopy: Array<string | null> = [...props.letterArr];
    const wrongCopy = [...props.currentPlayerLetterHints?.wrongPosition];

    props.currentPlayerLetterHints.wrongPosition.forEach((el) => {
      const foundIdx = letterArrCopy.indexOf(el.letter);
      if (foundIdx !== -1) {
        letterArrCopy[foundIdx] = null;
        const cpIdx = wrongCopy.findIndex((e) => e.wordIndex === el.wordIndex);
        if (cpIdx !== -1) {
          wrongCopy.splice(cpIdx, 1);
        }
      }
    });

    setDisplayedLetters(wrongCopy);
  }, [props.currentPlayerLetterHints, props.letterArr]);

  return (
    <div className="wrong-placement h-8">
      {props.currentPlayerLetterHints?.roundNumber !== props.game.currentRoundNumber && (
        <p className="text-xl pl-2 text-yellow-400">
          {displayedLetters
            .map((e: LetterEvaluation) => e.letter)
            .join(",")
            .toUpperCase()}
        </p>
      )}
    </div>
  );
}
