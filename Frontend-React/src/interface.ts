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

export type GameState = "Lobby" | "Starting" | "Started" | "Solved" | "EndedUnsolved";

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

export interface Evaluations {
  roundEvaluations: RoundEvaluation[];
  totalEvaluations: RoundEvaluation[];
  viewLengthSeconds: number;
}

export interface RoundEvaluation {
  player: Player;
  evaluation: LetterEvaluation[];
  isCorrectWord: boolean;
  submittedDateTime: Date;
}

export interface LetterEvaluation {
  letter: string;
  wordIndex: number;
  type: Type;
  round: number;
}

export interface Type {
  value: LetterValue;
}

type LetterValue = "Wrong" | "WrongPlacement" | "Correct";

export interface GameserviceState {
  roundInfo: RoundInfo | undefined;
  roundState: RoundState | undefined;
  evaluations: Evaluations | undefined;
  displayNotification: string;
  game: Game | undefined;
  stats: GameStats | undefined;
}

export interface GameServiceAction {
  type: "ROUND_INFO" | "ROUND_STATE" | "POINTS" | "DISPLAY_NOTIFICATION" | "GAME_UPDATE" | "STATS";
  payload: RoundInfo | RoundState | Evaluations | string | Game | GameStats;
}

export interface GameStats {
  roundCompleted: number;
  winners: WinnerSubmission[];
}

interface WinnerSubmission {
  player: Player;
  roundCompleted: Date;
}
