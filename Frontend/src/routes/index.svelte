<script lang="ts">
	import { GameService } from '../GameService';
	import Lobby from '../components/Lobby.svelte';
	import type { Game, Notificatino, Player } from '../interface';
	import { nanoid } from 'nanoid';
	import { onMount } from 'svelte';
	import { CACHEDPLAYER } from '../constants';
	import NoGame from '../components/NoGame.svelte';

	let player: Player = {
		id: '',
		name: ''
	};

	onMount(() => {
		let cachedPlayer = localStorage.getItem(CACHEDPLAYER);
		if (!cachedPlayer) {
			player = {
				name: '',
				id: nanoid()
			};

			localStorage.setItem(CACHEDPLAYER, JSON.stringify(player));
		} else {
			player = JSON.parse(cachedPlayer);
		}
	});

	const gameService = new GameService();

	let game: Game;
	let notification: Notificatino = { show: false, msg: '' };

	gameService._game.subscribe((g) => (game = g));
	gameService.showNotification.subscribe((v) => (notification = v));

	async function handleCreate() {
		await gameService.createGame(player);
	}

	async function handleJoin(e) {
		const gameId = e.detail;
		try {
			await gameService.joinGame(gameId, player);
		} catch (err) {
			console.log('Failed to join game: ', err);
		}
	}
</script>

{#if notification.show}
	<div class="toaster">{notification.msg}</div>
{/if}
{#if !game}
	<NoGame on:create={handleCreate} on:join={handleJoin} {player} />
{:else if game && game.state === 'Lobby'}
	<Lobby {game} {player} />
{/if}

<style>
	.toaster {
		position: absolute;
		top: 400px;
		left: 400px;
		width: 200px;
		height: 200px;
		background: tomato;
	}
</style>
