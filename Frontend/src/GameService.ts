import { HubConnectionBuilder } from '@microsoft/signalr';
import type { Game, GameState, Notificatino, Player } from './interface';
import { get, writable } from 'svelte/store';

export class GameService {
	private baseUrl = 'http://localhost:5172';
	_game = writable<Game>();
	private _player: Player;
	private connection = new HubConnectionBuilder().withUrl('http://localhost:5172/gameplay').build();
	notificationMsg = '';
	showNotification = writable<Notificatino>({ show: false, msg: '' });

	/*Callback handlers*/
	onPlayerJoinCallback: (player: Player, updatedGame: Game) => void = () =>
		console.log('OnPlayerJoin not assigned a callback');
	onGameStateUpdateCallback: (newState: GameState, game: Game) => void = () =>
		console.log('OnGameStateUpdate not assigned a callback');

	constructor(player: Player) {
		this.registerConnectionEvents();
		this._player = player;
	}

	showMessage(msg: string, duration = 4000): void {
		this.notificationMsg = msg;
		this.showNotification.set({ show: true, msg });

		setTimeout(() => {
			this.showNotification.set({ show: false, msg: '' });
		}, duration);
	}

	async createGame(player: Player): Promise<void> {
		// Call creategame api endpoint which returns game id
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
			await this.connection.invoke('ConnectToGame', game.gameId, player.name);
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
				this.showMessage(`Player joined: ${player.name}`);

				this.onPlayerJoinCallback(player, game);
			}
		});

		// GAME CHANGES STATE
		this.connection.on('gamestate', (newState: GameState, updatedGame: Game) => {
			this._game.set(updatedGame);

			this.onGameStateUpdateCallback(newState, updatedGame);
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
			await this.connection.invoke('ConnectToGame', gameId, player.name);
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
}
