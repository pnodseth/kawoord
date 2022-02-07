export interface Game {
  players: Player[];
  hostPlayer: Player;
  gameId: string;
  state: GameState;
  startedTime: number;
  endedTime: number;
  currentRoundNumber: number;
}

export interface Player {
  name: string;
  id: string;
}

export interface Notification {
  show: boolean;
  msg: string;
}

export type GameState = "Lobby" | "Starting" | "Started" | "Ended";

export interface KeyIndicator {
  [key: string]: LetterIndicator;
}

export type LetterIndicator = "notPresent" | "present" | "correct";

export interface RoundInfo {
  roundNumber: number;
  roundLengthSeconds: number;
  roundEndsUtc: Date;
}

export type RoundStateTypes = "Playing" | "PlayerSubmitted" | "Summary" | "Points";
export interface RoundState {
  value: RoundStateTypes;
}

export interface Points {
  roundPoints: PlayerPoints;
  totalPoints: PlayerPoints;
}

interface PlayerPoints {
  player: Player;
  points: number;
}

export interface GameserviceState {
  roundInfo: RoundInfo | undefined;
  roundState: RoundState | undefined;
  points: Points | undefined;
  displayNotification: string;
  game: Game | undefined;
}

export interface GameServiceAction {
  type: "ROUNDINFO" | "ROUNDSTATE" | "POINTS" | "DISPLAY_NOTIFICATION" | "GAME_UPDATE";
  payload: RoundInfo | RoundState | Points | string | Game;
}
