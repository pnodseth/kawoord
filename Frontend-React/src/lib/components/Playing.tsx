import { GameState, Player } from "../../interface";
import React, { useContext, useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import Keyboard from "$lib/components/Keyboard";
import Button from "$lib/components/Button";
import { RoundViewHeader } from "$lib/components/RoundViewHeader";

interface PlayingProps {
  player: Player;
  gameState: GameState;
}

function PlayerHasSubmitted() {
  return (
    <>
      <h2 className="font-kawoord text-2xl">Great job!</h2>
      <p className=" mt-6 animate-bounce">Waiting for other players to submit their word also...</p>
    </>
  );
}

export function Playing({ gameState, player }: PlayingProps) {
  const [countDown, setCountDown] = useState("");
  const [letterArr, setLetterArr] = useState<string[]>(["", "", "", "", ""]);
  const [letterIdx, setLetterIdx] = useState(0);

  const gameService = useContext(gameServiceContext);
  const currentRound = gameState.game?.rounds.find((round) => round.roundNumber === gameState.game?.currentRoundNumber);

  const playerHasSubmitted = gameState.game?.roundSubmissions.find(
    (e) => e.roundNumber === currentRound?.roundNumber && e.player.id === player.id
  );

  const currentPlayerLetterHints = gameState.game?.playerLetterHints.find((e) => e.player.id === player.id);

  const resetLetterArr = () => {
    setLetterArr(["", "", "", "", ""]);
    setLetterIdx(0);
  };

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
    resetLetterArr();
  }

  if (!gameState.game) return;

  if (gameState.game.roundViewEnum.value === "Playing") {
    return (
      <div className="bg-white rounded p-8 h-[70vh] text-gray-600 text-center">
        <RoundViewHeader
          game={gameState.game}
          countDown={countDown}
          letterArr={letterArr}
          playerLetterHints={currentPlayerLetterHints}
        />
        {!playerHasSubmitted ? (
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
        ) : (
          <PlayerHasSubmitted />
        )}
      </div>
    );
  } else if (gameState.game?.roundViewEnum.value === "NotStarted") {
    return <h2>Round is starting...</h2>;
  }
  /* When round ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
