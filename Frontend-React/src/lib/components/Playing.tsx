import { GameserviceState, Player } from "../../interface";
import React, { useContext, useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { InputGrid } from "$lib/components/InputGrid";
import Keyboard from "$lib/components/Keyboard";

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
          setCountDown(`${formatDistanceToNowStrict(new Date(ends))}`);
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
      <div className="bg-white rounded p-8 h-[70vh] text-gray-600 text-center">
        <p className="font-kawoord text-3xl mb-2">Round {gameState.roundInfo?.roundNumber}</p>
        <p className="mb-4">Guess the 5 letter word before the time runs out!</p>
        <p className="font-kawoord">{countDown}</p>
        <div className="spacer h-8" />
        <InputGrid handleSubmit={handleSubmit} />
        <div className="spacer h-8" />
        <Keyboard keyIndicators={{}} handleSubmit={handleSubmit} />
      </div>
    );
  }
  /* When game ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
