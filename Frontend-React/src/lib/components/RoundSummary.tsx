import { LetterEvaluation, RoundSummaryParams } from "../../interface";
import React from "react";
import { WordAnimation } from "$lib/components/WordAnimation";
import { WordAnimation2 } from "$lib/components/WordAnimation2";

export function RoundSummary({ gameState: { game }, player }: RoundSummaryParams) {
  function sortEvaluations(a: LetterEvaluation, b: LetterEvaluation) {
    if (a.wordIndex < b.wordIndex) return -1;
    return 1;
  }

  const currentPlayerEvaluation = game?.roundSubmissions
    ?.find((e) => e.player.id === player.id && e.roundNumber === game?.currentRoundNumber)
    ?.letterEvaluations?.sort(sortEvaluations);

  const otherPlayerEvaluations = game?.roundSubmissions?.filter(
    (e) => e.player.id !== player.id && e.roundNumber === game?.currentRoundNumber
  );

  console.log(
    "test ",
    game?.roundSubmissions.find((e) => e.roundNumber === game?.currentRoundNumber)
  );

  console.log("current:", currentPlayerEvaluation);
  console.log("other: ", otherPlayerEvaluations);

  return (
    <>
      <section className="summary">
        <h3 className="font-kawoord text-2xl">You:</h3>
        {currentPlayerEvaluation ? (
          <WordAnimation2 evalArr={currentPlayerEvaluation} />
        ) : (
          <p>You didnt submit a word... 🤔 </p>
        )}
        <div className="spacer h-8" />
        <ul>
          {otherPlayerEvaluations ? (
            otherPlayerEvaluations.map((ev, i) => {
              return (
                <li key={ev.player.id}>
                  <div className="spacer h-8" />
                  {ev.letterEvaluations ? (
                    <WordAnimation
                      letters={ev.letterEvaluations.sort(sortEvaluations)}
                      delayMs={3000 + i * 1000} /*to show each player incrementally, we delay the animation start*/
                      player={ev.player.name}
                    />
                  ) : (
                    <p>{ev.player.name} didnt submit a word this round 🤔</p>
                  )}
                  <div className="spacer h-8" />
                </li>
              );
            })
          ) : (
            <p>Nobody else submitted a word this round 🤷‍</p>
          )}
        </ul>
      </section>
    </>
  );
}
