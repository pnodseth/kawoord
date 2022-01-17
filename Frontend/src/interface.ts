export type GameState = 'noGame' | 'lobby' | 'started' | 'ended';

export interface Game {
	players: Player[];
	hostPlayer: Player;
	gameId: string;
}

interface Player {
	name: string;
}
