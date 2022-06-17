import React, { useContext } from "react";
import Button from "$lib/components/Button";
import { gameServiceContext } from "$lib/components/GameServiceContext";

export function EndedUnsolved() {
  const gameService = useContext(gameServiceContext);

  return (
    <div className="text-center">
      <h1 className="font-kawoord text-4xl my-8">Game ended</h1>
      <h3 className="font-kawoord text-xl mb-8">Unsolved 😞</h3>
      <p className="mb-4">The correct word was...</p>
      <p className="font-kawoord text-2xl">SOLUTION</p>
      <div className="spacer h-8"></div>
      <Button onClick={() => gameService.clearGame()}>Play again?</Button>
    </div>
  );
}
