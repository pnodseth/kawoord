import { GameState, Player } from "../../interface";
import React, { useContext, useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { InputGrid } from "$lib/components/InputGrid";
import Keyboard from "$lib/components/Keyboard";
import Button from "$lib/components/Button";

interface PlayingProps {
  player: Player;
  gameState: GameState;
}

export function Playing({ gameState, player }: PlayingProps) {
  const [countDown, setCountDown] = useState("");
  const [letterArr, setLetterArr] = useState<string[]>(["", "", "", "", ""]);
  const [letterIdx, setLetterIdx] = useState(0);

  const gameService = useContext(gameServiceContext);
  const currentRound = gameState.game?.rounds.find((round) => round.roundNumber === gameState.game?.currentRoundNumber);

  /* Set countdown timer */
  useEffect(() => {
    if (currentRound?.roundEndsUtc) {
      const ends = currentRound.roundEndsUtc;

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

  if (gameState.game?.roundViewEnum.value === "Playing" || gameState.game?.roundViewEnum.value === "PlayerSubmitted") {
    return (
      <div className="bg-white rounded p-8 h-[70vh] text-gray-600 text-center">
        <p className="font-kawoord text-3xl mb-2">Round {gameState.game.currentRoundNumber}</p>
        <p className="mb-4">Guess the 5 letter word before the time runs out!</p>
        <p className="font-kawoord">{countDown}</p>
        <div className="spacer h-8" />
        <InputGrid letterArr={letterArr} />
        <div className="spacer h-8" />
        {gameState.game?.roundViewEnum.value === "Playing" && (
          <>
            <Keyboard
              keyIndicators={{}}
              handleSubmit={handleSubmit}
              letterArr={letterArr}
              setLetterArr={setLetterArr}
              letterIdx={letterIdx}
              setLetterIdx={setLetterIdx}
            />
            <Button onClick={() => handleSubmit(letterArr.join(""))}>Submit</Button>
          </>
        )}
        {gameState.game?.roundViewEnum.value === "PlayerSubmitted" && (
          <>
            <h2 className="font-kawoord text-2xl">Great job!</h2>
            <p className=" mt-6 animate-bounce">Waiting for other players to submit their word also...</p>
          </>
        )}
      </div>
    );
  }
  /* When game ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
