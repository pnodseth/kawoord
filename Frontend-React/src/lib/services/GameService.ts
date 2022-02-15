import { HubConnectionBuilder } from "@microsoft/signalr";
import { Game, GameState, Player, Evaluations, RoundInfo, RoundState } from "../../interface";

export interface CallbackProps {
  onPointsUpdate: (points: Evaluations) => void;
  onRoundInfo: (roundInfo: RoundInfo) => void;
  onRoundStateUpdate: (data: RoundState) => void;
  onNotification: (msg: string, durationSec?: number) => void;
  onPlayerJoinCallback: (player: Player, updatedGame: Game) => void;
  onGameStateUpdateCallback: (newState: GameState, updatedGame: Game) => void;
  onGameUpdate: (game: Game) => void;
}

export class GameService {
  private baseUrl = "http://localhost:5172";
  private _gameId: string | undefined;
  private _player: Player | undefined;
  private connection = new HubConnectionBuilder().withUrl("http://localhost:5172/gameplay").build();

  /*Callback handlers*/
  onPlayerJoinCallback: (player: Player, updatedGame: Game) => void = () =>
    console.log("OnPlayerJoin not assigned a callback");
  onGameStateUpdateCallback: (newState: GameState, game: Game) => void = () =>
    console.log("OnGameStateUpdate not assigned a callback");
  onRoundInfo: (roundInfo: RoundInfo) => void = () => console.log("OnGameStateUpdate not assigned a callback");
  onRoundStateUpdate: (data: RoundState) => void = () => console.log("onRoundStateUpdate not assigned a callback");
  onPointsUpdate: (data: Evaluations) => void = () => console.log("onPointsUpdate not assigned a callback");
  onNotification: (msg: string, durationSec?: number) => void = () =>
    console.log("onNotification not assigned a callback");
  onGameUpdate: (game: Game) => void = () => console.log("onGameData callback Not implemented");

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
    // PLAYER JOINED
    this.connection.on("game-player-join", (player: Player, game: Game) => {
      if (game) {
        console.log("game: ", game);
        this.onNotification(`Player joined: ${player.name}`);
        this.onPlayerJoinCallback(player, game);
      }
    });

    // GAME CHANGES STATE
    this.connection.on("gamestate", (newState: GameState, updatedGame: Game) => {
      this.onGameStateUpdateCallback(newState, updatedGame);
    });

    this.connection.on("round-info", (roundInfo: RoundInfo) => {
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
