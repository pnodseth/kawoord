import React, { FC, useContext } from "react";
import { Player } from "$lib/components/Player";
import { useGameServiceState } from "$lib/hooks/useGameServiceState";
import { usePlayerName } from "$lib/hooks/hooks";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";
import Lobby from "$lib/components/Lobby";
import { GameServiceContext } from "$lib/components/GameServiceContext";

const GameView: FC = () => {
  const player = usePlayerName("");
  const { gameService, gameState } = useGameServiceState(player);
  const value = useContext(GameServiceContext);
  async function handleCreateGame() {
    await gameService?.createGame();
  }

  async function handleJoinGame(gameId: string) {
    await gameService?.joinGame(gameId);
  }

  return (
    <>
      <h1 className="text-xl text-center font-bold">Kawoord</h1>
      <p>Context: {value}</p>
      <Player />
      {!gameState.game ? (
        <NoGame onClick={handleCreateGame} onJoin={handleJoinGame} />
      ) : (
        <>
          {gameState.game.state === "Lobby" ? (
            <Lobby gameState={gameState} player={player} gameService={gameService} />
          ) : (
            <GameBoard player={player} gameState={gameState} />
          )}
        </>
      )}
    </>
  );
};
export default GameView;
