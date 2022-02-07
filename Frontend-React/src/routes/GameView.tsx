import React, { FC, useState } from "react";
import { Player } from "$lib/components/Player";
import { useGameService } from "$lib/hooks/useGameService";
import { usePlayerName } from "$lib/hooks/hooks";
import { Game } from "../interface";
import GameBoard from "$lib/components/GameBoard";
import { NoGame } from "$lib/components/NoGame";

const GameView: FC = () => {
  const player = usePlayerName("");
  const [game, setGame] = useState<Game>();
  const { gameService } = useGameService(player, {
    onNotification(msg: string, durationSec: number | undefined): void {
      console.log("registered callback for notifications", msg, durationSec);
    },
    onPointsUpdate(points): void {
      console.log("got new points! ", points);
    },
    onRoundInfo(roundInfo): void {
      console.log("registered callback for roundinfo!", roundInfo);
    },
    onRoundStateUpdate(data): void {
      console.log("registered callback for roundState", data);
    },
    onGameStateUpdateCallback(newState, updatedGame) {
      console.log("registed callback for onRoundStateUpdate ", newState);
      setGame(updatedGame);
    },
    onPlayerJoinCallback(player, updatedGame): void {
      console.log("Registered callback for onPlayerJoin ", player, updatedGame);
      setGame(updatedGame);
    },
  });

  async function handleCreateGame() {
    const game = await gameService?.createGame();
    if (game) {
      setGame(game);
    }
  }

  async function handleJoinGame(gameId: string) {
    const game = await gameService?.joinGame(gameId);
    if (game) {
      setGame(game);
    }
  }

  return (
    <>
      <h1 className="text-xl text-center font-bold">Kawoord</h1>
      <Player />
      {!game ? (
        <NoGame onClick={handleCreateGame} onJoin={handleJoinGame} />
      ) : (
        <>
          <GameBoard game={game} player={player} />
        </>
      )}
    </>
  );
};
export default GameView;
