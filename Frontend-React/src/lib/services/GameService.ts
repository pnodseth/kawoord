import { HubConnectionBuilder } from "@microsoft/signalr";
import { Evaluations, Game, GameViewMode, GameStats, Player, Round, RoundState } from "../../interface";

export interface CallbackProps {
  onPlayerEventCallback?: (player: Player, type: PlayerEvent) => void;
  onStats?: (gameStats: GameStats) => void;
  onPointsUpdate?: (points: Evaluations) => void;
  onRoundInfo?: (roundInfo: Round) => void;
  onRoundStateUpdate?: (data: RoundState) => void;
  onNotification?: (msg: string, durationSec?: number) => void;
  onPlayerJoinCallback?: (player: Player, updatedGame: Game) => void;
  onGameStateUpdateCallback?: (newState: GameViewMode, updatedGame: Game) => void;
  onGameUpdate?: (game: Game) => void;
}

type PlayerEvent = "PLAYER_JOIN" | "PLAYER_DISCONNECT";

export class GameService {
  private baseUrl = "http://localhost:5172";
  private _gameId: string | undefined;
  private _player: Player | undefined;
  private connection = new HubConnectionBuilder().withUrl("http://localhost:5172/gameplay").build();

  /*Callback handlers*/
  onPlayerJoinCallback: (player: Player, updatedGame: Game) => void = () =>
    console.log("OnPlayerJoin not assigned a callback");
  onGameStateUpdateCallback: (newState: GameViewMode, game: Game) => void = () =>
    console.log("OnGameStateUpdate not assigned a callback");
  onRoundInfo: (roundInfo: Round) => void = () => console.log("OnGameStateUpdate not assigned a callback");
  onRoundStateUpdate: (data: RoundState) => void = () => console.log("onRoundStateUpdate not assigned a callback");
  onPointsUpdate: (data: Evaluations) => void = () => console.log("onPointsUpdate not assigned a callback");
  onNotification: (msg: string, durationSec?: number) => void = () =>
    console.log("onNotification not assigned a callback");
  onGameUpdate: (game: Game) => void = () => console.log("onGameData callback Not implemented");
  onStats: (gameStats: GameStats) => void = () => console.log("onGameStats callback not assigned");
  onPlayerEvent: (player: Player, type: PlayerEvent) => void = () => console.log("onPlayerEvent callback not assigned");

  constructor(player?: Player) {
    this.registerConnectionEvents();
    if (player) {
      this._player = player;
    }
  }

  registerCallbacks(callbacks: CallbackProps): void {
    if (callbacks.onRoundInfo) {
      this.onRoundInfo = callbacks.onRoundInfo;
    }
    if (callbacks.onRoundStateUpdate) {
      this.onRoundStateUpdate = callbacks.onRoundStateUpdate;
    }
    if (callbacks.onPointsUpdate) {
      this.onPointsUpdate = callbacks.onPointsUpdate;
    }
    if (callbacks.onNotification) {
      this.onNotification = callbacks.onNotification;
    }
    if (callbacks.onGameUpdate) {
      this.onGameUpdate = callbacks.onGameUpdate;
    }
    if (callbacks.onPlayerJoinCallback) {
      this.onPlayerJoinCallback = callbacks.onPlayerJoinCallback;
    }
    if (callbacks.onGameStateUpdateCallback) {
      this.onGameStateUpdateCallback = callbacks.onGameStateUpdateCallback;
    }
    if (callbacks.onStats) {
      this.onStats = callbacks.onStats;
    }
    if (callbacks.onPlayerEventCallback) {
      this.onPlayerEvent = callbacks.onPlayerEventCallback;
    }
  }

  async createGame(player: Player | undefined): Promise<void> {
    console.log("hei");
    this._player = player;
    // Call create game api endpoint which returns game id
    const response = await fetch(
      `${this.baseUrl}/game/create?playerName=${this._player?.name}&playerId=${this._player?.id}`,
      {
        method: "POST",
      }
    );
    if (response.ok) {
      const game: Game = await response.json();
      this._gameId = game.gameId;

      this.onGameUpdate(game);
      // join socket with gameId
      await this.connect();
      await this.connection.invoke("ConnectToGame", game.gameId, this._player?.name, this._player?.id);
    } else {
      console.log(`Failed to fetch: ${response.status}`);
    }

    //connection.invoke('CreateGame', gameIdInput, playernameInput);
  }

  async connect(): Promise<void> {
    try {
      await this.connection.start();
      console.log("connected");
      return;
    } catch (err) {
      console.error(err);
    }
  }

  private registerConnectionEvents() {
    this.connection.on("player-event", (player: Player, type: PlayerEvent) => {
      this.onPlayerEvent(player, type);
    });

    this.connection.on("game-update", (updatedGame: Game) => {
      this.onGameUpdate(updatedGame);
    });

    // GAME CHANGES STATE
    this.connection.on("gamestate", (newState: GameViewMode, updatedGame: Game) => {
      this.onGameStateUpdateCallback(newState, updatedGame);
    });

    this.connection.on("round-info", (roundInfo: Round) => {
      if (typeof this.onRoundInfo === "function") {
        this.onRoundInfo(roundInfo);
      }
    });

    this.connection.on("round-state", (data: RoundState) => {
      if (typeof this.onRoundStateUpdate === "function") {
        this.onRoundStateUpdate(data);
      }
    });

    this.connection.on("points", (data: Evaluations) => {
      if (typeof this.onPointsUpdate === "function") {
        this.onPointsUpdate(data);
      }
    });

    this.connection.on("word-submitted", (playerName: string) => {
      this.onNotification(`${playerName} just submitted a word!`);
    });

    this.connection.on("stats", (stats: GameStats) => {
      this.onStats(stats);
    });
  }

  async joinGame(player: Player, gameId: string): Promise<void> {
    this._player = player;
    const response = await fetch(
      `${this.baseUrl}/game/join?playerName=${this._player?.name}&playerId=${this._player?.id}&gameId=${gameId}`,
      {
        method: "POST",
      }
    );

    if (response.ok) {
      const game: Game = await response.json();
      this._gameId = game.gameId;

      this.onGameUpdate(game);

      // join socket with gameId
      await this.connect();
      await this.connection.invoke("ConnectToGame", this._gameId, this._player?.name, this._player?.id);
    } else {
      console.log(`Failed to fetch: ${response.status}`);
      throw new Error(await response.text());
    }
  }

  async start(): Promise<void> {
    if (!this._gameId) {
      throw new Error("No game, cant start.");
    }
    const response = await fetch(`${this.baseUrl}/game/start?gameId=${this._gameId}&playerId=${this._player?.id}`, {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error(await response.json());
    }
  }

  async submitWord(word: string): Promise<void> {
    if (!this._gameId) {
      throw new Error("No game, cant submit word.");
    }
    if (!word) {
      throw new Error("Word is null or empty");
    }
    const response = await fetch(
      `${this.baseUrl}/game/submitword?gameId=${this._gameId}&playerId=${this._player?.id}&word=${word}`,
      {
        method: "POST",
      }
    );

    if (!response.ok) {
      throw new Error(await response.json());
    }
  }

  setPlayer(newplayer: Player) {
    this._player = newplayer;
  }
}
