import { GameState, LetterEvaluation, Player } from "../../interface";
import React, { useContext, useState } from "react";
import { RoundSummary } from "$lib/components/RoundSummary";
import { gameServiceContext } from "$lib/contexts/GameServiceContext";
import Keyboard from "$lib/components/Keyboard";
import Button from "$lib/components/Button";
import { RoundViewHeader } from "$lib/components/RoundViewHeader";
import { InputGrid } from "$lib/components/InputGrid";
import { PlayerSubmittedView } from "$lib/components/PlayerSubmittedView";
import { SyncLoader } from "react-spinners";
import { RoundViewEnum } from "$lib/components/constants";
import { PreRoundView } from "$lib/components/PreRoundView";
import AppLayout from "$lib/layout/AppLayout";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

interface PlayingProps {
  player: Player;
  gameState: GameState;
}

export function Playing({ gameState, player }: PlayingProps) {
  const [letterArr, setLetterArr] = useState<string[]>(["", "", "", "", ""]);
  const [letterIdx, setLetterIdx] = useState(0);
  const [submittedWord, setSubmittedWord] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const gameService = useContext(gameServiceContext);
  const currentRound = gameState.game?.rounds.find((round) => round.roundNumber === gameState.game?.currentRoundNumber);
  const [invalidWord, setInvalidWord] = useState(false);

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

    if (word.length !== 5) {
      throw new Error("Word length must be 5");
    }
    setSubmitting(true);
    try {
      await gameService.submitWord(word, gameState.game.gameId, player);
      resetLetterArr();
      setSubmittedWord(word);
    } catch (err) {
      setInvalidWord(true);

      setTimeout(() => {
        setInvalidWord(false);
      }, 3000);
    } finally {
      setSubmitting(false);
    }
  }

  if (!gameState.game) return null;

  if (currentRound?.roundViewEnum === RoundViewEnum.Playing) {
    return (
      <AppLayout padding="p-1" headerSize="small">
        <div className="playing-wrapper h-full flex flex-col">
          <div className=" px-8  pt-4">
            <RoundViewHeader
              game={gameState.game}
              currentRound={currentRound}
              letterArr={letterArr}
              playerLetterHints={currentPlayerLetterHints}
            />
          </div>
          {!playerHasSubmitted ? (
            <div className="flex flex-col justify-between flex-1">
              <div className="px-8" style={{ userSelect: "none" }}>
                <InputGrid invalidWord={invalidWord} letterArr={letterArr} correctLetters={correctLetterHints || []} />
                <div className="spacer h-4"></div>
              </div>
              <div className="keyboard-container px-1 md:px-8">
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

              <Button onClick={() => handleSubmit(letterArr.join(""))} disabled={submitting || invalidWord}>
                {!submitting ? "Submit" : <SyncLoader color="#FFF" />}
              </Button>
              <FixedBottomContent></FixedBottomContent>
            </div>
          ) : (
            <PlayerSubmittedView submittedWord={submittedWord} />
          )}
        </div>
      </AppLayout>
    );
  } else if (currentRound?.roundViewEnum === RoundViewEnum.NotStarted) {
    return <PreRoundView round={currentRound} />;
  }
  /* When round ends, display round summary and total score*/
  return <RoundSummary gameState={gameState} player={player} />;
}
