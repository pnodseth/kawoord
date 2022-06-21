import { GameState, LetterEvaluation, Player, PlayerLetterHints } from "../../interface";
import React, { useContext, useState } from "react";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import Keyboard from "$lib/components/Keyboard";
import Button from "$lib/components/Button";
import { RoundViewHeader } from "$lib/components/RoundViewHeader";
import { InputGrid } from "$lib/components/InputGrid";
import { PlayerSubmittedView } from "$lib/components/PlayerSubmittedView";
import { WrongPlacementLetters } from "$lib/components/WrongPlacementLetters";

interface PlayingProps {
  player: Player;
  gameState: GameState;
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
  const wrongPositionLetterHints =
    gameState.game?.currentRoundNumber != currentPlayerLetterHints?.roundNumber
      ? currentPlayerLetterHints?.wrongPosition
      : [];
  const wrongLetterHints =
    gameState.game?.currentRoundNumber != currentPlayerLetterHints?.roundNumber ? currentPlayerLetterHints?.wrong : [];

  let allLetterHints: LetterEvaluation[] = [];
  if (correctLetterHints && wrongPositionLetterHints && wrongLetterHints) {
    allLetterHints = [...correctLetterHints, ...wrongPositionLetterHints, ...wrongLetterHints];
  }

  const resetLetterArr = () => {
    setLetterArr(["", "", "", "", ""]);
    setLetterIdx(0);
  };

  async function handleSubmit(word: string) {
    if (!gameState.game?.gameId) return;

    console.log("submitting word", word);
    if (word.length !== 5) {
      throw new Error("Word length must be 5");
    }
    try {
      //todo: Add submitting state
      await gameService.submitWord(word, gameState.game.gameId);
      resetLetterArr();
      setSubmittedWord(word);
    } catch (err) {
      console.log("error submitting word", err);
    }
  }

  if (!gameState.game) return null;

  if (gameState.game.roundViewEnum.value === "Playing") {
    return (
      <div className="playing-wrapper bg-white rounded  h-[70vh] text-gray-600 text-center flex flex-col pb-2">
        <div className="px-8  pt-4">
          <RoundViewHeader
            game={gameState.game}
            currentRound={currentRound}
            letterArr={letterArr}
            playerLetterHints={currentPlayerLetterHints}
          />
        </div>
        {!playerHasSubmitted && (
          <div className="px-8" style={{ userSelect: "none" }}>
            <InputGrid letterArr={letterArr} correctLetters={correctLetterHints || []} />
            <WrongPlacementLetters
              currentPlayerLetterHints={currentPlayerLetterHints}
              game={gameState.game}
              letterArr={letterArr}
            />
          </div>
        )}
        {!playerHasSubmitted ? (
          <>
            <div className="keyboard-container px-2">
              <Keyboard
                letterHints={allLetterHints}
                handleSubmit={handleSubmit}
                letterArr={letterArr}
                setLetterArr={setLetterArr}
                letterIdx={letterIdx}
                setLetterIdx={setLetterIdx}
              />
            </div>
            <div className="spacer  h-1 sm:h-8 md:h8 mt-auto" />
            <div>
              <Button onClick={() => handleSubmit(letterArr.join(""))}>Submit</Button>
            </div>
          </>
        ) : (
          <PlayerSubmittedView submittedWord={submittedWord} />
        )}
      </div>
    );
  } else if (gameState.game?.roundViewEnum.value === "NotStarted") {
    return <h2>Round is starting...</h2>;
  }
  /* When round ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
