export interface Game {
	players: Player[];
	hostPlayer: Player;
	gameId: string;
	state: GameState;
}

export interface Player {
	name: string;
	id: string;
}

export interface Notificatino {
	show: boolean;
	msg: string;
}

export type GameState = 'Lobby' | 'Starting' | 'Started' | 'Ended';
