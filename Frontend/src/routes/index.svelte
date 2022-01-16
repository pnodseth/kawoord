<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';

	const connection = new HubConnectionBuilder().withUrl('https://localhost:7119/gameplay').build();

	connection.on('ReceiveMessage', (user, message) => {
		console.log(`${user} says: ${message}`);
	});

	connection.on('game-created', (message) => {
		console.log(message);
	});

	connection
		.start()
		.then(function () {
			console.log('connected');
		})
		.catch(function (err) {
			return console.error(err.toString());
		});

	function createGame() {
		//todo
		// Call creategame api endpoint which returns game id
		// join socket with gameId
		console.log('creating game...');
		connection.invoke('CreateGame', gameIdInput, playernameInput);
	}

	function joinGame() {
		connection.invoke('JoinGroup', gameIdInput, playernameInput);
	}

	let playernameInput = '';
	let gameIdInput = '';
</script>

<div>
	<label for="playername">
		Player name
		<input type="text" id="playername" bind:value={playernameInput} />
	</label>
</div>
<label for="game-id-input">
	Game Id:
	<input type="text" id="game-id-input" bind:value={gameIdInput} />
</label>
<div>
	<button on:click={createGame}>Create game</button>
</div>

<div>
	<button on:click={joinGame}>Join game</button>
</div>
