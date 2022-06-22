import { HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { Game, Player, StateType, StateTypeData } from "../../interface";

export interface CallbackProps {
  onNotification?: (msg: string, durationSec?: number) => void;
  onGameUpdate?: (game: Game) => void;
  onClearGame?: () => void;
  onStateReceived?: (stateType: StateType, data: StateTypeData) => void;
}

export class GameService {
  private baseUrl = import.meta.env.DEV ? "http://localhost:5172" : "https://kawoord.com";
  private _player: Player | undefined;
  private connection = new HubConnectionBuilder()
    .withUrl(`${this.baseUrl}/gameplay`)
    .withAutomaticReconnect()
    .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
    .build();

  /*Callback handlers*/
  onNotification: (msg: string, durationSec?: number) => void = () =>
    console.log("onNotification not assigned a callback");
  onGameUpdate: (game: Game) => void = () => console.log("onGameData callback Not implemented");
  onClearGame: () => void = () => console.log("onClearGame callback not implemented");
  onStateReceived: (stateType: StateType, data: StateTypeData) => void = () =>
    console.log("onStateReceived not implemented");

  constructor(player?: Player) {
    this.registerConnectionEvents();
    if (player) {
      this._player = player;
    }
  }

  registerCallbacks(callbacks: CallbackProps): void {
    if (callbacks.onNotification) {
      this.onNotification = callbacks.onNotification;
    }
    if (callbacks.onGameUpdate) {
      this.onGameUpdate = callbacks.onGameUpdate;
    }
    if (callbacks.onClearGame) {
      this.onClearGame = callbacks.onClearGame;
    }
    if (callbacks.onStateReceived) {
      this.onStateReceived = callbacks.onStateReceived;
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
      console.log("connection status: ", this.connection.state);
      return;
    } catch (err) {
      console.log("SignalR Unable to ocnenct. Connection status: ", this.connection.state);
      console.error(err);
    }
  }

  private registerConnectionEvents() {
    this.connection.on("game-update", (updatedGame: Game) => {
      this.onGameUpdate(updatedGame);
    });

    this.connection.on("state", (stateType: StateType, data: StateTypeData) => {
      console.log("received solution: ", data);
      this.onStateReceived(stateType, data);
    });

    this.connection.onreconnecting((error) => {
      console.assert(this.connection.state === HubConnectionState.Reconnecting);
      console.log("SignalR reconnecting....", error);
      //todo trigger ui notification here
    });

    this.connection.onreconnected(() => {
      console.assert(this.connection.state === HubConnectionState.Connected);
      console.log("SignalR reconnected. ");
      //todo trigger ui here
    });

    this.connection.onclose(() => {
      //Todo:  SignalR was unable to reconnect.Connection has been permanently lost. Inform users to refresh page.
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

      this.onGameUpdate(game);

      // join socket with gameId
      await this.connect();
      await this.connection.invoke("ConnectToGame", gameId, this._player?.name, this._player?.id);
    } else {
      console.log(`Failed to fetch: ${response.status}`);
      throw new Error(await response.text());
    }
  }

  async start(gameId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/game/start?gameId=${gameId}&playerId=${this._player?.id}`, {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error(await response.json());
    }
  }

  async submitWord(word: string, gameId: string): Promise<void> {
    if (!word) {
      throw new Error("Word is null or empty");
    }
    const response = await fetch(
      `${this.baseUrl}/game/submitword?gameId=${gameId}&playerId=${this._player?.id}&word=${word}`,
      {
        method: "POST",
      }
    );

    if (!response.ok) {
      throw new Error("Not a valid word");
    }
  }

  async clearGame() {
    this.onClearGame();
    await this.connection.stop();
    console.log("connection status: ", this.connection.state);
  }
}
