import { LetterEvaluation, RoundSummaryParams } from "../../interface";
import React from "react";
import { WordAnimation } from "$lib/components/WordAnimation";
import { WordAnimation2 } from "$lib/components/WordAnimation2";

export function RoundSummary({ gameState: { evaluations }, player }: RoundSummaryParams) {
  function sortEvaluations(a: LetterEvaluation, b: LetterEvaluation) {
    if (a.wordIndex < b.wordIndex) return -1;
    return 1;
  }

  const currentPlayerEvaluation = evaluations?.roundEvaluations
    .find((e) => e.player.id === player.id)
    ?.evaluation?.sort(sortEvaluations);

  const otherPlayerEvaluations = evaluations?.roundEvaluations.filter((e) => e.player.id !== player.id);

  return (
    <>
      <section className="summary">
        <h3 className="font-kawoord text-2xl">You:</h3>
        {currentPlayerEvaluation && <WordAnimation2 evalArr={currentPlayerEvaluation} />}
        <div className="spacer h-8" />
        {otherPlayerEvaluations &&
          otherPlayerEvaluations.map((ev, i) => {
            return (
              <li key={ev.player.id}>
                <div className="spacer h-8" />
                <WordAnimation
                  letters={ev.evaluation.sort(sortEvaluations)}
                  delayMs={3000 + i * 1000} /*to show each player incrementally, we delay the animation start*/
                  player={ev.player.name}
                />
                <div className="spacer h-8" />
              </li>
            );
          })}
      </section>
    </>
  );
}
