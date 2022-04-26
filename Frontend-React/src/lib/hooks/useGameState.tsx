import {
  Evaluations,
  Game,
  GameServiceAction,
  GameState,
  GameStats,
  Round,
  RoundEvaluation,
  RoundState,
} from "../../interface";
import { useContext, useEffect, useReducer } from "react";
import { gameServiceContext } from "$lib/components/GameServiceContext";

function reducer(state: GameState, action: GameServiceAction) {
  switch (action.type) {
    case "STATS": {
      const newState: GameState = { ...state, stats: action.payload as GameStats };
      return newState;
    }

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
        game: { ...(state.game as Game), currentRoundState: action.payload as RoundState },
      };
      return newState;
    }
    case "POINTS": {
      const newState: GameState = { ...state, evaluations: action.payload as RoundEvaluation[] };
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
  stats: undefined,
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
        onRoundStateUpdate: (data: RoundState) => {
          console.log(`Got round state update: ${JSON.stringify(data)}`);
          dispatch({ type: "ROUND_STATE", payload: data });
        },
        onPointsUpdate: (data: RoundEvaluation[]) => {
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
        onStats(stats): void {
          console.log("stats arrived: ", stats);
          dispatch({ type: "STATS", payload: stats });
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
