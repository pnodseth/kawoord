import { HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { Game, Player, PlayerEventData, StateType, StateTypeData } from "../../interface";

export interface CallbackProps {
  onNotification?: (msg: string, durationSec?: number) => void;
  onGameUpdate?: (game: Game) => void;
  onClearGame?: () => void;
  onStateReceived?: (stateType: StateType, data: StateTypeData) => void;
  onPlayerEvent?: (data: PlayerEventData) => void;
}

export class GameService {
  private baseUrl = import.meta.env.DEV ? "http://localhost:5172" : "https://gameservice.kawoord.com";
  private connection = new HubConnectionBuilder()
    .withUrl(`${this.baseUrl}/gameplay`, { withCredentials: false })
    .withAutomaticReconnect()
    .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning)
    .build();

  constructor() {
    this.registerConnectionEvents();
  }

  /*Callback handlers*/
  onNotification: (msg: string, durationSec?: number) => void = () =>
    console.log("onNotification not assigned a callback");

  onGameUpdate: (game: Game) => void = () => console.log("onGameData callback Not implemented");

  onClearGame: () => void = () => console.log("onClearGame callback not implemented");

  onStateReceived: (stateType: StateType, data: StateTypeData) => void = () =>
    console.log("onStateReceived not implemented");

  onPlayerEvent: (data: PlayerEventData) => void = () => {
    console.log("onPlayerEvent not implemented");
  };

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
    if (typeof callbacks.onPlayerEvent === "function") {
      //todo
      this.onPlayerEvent = callbacks.onPlayerEvent;
    }
  }

  async createGame(player: Player, isPublic = false): Promise<void> {
    // Call create game api endpoint which returns game id
    const response = await fetch(
      `${this.baseUrl}/game/create?playerName=${player.name}&playerId=${player.id}&isPublic=${isPublic}`,
      {
        method: "POST",
      }
    );
    if (response.ok) {
      const game: Game = await response.json();
      // join socket with gameId
      await this.connect();
      await this.connection.invoke("ConnectToGame", game.gameId, player.name, player.id);
      this.onGameUpdate(game);
    } else {
      //todo: Handle 405 error which happens e.g if backend is currently redeploying
      //.. and also other errors
      console.log(`Failed to fetch: ${response.status}`);
    }
  }

  async findPublicGame(player: Player): Promise<void> {
    await this.createGame(player, true);
  }

  async connect(): Promise<void> {
    try {
      await this.connection.start();
      return;
    } catch (err) {
      console.log("SignalR Unable to connect. Connection status: ", this.connection.state);
      console.error(err);
    }
  }

  async joinGame(player: Player, gameId: string): Promise<void> {
    const response = await fetch(
      `${this.baseUrl}/game/join?playerName=${player.name}&playerId=${player.id}&gameId=${gameId}`,
      {
        method: "POST",
      }
    );

    if (response.ok) {
      const game: Game = await response.json();

      this.onGameUpdate(game);

      // join socket with gameId
      await this.connect();
      await this.connection.invoke("ConnectToGame", gameId, player.name, player.id);
    } else {
      console.log(`Failed to fetch: ${response.status}`);
      throw new Error(await response.text());
    }
  }

  async start(gameId: string, player: Player): Promise<void> {
    const response = await fetch(`${this.baseUrl}/game/start?gameId=${gameId}&playerId=${player.id}`, {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error(await response.json());
    }
  }

  async submitWord(word: string, gameId: string, player: Player): Promise<void> {
    if (!word) {
      throw new Error("Word is null or empty");
    }
    const response = await fetch(
      `${this.baseUrl}/game/submitword?gameId=${gameId}&playerId=${player.id}&word=${word}`,
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

  async GetRandomName() {
    try {
      const res = await fetch(`${this.baseUrl}/random-name`);
      return await res.json();
    } catch (e) {
      return "Ooopsy";
    }
  }

  private registerConnectionEvents() {
    this.connection.on("game-update", (updatedGame: Game) => {
      this.onGameUpdate(updatedGame);
    });

    this.connection.on("state", (stateType: StateType, data: StateTypeData) => {
      this.onStateReceived(stateType, data);
    });

    this.connection.on("playerEvent", (data: PlayerEventData) => {
      this.onPlayerEvent(data);
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
}

export const gameService = new GameService();
