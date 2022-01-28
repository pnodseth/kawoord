<script lang="ts">
	import { GameService } from '../GameService';
	import Lobby from '../components/Lobby.svelte';
	import type { Game, Notificatino, Player, RoundInfo, RoundState } from '../interface';
	import { nanoid } from 'nanoid';
	import { onMount } from 'svelte';
	import { CACHEDPLAYER } from '../constants';
	import NoGame from '../components/NoGame.svelte';
	import Starting from '../components/Starting.svelte';
	import Gameboard from '../components/Gameboard.svelte';

	let player: Player = {
		id: '',
		name: ''
	};

	let gameService: GameService;

	let game: Game;
	let roundInfo: RoundInfo;
	let roundState: RoundState;
	let notification: Notificatino = { show: false, msg: '' };

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

	async function handleStart() {
		try {
			await gameService.start();
		} catch (err) {
			console.log(err);
		}
	}

	// Set player id or get from localstorage
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

		gameService = new GameService(player);
		//todo: rewrite to use onGameUpdate callbacks instead of svelte store
		gameService._game.subscribe((g) => (game = g));
		gameService.registerCallbacks({
			onRoundInfo: (info) => {
				console.log(`info: ${JSON.stringify(info)}`);
				roundInfo = info;
			},
			onRoundStateUpdate: (data: RoundState) => {
				console.log(`Got round state update: ${JSON.stringify(data)}`);
				roundState = data;
			}
		});
		gameService.showNotification.subscribe((v) => (notification = v));
	});
	let debug = false;
</script>

<section class="max-w-md m-auto m-auto">
	{#if notification.show}
		<div class="toaster">{notification.msg}</div>
	{/if}

	<div class="spacer h-8" />
	{#if debug}
		<Gameboard {game} {player} {roundInfo} {roundState} />
	{:else if !game}
		<NoGame on:create={handleCreate} on:join={handleJoin} {player} />
	{:else if game && game.state === 'Lobby'}
		<Lobby {game} {player} on:start={handleStart} />
	{:else if game && game.state === 'Starting'}
		<Starting {game} {player} />
	{:else if game && game.state === 'Started'}<Gameboard
			{game}
			{player}
			{roundInfo}
			{roundState}
		/>{/if}
</section>

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
