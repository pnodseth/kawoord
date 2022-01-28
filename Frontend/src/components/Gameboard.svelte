<script lang="ts">
	import type { Game, KeyIndicator, Player, RoundInfo, RoundState } from '../interface';
	import Keyboard from './Keyboard.svelte';
	import { createEventDispatcher, onDestroy } from 'svelte';
	import isBefore from 'date-fns/isBefore';
	import formatDistanceToNowStrict from 'date-fns/formatDistanceToNowStrict';

	const dispatch = createEventDispatcher();
	export let game: Game;
	export let player: Player;
	export let roundInfo: RoundInfo;
	export let roundState: RoundState;

	let keyIndicators: KeyIndicator = {};
	let letters = ['', '', '', '', ''];

	let countDown: string;
	let interval;

	$: {
		if (roundInfo) {
			interval = setInterval(() => {
				if (isBefore(new Date(), new Date(roundInfo.roundEndsUtc))) {
					countDown = `Ends in in: ${formatDistanceToNowStrict(new Date(roundInfo.roundEndsUtc))}`;
				} else {
					clearInterval(interval);
					countDown = 'Round has ended!';
				}
			}, 1000);
		} else {
			clearInterval(interval);
		}
	}

	function handleTap(e) {
		console.log(e.detail);
		if (e.detail === 'Enter') {
			dispatch('submitWord', 'feste');
		}
	}

	onDestroy(() => {
		clearInterval(interval);
	});
</script>

<section class="gameboard">
	<h2>Game has started!</h2>
	<h2>{player.name}</h2>
	<p>Round {game?.currentRoundNumber || 1}</p>
	<p>You have 30 seconds to guess the word...</p>
	<p>State: {roundState?.state.value}</p>
	<p>{countDown}</p>
	<div class="spacer h-8" />
	<div class="letters grid grid-cols-5 h-12  gap-3 px-12">
		{#each letters as letter}
			<div class="bg-gray-50 border-2 border-gray-600 flex justify-center items-center">
				Ye{letter}
			</div>
		{/each}
	</div>
	<div class="spacer h-8" />
	<Keyboard {keyIndicators} on:tap={handleTap} />
</section>
