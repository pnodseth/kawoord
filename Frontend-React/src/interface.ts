export interface Game {
  players: Player[];
  hostPlayer: Player;
  gameId: string;
  gameViewEnum: number;
  startedTime: number;
  endedTime: number;
  currentRoundNumber: number;
  maxPlayers: number;
  rounds: Round[];
  roundSubmissions: RoundSubmission[];
  playerLetterHints: PlayerLetterHints[];
}

export interface PlayerLetterHints {
  correct: LetterEvaluation[];
  wrongPosition: LetterEvaluation[];
  wrong: LetterEvaluation[];
  player: Player;
  roundNumber: number;
}

export interface Player {
  name: string;
  id: string;
}

export interface KeyIndicatorDict {
  [key: string]: LetterIndicator;
}

export type LetterIndicator = "notPresent" | "present" | "correct";

export interface Round {
  roundNumber: number;
  roundLengthSeconds: number;
  roundEndsUtc: Date;
  preRoundEndsUtc: Date;
  roundViewEnum: number;
}

export interface RoundView {
  value: "NotStarted" | "Playing" | "PlayerSubmitted" | "Summary" | "Points";
}

export interface RoundSubmission {
  player: Player;
  letterEvaluations: LetterEvaluation[];
  isCorrectWord: boolean;
  submittedDateTime: Date;
  roundNumber: number;
}

export interface LetterEvaluation {
  letter: string;
  wordIndex: number;
  letterValueType: LetterValueType;
  round: number;
}

export interface LetterValueType {
  value: "Wrong" | "WrongPlacement" | "Correct";
}

export interface GameState {
  displayNotification: string;
  game: Game | undefined;
  solution?: string;
}

export interface GameServiceAction {
  type: "DISPLAY_NOTIFICATION" | "GAME_UPDATE" | "ClEAR_GAME" | "SOLUTION";
  payload: Round | RoundView | RoundSubmission[] | Game | string | undefined;
}

export interface RoundSummaryParams {
  gameState: GameState;
  player: Player;
}

export type StateType = "solution" | "something";

export type StateTypeData = string;

export interface PlayerEventData {
  type: number;
  playerName: string;
  id: string;
}
