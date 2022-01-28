<script lang="ts">
	import type { KeyIndicator, LetterIndicator } from '../interface';

	export let key: string;
	export let keyIndicators: KeyIndicator = {};
	let indicator: LetterIndicator;

	$: {
		if (key && keyIndicators[key]) {
			indicator = keyIndicators[key];
		} else {
			indicator = null;
		}
	}
</script>

<button
	on:click
	class="font-bold border-0 p-0 mt-0 mr-1 mb-0 ml-0 flex-1 flex justify-center items-center cursor-pointer uppercase bg-gray-200"
	class:text-xs={key === 'Enter'}
	class:correct={indicator === 'correct'}
	class:not-present={indicator === 'notPresent'}><slot /></button
>

<style>
	button {
		height: 58px;
		border-radius: 4px;
		-webkit-tap-highlight-color: rgba(0, 0, 0, 0.3);
		color: #343434;
	}

	.correct {
		background-color: green;
		color: #fff;
	}

	.not-present {
		background-color: #bdc6d5;
		color: #e5e7eb;
	}
</style>
