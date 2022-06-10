export interface Game {
  players: Player[];
  hostPlayer: Player;
  gameId: string;
  gameStateEnum: GameStateEnum;
  startedTime: number;
  endedTime: number;
  currentRoundNumber: number;
  roundStateEnum: RoundState;
  rounds: Round[];
}

export interface Player {
  name: string;
  id: string;
}

export interface Notification {
  show: boolean;
  msg: string;
}

export type GameMode = "Lobby" | "Starting" | "Playing" | "Solved" | "EndedUnsolved";

export interface GameStateEnum {
  value: "Lobby" | "Starting" | "Started" | "Solved" | "EndedUnsolved";
}

export interface KeyIndicatorDict {
  [key: string]: LetterIndicator;
}

export type LetterIndicator = "notPresent" | "present" | "correct";

export interface Round {
  roundNumber: number;
  roundLengthSeconds: number;
  roundEndsUtc: Date;
}

export type RoundStateTypes = "NotStarted" | "Playing" | "PlayerSubmitted" | "Summary" | "Points";

export interface RoundState {
  value: RoundStateTypes;
}

export interface Evaluations {
  roundEvaluations: WordEvaluation[];
  totalEvaluations: WordEvaluation[];
  viewLengthSeconds: number;
}

export interface WordEvaluation {
  player: Player;
  evaluation: LetterEvaluation[];
  isCorrectWord: boolean;
  submittedDateTime: Date;
  roundNumber: number;
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
  evaluations: WordEvaluation[] | undefined;
  displayNotification: string;
  game: Game | undefined;
}

export interface GameServiceAction {
  type: "ROUND_INFO" | "ROUND_STATE" | "POINTS" | "DISPLAY_NOTIFICATION" | "GAME_UPDATE";
  payload: Round | RoundState | WordEvaluation[] | string | Game;
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
