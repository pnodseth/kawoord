<script lang="ts">
	import Key from './Key.svelte';
	import { createEventDispatcher } from 'svelte';
	import type { KeyIndicator } from '../interface';

	export let keyIndicators: KeyIndicator = {};
	const dispatch = createEventDispatcher();

	function handleTap(e) {
		dispatch('tap', e);
	}
</script>

<div id="keyboard" class="w-full">
	<div class="row flex w-full mt-0 mx-auto mb-0.5">
		{#each ['q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'å'] as key}
			<Key {key} on:click={() => handleTap(key)} {keyIndicators}>{key}</Key>
		{/each}
	</div>
	<div class="row flex w-full mt-0 mx-auto mb-0.5">
		<div class="spacer half" />
		{#each ['a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'ø', 'æ'] as key}
			<Key {key} on:click={() => handleTap(key)} {keyIndicators}>{key}</Key>
		{/each}
		<div class="spacer half" />
	</div>
	<div class="row flex w-full mt-0 mx-auto mb-0.5">
		<Key key="Enter" on:click={() => handleTap('Enter')}>Enter</Key>
		{#each ['z', 'x', 'c', 'v', 'b', 'n', 'm'] as key}
			<Key {key} on:click={() => handleTap(key)} {keyIndicators}>{key}</Key>
		{/each}
		<Key key="Backspace" on:click={() => handleTap('Backspace')}>
			<svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24">
				<path
					fill="var(--color-tone-1)"
					d="M22 3H7c-.69 0-1.23.35-1.59.88L0 12l5.41 8.11c.36.53.9.89 1.59.89h15c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H7.07L2.4 12l4.66-7H22v14zm-11.59-2L14 13.41 17.59 17 19 15.59 15.41 12 19 8.41 17.59 7 14 10.59 10.41 7 9 8.41 12.59 12 9 15.59z"
				/>
			</svg>
		</Key>
	</div>
</div>

<style>
	#keyboard {
		user-select: none;
	}

	.half {
		flex: 0.5;
	}
</style>
