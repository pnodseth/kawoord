import { HubConnectionBuilder } from '@microsoft/signalr';
import type { Game, GameState } from './interface';

export class GameService {
	private baseUrl = 'http://localhost:3000';
	private connection = new HubConnectionBuilder()
		.withUrl('https://localhost:7119/gameplay')
		.build();

	constructor() {
		this.registerConnectionEvents();
	}

	async createGame(playerName) {
		// Call creategame api endpoint which returns game id
		const response = await fetch(`http://localhost:5172/game/create?playername=${playerName}`, {
			method: 'POST'
		});
		if (response.ok) {
			const game: any = await response.json();
			console.log(`got game info: ${JSON.stringify(game)}`);

			// join socket with gameId
			await this.connect();
			await this.connection.invoke('ConnectToGame', game.gameId, playerName);
			console.log('testing');
			return game;
		} else {
			console.log(`Failed to fetch: ${response.status}`);
		}

		return;
		//connection.invoke('CreateGame', gameIdInput, playernameInput);
	}

	async connect() {
		try {
			await this.connection.start();
			return;
			console.log('connected');
		} catch (err) {
			console.error(err.toString());
		}
	}

	private registerConnectionEvents() {
		this.connection.on('game-player-join', (player) => {
			console.log(`${player} joined!`);
		});
		return;
	}

	async joinGame(gameId: string, playername: string): Promise<Game> {
		const response = await fetch(
			`http://localhost:5172/game/add?playername=${playername}&gameId=${gameId}`,
			{
				method: 'POST'
			}
		);

		if (response.ok) {
			const game: Game = await response.json();
			console.log(`got game info: ${JSON.stringify(game)}`);
			// join socket with gameId
			await this.connect();
			await this.connection.invoke('ConnectToGame', game.gameId, playername);
			return game;
		} else {
			console.log(`Failed to fetch: ${response.status}`);
		}
	}
}
