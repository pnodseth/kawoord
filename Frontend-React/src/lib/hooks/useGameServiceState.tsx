import { Game, GameServiceAction, GameserviceState, Player, Points, RoundInfo, RoundState } from "../../interface";
import { useEffect, useReducer, useState } from "react";
import { GameService } from "$lib/services/GameService";

function reducer(state: GameserviceState, action: GameServiceAction) {
  switch (action.type) {
    case "ROUNDINFO": {
      const newState: GameserviceState = { ...state, roundInfo: action.payload as RoundInfo };
      return newState;
    }

    case "ROUNDSTATE": {
      const newState: GameserviceState = { ...state, roundState: action.payload as RoundState };
      return newState;
    }
    case "POINTS": {
      const newState: GameserviceState = { ...state, points: action.payload as Points };
      return newState;
    }
    case "DISPLAY_NOTIFICATION":
      return { ...state, displayNotification: action.payload as string };
    case "GAME_UPDATE":
      return { ...state, game: action.payload as Game };
    default:
      return state;
  }
}

const initialState: GameserviceState = {
  displayNotification: "",
  points: undefined,
  roundState: undefined,
  roundInfo: undefined,
  game: undefined,
};

export const useGameServiceState = (player: Player) => {
  const [gameService, setGameService] = useState<GameService>();
  const [gameState, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    if (player) {
      setGameService(new GameService(player));
    }
  }, [player]);

  useEffect(() => {
    if (gameService && player) {
      gameService.registerCallbacks({
        onRoundInfo: (info) => {
          console.log(`info: ${JSON.stringify(info)}`);
          dispatch({ type: "ROUNDINFO", payload: info });
        },
        onRoundStateUpdate: (data: RoundState) => {
          console.log(`Got round state update: ${JSON.stringify(data)}`);
          dispatch({ type: "ROUNDSTATE", payload: data });
        },
        onPointsUpdate: (data: Points) => {
          console.log(`Got points: ${JSON.stringify(data)}`);
          dispatch({ type: "POINTS", payload: data });
        },
        onNotification: (msg) => {
          console.log(`Got display notification: ${msg}`);
          showNotification(msg);
        },
        onGameStateUpdateCallback(newState, updatedGame): void {
          dispatch({ type: "GAME_UPDATE", payload: updatedGame });
        },
        onPlayerJoinCallback(player, updatedGame): void {
          console.log("player joined: ", player, updatedGame);
          dispatch({ type: "GAME_UPDATE", payload: updatedGame });
        },
        onGameUpdate(game): void {
          console.log("yeah!1!", game);
          dispatch({ type: "GAME_UPDATE", payload: game });
        },
      });
    }

    function showNotification(msg: string, durationSec = 6): void {
      dispatch({
        type: "DISPLAY_NOTIFICATION",
        payload: msg,
      });

      setTimeout(() => {
        dispatch({
          type: "DISPLAY_NOTIFICATION",
          payload: "",
        });
      }, durationSec * 1000);
    }
  }, [gameService, player, gameState]);

  return { gameService, gameState };
};
