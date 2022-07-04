import React, { useContext, useEffect, useState } from "react";
import { GameState, Player } from "../interface";
import trophy from "../assets/trophy.svg";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import AppLayout from "$lib/layout/AppLayout";

interface SolvedProps {
  gameState: GameState;
  player: Player;
}

type SolvedView = "summary" | "oneWinner" | "manyWinners";

export function Solved({ player, gameState }: SolvedProps) {
  const [view, setView] = useState<SolvedView>("oneWinner");
  const gameService = useContext(gameServiceContext);
  const winners = gameState.game?.roundSubmissions.filter((e) => e.isCorrectWord);

  useEffect(() => {
    if (winners && winners.length === 1) {
      setView("oneWinner");
    } else {
      setView("manyWinners");
    }
  }, [winners]);

  if (view === "oneWinner") {
    return (
      <AppLayout noBg={true}>
        <div className="spacer h-12"></div>
        <h1 className="font-kawoord text-3xl">{winners && winners[0].player?.name} won!</h1>
        <div className="spacer h-0"></div>
        <div className="img-container flex justify-center">
          <img src={trophy} alt="" style={{ maxWidth: "400px", maxHeight: "40vh" }} />
        </div>
        <Button onClick={() => gameService.clearGame()}>Play again?</Button>
      </AppLayout>
    );
  } else if (view === "manyWinners") {
    return (
      <AppLayout noBg={true}>
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
      </AppLayout>
    );
  }

  return null;
}
