import { GameserviceState, Player } from "../../interface";
import React from "react";

interface RoundSummaryParams {
  gameState: GameserviceState;
  player: Player;
}

export function RoundSummary({ gameState: { points, roundInfo } }: RoundSummaryParams) {
  return (
    <>
      <section className="summary">
        <h2>Summary</h2>
        <p>Round Points:</p>
        {points &&
          points.roundPoints.map((p) => {
            return (
              <li key={p.player.id}>
                {p.player.name}: {p.points} points
              </li>
            );
          })}
        <p>Total points:</p>
        <p>After round {roundInfo?.roundNumber}, this is the score:</p>
        <ul>
          {points?.totalPoints.map((p) => {
            return (
              <li key={p.player.id}>
                {p.player.name}: {p.points} points
              </li>
            );
          })}
        </ul>
      </section>
    </>
  );
}
