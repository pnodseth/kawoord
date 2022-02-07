import { Game, GameState, Player, Points, RoundInfo, RoundState } from "../../interface";
import { useEffect, useReducer, useState } from "react";
import { CallbackProps, GameService } from "$lib/services/GameService";

interface GameserviceState {
  roundInfo: RoundInfo | undefined;
  roundState: RoundState | undefined;
  points: Points | undefined;
  displayNotification: DisplayNotification;
}

interface DisplayNotification {
  msg: string;
  durationSec: number;
}

const initialState: GameserviceState = {
  displayNotification: { durationSec: 0, msg: "" },
  points: undefined,
  roundState: undefined,
  roundInfo: undefined,
};

interface Action {
  type: "ROUNDINFO" | "ROUNDSTATE" | "POINTS" | "DISPLAY_NOTIFICATION";
  payload: GameserviceState;
}

function reducer(state: GameserviceState, action: Action) {
  switch (action.type) {
    case "ROUNDINFO": {
      const newState: GameserviceState = { ...state, roundInfo: action.payload.roundInfo };
      return newState;
    }

    case "ROUNDSTATE": {
      const newState: GameserviceState = { ...state, roundState: action.payload.roundState };
      return newState;
    }
    case "POINTS": {
      const newState: GameserviceState = { ...state, points: action.payload.points };
      return newState;
    }
    case "DISPLAY_NOTIFICATION":
      return { ...state, displayNotification: action.payload.displayNotification };
    default:
      return state;
  }
}

export const useGameService = (player: Player, callbacks: CallbackProps) => {
  const [state, dispatch] = useReducer(reducer, initialState);
  const [gameService, setGameService] = useState<GameService>();
  const [game, setGame] = useState<Game>();

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
  }, [callbacks, gameService, player, state]);

  return { gameService, updatedGame: game };
};
