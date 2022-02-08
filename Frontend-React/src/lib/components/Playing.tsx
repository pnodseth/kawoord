import { GameserviceState, Player } from "../../interface";
import React, { useContext, useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";
import { RoundSummary } from "$lib/components/RoundSummary";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";

interface PlayingProps {
  player: Player;
  gameState: GameserviceState;
}

export function Playing({ gameState, player }: PlayingProps) {
  const [countDown, setCountDown] = useState("");
  const gameService = useContext(gameServiceContext);

  /* Set countdown timer */
  useEffect(() => {
    if (gameState.roundInfo?.roundEndsUtc) {
      const ends = gameState.roundInfo.roundEndsUtc;

      const intervalId = setInterval(() => {
        if (isBefore(new Date(), new Date(ends))) {
          setCountDown(`Ends in in: ${formatDistanceToNowStrict(new Date(ends))}`);
        } else {
          clearInterval(intervalId);
          setCountDown("Round has ended!");
        }
      }, 1000);

      return function cleanup() {
        clearInterval(intervalId);
      };
    }
  });

  function handleSubmit() {
    console.log("submitting word");
    gameService?.submitWord("feste");
  }

  if (gameState.roundState?.value === "Playing" || gameState.roundState?.value === "PlayerSubmitted") {
    return (
      <>
        <p>{countDown}</p>
        <div className="spacer h-8" />
        <div className="letters grid grid-cols-5 h-12  gap-3 px-12">
          <p className="border-black border-2">Hei</p>
          <p className="border-black border-2">Hei</p>
          <p className="border-black border-2">Hei</p>
          <p className="border-black border-2">Hei</p>
          <p className="border-black border-2">Hei</p>
        </div>
        <Button onClick={handleSubmit}>Submit</Button>
        <div className="spacer h-8" />
        <h2>Keyboard Here</h2>
      </>
    );
  }
  /* When game ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
