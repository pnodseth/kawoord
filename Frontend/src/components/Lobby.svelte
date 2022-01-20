<script lang="ts">
	import type { Game, Player } from '../interface';

	export let game: Game;
	export let player: Player = { id: null, name: '' };
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
		<button>Start</button>
	{/if}
</section>
