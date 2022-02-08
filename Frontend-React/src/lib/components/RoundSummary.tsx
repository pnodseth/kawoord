import { GameserviceState, Player, RoundEvaluation } from "../../interface";
import React from "react";
import { PlayerEvaluationWord } from "$lib/components/PlayerEvaluationWord";

interface RoundSummaryParams {
  gameState: GameserviceState;
  player: Player;
}

export function RoundSummary({ gameState: { evaluations }, player }: RoundSummaryParams) {
  function isCurrentPlayer(e: RoundEvaluation) {
    return e.player.id === player.id;
  }

  function isOtherPlayers(e: RoundEvaluation) {
    return e.player.id !== player.id;
  }

  return (
    <>
      <section className="summary">
        <h2>Summary</h2>
        <h3>Your Word:</h3>
        {/*YOUR WORD:*/}
        {evaluations?.roundEvaluations
          .filter((e) => isCurrentPlayer(e))
          .map((p) => {
            return <PlayerEvaluationWord key={p.player.id} evaluation={p} showLetter={true} />;
          })}
        {/*OTHER PLAYERS WORD*/}
        <h3>Other players:</h3>
        {evaluations?.roundEvaluations
          .filter((e) => isOtherPlayers(e))
          .map((p) => {
            return (
              <div key={p.player.id}>
                <h4>{p.player.name}</h4>
                <PlayerEvaluationWord evaluation={p} />
              </div>
            );
          })}
      </section>
    </>
  );
}
