import { HubConnectionBuilder } from '@microsoft/signalr';
import type { Game, GameState, Player, Points, RoundInfo, RoundState } from './interface';
import { get, writable } from 'svelte/store';

interface CallbackProps {
	onPointsUpdate: (points: Points) => void;
	onRoundInfo: (roundInfo: RoundInfo) => void;
	onRoundStateUpdate: (data: unknown) => void;
	onNotification: (msg: string, durationSec?: number) => void;
}

export class GameService {
	private baseUrl = 'http://localhost:5172';
	_game = writable<Game>();
	private _player: Player;
	private connection = new HubConnectionBuilder().withUrl('http://localhost:5172/gameplay').build();

	/*Callback handlers*/
	onPlayerJoinCallback: (player: Player, updatedGame: Game) => void = () =>
		console.log('OnPlayerJoin not assigned a callback');
	onGameStateUpdateCallback: (newState: GameState, game: Game) => void = () =>
		console.log('OnGameStateUpdate not assigned a callback');
	onRoundInfo: (roundInfo: RoundInfo) => void = () =>
		console.log('OnGameStateUpdate not assigned a callback');
	onRoundStateUpdate: (data: RoundState) => void = () =>
		console.log('onRoundStateUpdate not assigned a callback');
	onPointsUpdate: (data: Points) => void = () =>
		console.log('onPointsUpdate not assigned a callback');
	onNotification: (msg: string, durationSec?: number) => void = () =>
		console.log('onNotification not assigned a callback');

	constructor(player: Player) {
		this.registerConnectionEvents();
		this._player = player;
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
	}

	async createGame(player: Player): Promise<void> {
		// Call create game api endpoint which returns game id
		const response = await fetch(
			`${this.baseUrl}/game/create?playerName=${player.name}&playerId=${player.id}`,
			{
				method: 'POST'
			}
		);
		if (response.ok) {
			const game: Game = await response.json();
			if (game) {
				this._game.set(game);
			}

			// join socket with gameId
			await this.connect();
			await this.connection.invoke('ConnectToGame', game.gameId, player.name, player.id);
		} else {
			console.log(`Failed to fetch: ${response.status}`);
		}

		return;
		//connection.invoke('CreateGame', gameIdInput, playernameInput);
	}

	async connect(): Promise<void> {
		try {
			await this.connection.start();
			console.log('connected');
			return;
		} catch (err) {
			console.error(err.toString());
		}
	}

	private registerConnectionEvents() {
		// PLAYER JOINED
		this.connection.on('game-player-join', (player: Player, game: Game) => {
			if (game) {
				this._game.set(game);
				console.log('game: ', game);
				this.onNotification(`Player joined: ${player.name}`);
				this.onPlayerJoinCallback(player, game);
			}
		});

		// GAME CHANGES STATE
		this.connection.on('gamestate', (newState: GameState, updatedGame: Game) => {
			this._game.set(updatedGame);

			this.onGameStateUpdateCallback(newState, updatedGame);
		});

		this.connection.on('round-info', (roundInfo: RoundInfo) => {
			if (typeof this.onRoundInfo === 'function') {
				this.onRoundInfo(roundInfo);
			}
		});

		this.connection.on('round-state', (data: RoundState) => {
			if (typeof this.onRoundStateUpdate === 'function') {
				this.onRoundStateUpdate(data);
			}
		});

		this.connection.on('points', (data: Points) => {
			if (typeof this.onPointsUpdate === 'function') {
				this.onPointsUpdate(data);
			}
		});

		this.connection.on('word-submitted', (playerName: string) => {
			this.onNotification(`${playerName} just submitted a word!`);
		});
	}

	async joinGame(gameId: string, player: Player): Promise<void> {
		const response = await fetch(
			`${this.baseUrl}/game/join?playerName=${player.name}&playerId=${player.id}&gameId=${gameId}`,
			{
				method: 'POST'
			}
		);

		if (response.ok) {
			const game: Game = await response.json();
			this._game.set(game);

			// join socket with gameId
			await this.connect();
			await this.connection.invoke('ConnectToGame', gameId, player.name, player.id);
		} else {
			console.log(`Failed to fetch: ${response.status}`);
			throw new Error(await response.text());
		}
	}

	async start(): Promise<void> {
		const game = get(this._game);
		if (!game) {
			throw new Error('No game, cant start.');
		}
		const response = await fetch(
			`${this.baseUrl}/game/start?gameId=${game.gameId}&playerId=${this._player.id}`,
			{
				method: 'POST'
			}
		);

		if (!response.ok) {
			throw new Error(await response.json());
		}
	}

	async submitWord(word: string): Promise<void> {
		const game = get(this._game);
		if (!game) {
			throw new Error('No game, cant submit word.');
		}
		if (!word) {
			throw new Error('Word is null or empty');
		}
		const response = await fetch(
			`${this.baseUrl}/game/submitword?gameId=${game.gameId}&playerId=${this._player.id}&word=${word}`,
			{
				method: 'POST'
			}
		);

		if (!response.ok) {
			throw new Error(await response.json());
		}
	}
}
