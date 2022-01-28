<script lang="ts">
	import type { Player } from '../interface';
	import { CACHEDPLAYER } from '../constants';
	import { createEventDispatcher } from 'svelte';
	import Button from './input/Button.svelte';

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
		<Button on:click={setPlayerName} class="bg-teal-500">Set player name</Button>
	{:else}
		<h2>{player.name}</h2>
	{/if}

	<div>
		<Button on:click={() => dispatch('create')} class="bg-teal-500">Create game</Button>
	</div>

	<div>
		<label for="game-id-input">
			Game Id:
			<input type="text" id="game-id-input" bind:value={gameIdInput} />
		</label>
		<Button on:click={() => dispatch('join', gameIdInput)}>Join game</Button>
	</div>
</section>
