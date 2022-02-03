<script lang="ts">
	import type { Game, Player, RoundInfo, RoundState } from '../interface';

	export let roundState: RoundState;
	export let player: Player;
	export let game: Game;
	export let roundInfo: RoundInfo;
</script>

{#if roundState?.state.value === 'Summary'}
	<section class="summary">
		<h2>Summary</h2>
		{#each roundState?.data?.roundPoints as points}
			<p>{points?.player?.name}: {points?.points}</p>
		{/each}
	</section>
{:else}
	<section class="points">
		<h2>Points!</h2>
		<p>After round {roundInfo?.roundNumberq}, this is the score:</p>
		<ul>
			<li>Pål.....48 points</li>
			<li>Peter.....38 points</li>
			<li>Liam.....16 points</li>
		</ul>
	</section>
{/if}
