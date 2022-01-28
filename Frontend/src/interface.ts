export interface Game {
	players: Player[];
	hostPlayer: Player;
	gameId: string;
	state: GameState;
	startedTime: number;
	endedTime: number;
	currentRoundNumber: number;
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

export interface KeyIndicator {
	[key: string]: LetterIndicator;
}

export type LetterIndicator = 'notPresent' | 'present' | 'correct';

export interface RoundInfo {
	round: number;
	roundEndsUtc: number;
}

export type RoundStateTypes = 'Playing' | 'Summary' | 'Points';
export interface RoundState {
	state: {
		value: RoundStateTypes;
	};
}
