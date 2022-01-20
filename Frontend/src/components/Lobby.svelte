<script lang="ts">
	import type { Game, Player } from '../interface';
	import { createEventDispatcher } from 'svelte';

	export let game: Game;
	export let player: Player = { id: null, name: '' };
	const dispatch = createEventDispatcher();
</script>

<section id="lobby">
	<h1>Lobby</h1>
	{#if game}
		<ul>
			{#each game.players as player}
				<li>{player.name}</li>
			{/each}
		</ul>
	{/if}
	{#if game.hostPlayer.id === player.id}
		<button on:click={() => dispatch('start')}>Start</button>
	{/if}
</section>
