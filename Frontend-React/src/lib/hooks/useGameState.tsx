import { Game, GameServiceAction, GameState, Round, WordEvaluation, RoundView } from "../../interface";
import { useContext, useEffect, useReducer } from "react";
import { gameServiceContext } from "$lib/components/GameServiceContext";

function reducer(state: GameState, action: GameServiceAction) {
  switch (action.type) {
    case "ROUND_INFO": {
      const round: Round = action.payload as Round;
      const newState: GameState = {
        ...state,
        game: {
          ...(state.game as Game),
          rounds: [...(state.game?.rounds as Round[]), round],
          currentRoundNumber: round.roundNumber,
        },
      };
      return newState;
    }

    case "ROUND_STATE": {
      const newState: GameState = {
        ...state,
        game: { ...(state.game as Game), roundViewEnum: action.payload as RoundView },
      };
      return newState;
    }
    case "POINTS": {
      const newState: GameState = {
        ...state,
        evaluations: [...(state.evaluations ?? []), ...(action.payload as WordEvaluation[])],
      };
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

const initialState: GameState = {
  displayNotification: "",
  evaluations: undefined,
  game: undefined,
};

export const useGameState = () => {
  const gameService = useContext(gameServiceContext);
  const [gameState, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    if (gameService) {
      gameService.registerCallbacks({
        onRoundInfo: (info) => {
          console.log(`info: ${JSON.stringify(info)}`);
          dispatch({ type: "ROUND_INFO", payload: info });
        },
        onRoundStateUpdate: (data: RoundView) => {
          console.log(`Got round state update: ${JSON.stringify(data)}`);
          dispatch({ type: "ROUND_STATE", payload: data });
        },
        onPointsUpdate: (data: WordEvaluation[]) => {
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
          //console.log("player joined: ", player, updatedGame);
          //dispatch({ type: "GAME_UPDATE", payload: updatedGame });
        },
        onGameUpdate(game): void {
          console.log("game update!", game);
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
  }, [gameService, gameState]);

  return { gameState };
};
