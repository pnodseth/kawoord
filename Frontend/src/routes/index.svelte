<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import { GameService } from '../GameService';
	import Lobby from '../components/Lobby.svelte';
	import type { Game, GameState } from '../interface';

	const connection = new HubConnectionBuilder().withUrl('https://localhost:7119/gameplay').build();
	const gameService = new GameService();

	let playernameInput = '';
	let gameIdInput = '';
	let gameState: GameState = 'noGame';
	let game: Game;

	async function handleCreate() {
		game = await gameService.createGame(playernameInput);
		gameState = 'lobby';
	}

	async function handleJoin() {
		game = await gameService.joinGame(gameIdInput, playernameInput);
		gameState = 'lobby';
	}
</script>

{#if gameState === 'noGame'}
	<main>
		<div>
			<label for="playername">
				Player name
				<input type="text" id="playername" bind:value={playernameInput} />
			</label>
		</div>
		<label for="game-id-input">
			Game Id:
			<input type="text" id="game-id-input" bind:value={gameIdInput} />
		</label>
		<div>
			<button on:click={handleCreate}>Create game</button>
		</div>

		<div>
			<button on:click={handleJoin}>Join game</button>
		</div>
	</main>
{:else if gameState === 'lobby'}
	<Lobby />
{/if}
