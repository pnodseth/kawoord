import { Player, Points, RoundState } from "../../interface";
import { useEffect, useState } from "react";
import { CallbackProps, GameService } from "$lib/services/GameService";

export const useGameService = (player: Player, callbacks: CallbackProps) => {
  const [gameService, setGameService] = useState<GameService>();

  useEffect(() => {
    if (player) {
      setGameService(new GameService(player));
    }
  }, [player]);

  useEffect(() => {
    if (gameService && player && callbacks) {
      gameService.registerCallbacks({
        onRoundInfo: (info) => {
          console.log(`info: ${JSON.stringify(info)}`);
          callbacks.onRoundInfo(info);
        },
        onRoundStateUpdate: (data: RoundState) => {
          console.log(`Got round state update: ${JSON.stringify(data)}`);
          callbacks.onRoundStateUpdate(data);
        },
        onPointsUpdate: (data: Points) => {
          console.log(`Got points: ${JSON.stringify(data)}`);
          callbacks.onPointsUpdate(data);
        },
        onNotification: (msg, durationSec) => {
          console.log(`Got display notification: ${msg}`);
          callbacks.onNotification(msg, durationSec);
        },
        onGameStateUpdateCallback(newState, updatedGame): void {
          callbacks.onGameStateUpdateCallback(newState, updatedGame);
        },
        onPlayerJoinCallback(player, updatedGame): void {
          callbacks.onPlayerJoinCallback(player, updatedGame);
        },
      });
    }
  }, [callbacks, gameService, player]);

  return { gameService };
};
