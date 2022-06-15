import { Game, GameState, LetterEvaluation, Player, PlayerLetterHints } from "../../interface";
import React, { useContext, useState } from "react";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import Keyboard from "$lib/components/Keyboard";
import Button from "$lib/components/Button";
import { RoundViewHeader } from "$lib/components/RoundViewHeader";
import { InputGrid } from "$lib/components/InputGrid";

interface PlayingProps {
  player: Player;
  gameState: GameState;
}

interface PlayerHasSubmittedProps {
  submittedWord: string;
}

const PlayerHasSubmitted = ({ submittedWord }: PlayerHasSubmittedProps) => (
  <>
    <h2 className="font-kawoord text-2xl">Great job!</h2>
    <div className="spacer h-6"></div>
    <h2 className="font-kawoord text-xl">You submitted: {submittedWord}</h2>
    <div className="spacer h-10"></div>
    <p className=" mt-6 animate-bounce">Waiting for other players to submit their word also...</p>
  </>
);

function WrongPlacementLetters(props: { currentPlayerLetterHints: PlayerLetterHints | undefined; game: Game }) {
  return (
    <div className="wrong-placement">
      {props.currentPlayerLetterHints?.roundNumber !== props.game.currentRoundNumber && (
        <p className="text-xl pl-2 text-yellow-400">
          {props.currentPlayerLetterHints?.wrongPosition
            .map((e: LetterEvaluation) => e.letter)
            .join(",")
            .toUpperCase()}
        </p>
      )}
    </div>
  );
}

export function Playing({ gameState, player }: PlayingProps) {
  const [letterArr, setLetterArr] = useState<string[]>(["", "", "", "", ""]);
  const [letterIdx, setLetterIdx] = useState(0);
  const [submittedWord, setSubmittedWord] = useState("");

  const gameService = useContext(gameServiceContext);
  const currentRound = gameState.game?.rounds.find((round) => round.roundNumber === gameState.game?.currentRoundNumber);

  const playerHasSubmitted = gameState.game?.roundSubmissions.find(
    (e) => e.roundNumber === currentRound?.roundNumber && e.player.id === player.id
  );

  const currentPlayerLetterHints = gameState.game?.playerLetterHints.find((e) => e.player.id === player.id);
  const correctLetterHints =
    gameState.game?.currentRoundNumber != currentPlayerLetterHints?.roundNumber
      ? currentPlayerLetterHints?.correct
      : [];

  const resetLetterArr = () => {
    setLetterArr(["", "", "", "", ""]);
    setLetterIdx(0);
  };

  function handleSubmit(word: string) {
    console.log("submitting word", word);
    if (word.length !== 5) {
      throw new Error("Word length must be 5");
    }
    gameService?.submitWord(word);
    resetLetterArr();
    setSubmittedWord(word);
  }

  if (!gameState.game) return;

  if (gameState.game.roundViewEnum.value === "Playing") {
    return (
      <div className="bg-white rounded p-8 h-[70vh] text-gray-600 text-center">
        <RoundViewHeader
          game={gameState.game}
          currentRound={currentRound}
          letterArr={letterArr}
          playerLetterHints={currentPlayerLetterHints}
        />
        {!playerHasSubmitted && (
          <>
            <InputGrid letterArr={letterArr} correctLetters={correctLetterHints || []} />
            <WrongPlacementLetters currentPlayerLetterHints={currentPlayerLetterHints} game={gameState.game} />
          </>
        )}
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
          <PlayerHasSubmitted submittedWord={submittedWord} />
        )}
      </div>
    );
  } else if (gameState.game?.roundViewEnum.value === "NotStarted") {
    return <h2>Round is starting...</h2>;
  }
  /* When round ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
