import { Game, GameState, LetterEvaluation, Player } from "../../interface";
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

function RoundViewHeader(props: {
  game: Game;
  countDown: string;
  letterArr: string[];
  correctLetters: LetterEvaluation[];
  wrongPlacement: LetterEvaluation[];
}) {
  return (
    <div className="relative">
      <p className="font-kawoord text-3xl mb-2 ">Round {props.game?.currentRoundNumber}</p>
      {/*<p className="mb-4">Guess the 5 letter word before the time runs out!</p>*/}
      <p className="font-kawoord absolute right-0 top-0">{props.countDown}</p>
      <div className="spacer h-4" />
      <InputGrid letterArr={props.letterArr} correctLetters={props.correctLetters} />
      <div className="wrong-placement">
        <p className="text-xl pl-2 text-yellow-400">
          {props.wrongPlacement
            .map((e) => e.letter)
            .join(",")
            .toUpperCase()}
        </p>
      </div>

      <div className="spacer h-8" />
    </div>
  );
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

  const playerSubmissions = gameState.game?.roundSubmissions?.filter((e) => e.player.id === player.id) || [];

  const correctLetters = playerSubmissions
    .map((submission) => {
      return submission.letterEvaluations.filter(
        (e) => e.letterValueType.value === "Correct" && e.round !== gameState.game?.currentRoundNumber
      );
    })
    .flat();

  const wrongPlacement = playerSubmissions
    .map((submission) => {
      return submission.letterEvaluations.filter((e) => e.letterValueType.value === "WrongPlacement");
    })
    .flat();

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

  if (!gameState.game) return;

  if (gameState.game.roundViewEnum.value === "Playing") {
    return (
      <div className="bg-white rounded p-8 h-[70vh] text-gray-600 text-center">
        <RoundViewHeader
          game={gameState.game}
          countDown={countDown}
          letterArr={letterArr}
          correctLetters={correctLetters}
          wrongPlacement={wrongPlacement}
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
