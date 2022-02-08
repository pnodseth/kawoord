import { GameserviceState, Player } from "../../interface";
import React, { useContext, useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";
import { RoundSummary } from "$lib/components/RoundSummary";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { InputGrid } from "$lib/components/InputGrid";

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

  function handleSubmit(word: string) {
    console.log("submitting word", word);
    if (word.length !== 5) {
      throw new Error("Word length must be 5");
    }
    gameService?.submitWord(word);
  }

  if (gameState.roundState?.value === "Playing" || gameState.roundState?.value === "PlayerSubmitted") {
    return (
      <div className="bg-red-500">
        <p>{countDown}</p>
        <div className="spacer h-8" />
        <InputGrid handleSubmit={handleSubmit} />
        <Button onClick={() => handleSubmit("sdfsfddf")}>Submit</Button>
        <div className="spacer h-8" />
        <h2>Keyboard Here</h2>
      </div>
    );
  }
  /* When game ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
