import { HubConnectionBuilder } from '@microsoft/signalr';
import type { Game, GameState, Notificatino, Player } from './interface';
import { writable } from 'svelte/store';

export class GameService {
	private baseUrl = 'http://localhost:5172';
	_game = writable<Game>();
	private connection = new HubConnectionBuilder()
		.withUrl('https://localhost:7119/gameplay')
		.build();
	notificationMsg = '';
	showNotification = writable<Notificatino>({ show: false, msg: '' });

	constructor() {
		this.registerConnectionEvents();
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
		this.connection.on('game-player-join', (player: string, game: Game) => {
			if (game) {
				this._game.set(game);
				this.showMessage(`Player joined: ${player}`);
			}
		});

		// GAME CHANGES STATE
		this.connection.on('gamestate', (newState: GameState) => {
			this._game.update((e) => {
				return { ...e, state: newState };
			});
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
}
