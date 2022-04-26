export interface Game {
  players: Player[];
  hostPlayer: Player;
  gameId: string;
  state: GameViewMode;
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

export type GameViewMode = "Lobby" | "Starting" | "Started" | "Solved" | "EndedUnsolved";

export interface KeyIndicatorDict {
  [key: string]: LetterIndicator;
}

export type LetterIndicator = "notPresent" | "present" | "correct";

export interface Round {
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

export interface GameState {
  roundInfo: Round | undefined;
  roundState: RoundState | undefined;
  evaluations: Evaluations | undefined;
  displayNotification: string;
  game: Game | undefined;
  stats: GameStats | undefined;
}

export interface GameServiceAction {
  type: "ROUND_INFO" | "ROUND_STATE" | "POINTS" | "DISPLAY_NOTIFICATION" | "GAME_UPDATE" | "STATS";
  payload: Round | RoundState | Evaluations | string | Game | GameStats;
}

export interface GameStats {
  roundCompleted: number;
  winners: WinnerSubmission[];
}

interface WinnerSubmission {
  player: Player;
  roundCompleted: Date;
}

export interface UseGameNotificationsProps {
  onNotification: (msg: string) => void;
  onPlayerEvent: (type: PlayerEvent, player: Player) => void;
}

type PlayerEvent = "PLAYER_JOIN" | "PLAYER_LEAVE";

export interface RoundSummaryParams {
  gameState: GameState;
  player: Player;
}
