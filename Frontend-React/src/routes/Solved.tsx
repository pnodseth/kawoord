import React, { useContext, useEffect, useState } from "react";
import { GameState, Player } from "../interface";
import { RoundSummary } from "$lib/components/RoundSummary";
import trophy from "../assets/trophy.svg";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";

interface SolvedProps {
  gameState: GameState;
  player: Player;
}

type SolvedView = "summary" | "oneWinner" | "manyWinners";

export function Solved({ player, gameState }: SolvedProps) {
  const [view, setView] = useState<SolvedView>("summary");
  const gameService = useContext(gameServiceContext);
  const winners = gameState.game?.roundSubmissions.filter((e) => e.isCorrectWord);

  useEffect(() => {
    setTimeout(() => {
      if (winners && winners.length === 1) {
        console.log("should be a winner here...");
        setView("oneWinner");
      } else {
        setView("manyWinners");
      }
    }, 6000);
  }, [winners]);

  const getWinnerNames = (): string[] => {
    const names = winners?.map((e) => e.player.name);
    return ["Pål", "peter"];
  };

  console.log("winners: ", winners);

  if (view === "summary") {
    return <RoundSummary gameState={gameState} player={player} />;
  } else if (view === "oneWinner") {
    return (
      <div className="text-center">
        <div className="spacer h-12"></div>
        <h1 className="font-kawoord text-3xl">{winners && winners[0].player?.name} won!</h1>
        <div className="spacer h-0"></div>
        <div className="img-container flex justify-center">
          <img src={trophy} alt="" style={{ maxWidth: "400px", maxHeight: "40vh" }} />
        </div>
        <Button onClick={() => gameService.clearGame()}>Play again?</Button>
      </div>
    );
  } else if (view === "manyWinners") {
    return (
      <div className="text-center">
        <div className="spacer h-12"></div>
        <h1 className="font-kawoord text-3xl">It`s a tie!</h1>
        <div className="spacer h-8"></div>
        <div className="winners">
          {winners &&
            winners.map((winner) => {
              return (
                <h1 key={winner.player.id} className="font-kawoord text-3xl">
                  {winner.player?.name} won!
                </h1>
              );
            })}
        </div>
        <div className="img-container flex justify-center">
          <img src={trophy} alt="" style={{ maxWidth: "400px", maxHeight: "40vh" }} />
        </div>
        <Button onClick={() => gameService.clearGame()}>Play again?</Button>
      </div>
    );
  }

  return null;
}
