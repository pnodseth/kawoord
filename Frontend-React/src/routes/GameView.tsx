import React, { FC } from "react";
import { Player } from "$lib/components/Player";
import { useGameService } from "$lib/hooks/useGameService";
import { usePlayerName } from "$lib/hooks/hooks";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";

const GameView: FC = () => {
  const player = usePlayerName("");
  const { gameService, gameState } = useGameService(player);

  async function handleCreateGame() {
    await gameService?.createGame();
  }

  async function handleJoinGame(gameId: string) {
    await gameService?.joinGame(gameId);
  }

  return (
    <>
      <h1 className="text-xl text-center font-bold">Kawoord</h1>
      <Player />
      {!gameState.game ? (
        <NoGame onClick={handleCreateGame} onJoin={handleJoinGame} />
      ) : (
        <>
          {gameState.game.state === "Lobby" ? (
            <Lobby gameState={gameState} player={player} />
          ) : (
            <GameBoard game={gameState.game} player={player} gameState={gameState} />
          )}
        </>
      )}
    </>
  );
};
export default GameView;
