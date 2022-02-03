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
	roundNumber: number;
	roundLengthSeconds: number;
	roundEndsUtc: Date;
}

export type RoundStateTypes = 'Playing' | 'PlayerSubmitted' | 'Summary' | 'Points';
export interface RoundState {
	state: {
		value: RoundStateTypes;
	};
	data?: RoundStateData;
}

interface RoundStateData {
	roundPoints: PlayerPoints;
	totalPoints: PlayerPoints;
	timeUntilNextRoundSeconds: number;
}

interface PlayerPoints {
	player: Player;
	points: number;
}
