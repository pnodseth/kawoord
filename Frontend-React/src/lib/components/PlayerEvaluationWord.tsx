import { LetterEvaluation, RoundEvaluation } from "../../interface";
import React from "react";
import { LetterTile } from "$lib/components/LetterTile";

interface PlayerEvaluationWordParams {
  evaluation: RoundEvaluation;
  showLetter?: boolean;
}

export function PlayerEvaluationWord({ evaluation: { player, evaluation }, showLetter }: PlayerEvaluationWordParams) {
  function sortEvaluations(a: LetterEvaluation, b: LetterEvaluation) {
    if (a.wordIndex < b.wordIndex) return -1;
    return 1;
  }

  return (
    <ul className="letters grid grid-cols-5 h-12  gap-3 px-12">
      {evaluation?.sort(sortEvaluations).map((e) => {
        return <LetterTile key={e.wordIndex + "-" + player.id} e={e} showLetter={showLetter} />;
      })}
    </ul>
  );
}