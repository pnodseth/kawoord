import { LetterEvaluation, RoundSummaryParams } from "../../interface";
import React from "react";
import { WordAnimation } from "$lib/components/WordAnimation";
import { WordAnimation2 } from "$lib/components/WordAnimation2";
import { motion } from "framer-motion";

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

  const allOtherPlayerNames = game?.players.filter((p) => p.id !== player.id);

  const otherEvals =
    allOtherPlayerNames?.map((p) => {
      return {
        player: p,
        eval: otherPlayerEvaluations?.find((e) => e.player.id === p.id),
      };
    }) || [];

  return (
    <motion.div animate={{ opacity: [0, 1] }} transition={{ duration: 0.4, type: "spring" }} className="summary">
      <h1 className="text-3xl font-kawoord text-center py-8">Round {game?.currentRoundNumber} summary</h1>
      <h3 className="font-kawoord text-2xl">You:</h3>
      {currentPlayerEvaluation ? (
        <WordAnimation2 evalArr={currentPlayerEvaluation} showLetters={true} />
      ) : (
        <p>You didnt submit a word... 🤔 </p>
      )}
      <div className="spacer h-4" />
      <ul>
        {otherEvals.map((ev, i) => {
          return (
            <li key={ev.player.id}>
              <h3 className="font-kawoord text-2xl mb-3">{ev.player.name}</h3>
              {ev.eval ? (
                <WordAnimation
                  letters={ev.eval.letterEvaluations.sort(sortEvaluations)}
                  delayMs={3000 + i * 1000} /*to show each player incrementally, we delay the animation start*/
                  player={ev.player.name}
                  showLetters={false}
                />
              ) : (
                "Didn't submit a word this round 🤔"
              )}
              <div className="spacer h-4"></div>
            </li>
          );
        })}
      </ul>
    </motion.div>
  );
}
