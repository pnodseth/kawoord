import React, { useContext } from "react";
import { GameState, Player } from "../interface";
import trophy from "../assets/trophy.svg";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/contexts/GameServiceContext";
import AppLayout from "$lib/layout/AppLayout";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

interface SolvedProps {
  gameState: GameState;
  player: Player;
}

export function Solved({ player, gameState }: SolvedProps) {
  const gameService = useContext(gameServiceContext);
  const winners = gameState.game?.roundSubmissions.filter((e) => e.isCorrectWord);

  return (
    <AppLayout noBg={true} headerSize="small">
      <div className="spacer h-0"></div>
      {winners && winners.length > 1 && <h1 className="font-kawoord text-xl">It`s a tie!</h1>}
      <div className="md:spacer h-2"></div>
      <div className="winners">
        {winners &&
          winners.map((winner) => {
            const name = winner.player.id === player.id ? "You" : winner.player.name;
            return (
              <h1 key={winner.player.id} className="font-kawoord text-3xl">
                {name} won!
              </h1>
            );
          })}
      </div>
      <p className="md:mt-4 xl:mt-16 xl:text-2xl">Correct word: {gameState.solution?.toUpperCase()}</p>
      <FixedBottomContent>
        <div className="img-container flex justify-center">
          <img src={trophy} alt="" style={{ maxWidth: "400px", maxHeight: "40vh", width: "328px", height: "328px" }} />
        </div>
        <Button onClick={() => gameService.clearGame()}>Play again?</Button>
      </FixedBottomContent>
    </AppLayout>
  );
}
