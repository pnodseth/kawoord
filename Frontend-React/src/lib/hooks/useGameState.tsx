import { Game, GameServiceAction, GameState } from "../../interface";
import { useContext, useEffect, useReducer } from "react";
import { gameServiceContext } from "$lib/components/GameServiceContext";

function reducer(state: GameState, action: GameServiceAction) {
  switch (action.type) {
    case "ClEAR_GAME":
      return { ...state, game: undefined };
    case "DISPLAY_NOTIFICATION":
      return { ...state, displayNotification: action.payload as string };
    case "GAME_UPDATE":
      return { ...state, game: action.payload as Game };
    default:
      return state;
  }
}

const initialState: GameState = {
  displayNotification: "",
  game: undefined,
};

export const useGameState = () => {
  const gameService = useContext(gameServiceContext);
  const [gameState, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    if (gameService) {
      gameService.registerCallbacks({
        onNotification: (msg) => {
          console.log(`Got display notification: ${msg}`);
          showNotification(msg);
        },
        onGameUpdate(game): void {
          console.log("game update!", game);
          dispatch({ type: "GAME_UPDATE", payload: game });
        },
        onClearGame() {
          dispatch({ type: "ClEAR_GAME", payload: undefined });
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
  }, [gameService, gameState]);

  return { gameState };
};
