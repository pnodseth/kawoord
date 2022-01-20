<script lang="ts">
	import type { Player } from '../interface';
	import { CACHEDPLAYER } from '../constants';
	import { createEventDispatcher } from 'svelte';

	export let player: Player = { id: '', name: '' };
	const dispatch = createEventDispatcher();
	let playerNameInput = '';
	let gameIdInput = '';

	function setPlayerName() {
		if (playerNameInput !== '') {
			player.name = playerNameInput;
		}
		localStorage.setItem(CACHEDPLAYER, JSON.stringify(player));
	}
</script>

<section>
	{#if !player.name}
		<div>
			<label for="playername">
				Player name
				<input type="text" id="playername" bind:value={playerNameInput} />
			</label>
		</div>
		<button on:click={setPlayerName}>Set player name</button>
	{:else}
		<h2>{player.name}</h2>
	{/if}

	<div>
		<button on:click={() => dispatch('create')}>Create game</button>
	</div>

	<div>
		<label for="game-id-input">
			Game Id:
			<input type="text" id="game-id-input" bind:value={gameIdInput} />
		</label>
		<button on:click={() => dispatch('join', gameIdInput)}>Join game</button>
	</div>
</section>
