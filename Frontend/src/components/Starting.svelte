<script lang="ts">
	import formatDistanceToNowStrict from 'date-fns/formatDistanceToNowStrict';
	import isBefore from 'date-fns/isBefore';
	import type { Game, Player } from '../interface';
	import { onDestroy, onMount } from 'svelte';

	export let game: Game;
	export let player: Player;
	let startingIn = '';
	// eslint-disable-next-line no-undef
	let interval: NodeJS.Timer;
	onMount(() => {
		interval = setInterval(() => {
			if (isBefore(new Date(), new Date(game.startedTime))) {
				startingIn = `Starting in: ${formatDistanceToNowStrict(new Date(game.startedTime))}`;
			} else {
				clearInterval(interval);
				startingIn = 'Game is starting!';
			}
		}, 1000);
	});

	onDestroy(() => {
		clearInterval(interval);
	});
</script>

<section>
	<p>Player: {player.name}</p>
	<h2>Starting</h2>

	<p>{startingIn}</p>
	<p>{game.startedTime}</p>
</section>
