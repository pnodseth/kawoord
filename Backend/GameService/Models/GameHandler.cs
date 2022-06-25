using Backend.BotPlayerService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.GameService.Models;

public class GameHandler
{
    private readonly PlayerConnectionsDictionary _connectionsDictionary;
    private readonly GamePool _gamePool;
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<GameHandler> _logger;
    public readonly BotPlayerHandler BotPlayerHandler;

    public GameHandler(IHubContext<Hub> hubContext, GamePool gamePool, ILogger<GameHandler> logger,
        PlayerConnectionsDictionary connectionsDictionary)
    {
        _hubContext = hubContext;
        _gamePool = gamePool;
        _logger = logger;
        _connectionsDictionary = connectionsDictionary;
        BotPlayerHandler = new BotPlayerHandler(this);
    }

    public Game? Game { get; set; }

    public GameHandler SetGameFromGameId(string gameId)
    {
        Game = FindGame(gameId);
        return this;
    }

    public void CreateGame(Player player)
    {
        Game = new Game(this);
        Game.AddPlayer(player, true);

        _gamePool.AddGame(Game);
        _logger.LogInformation("{Player} created game {GameId} at {Time}", player.Name, Game.GameId, DateTime.UtcNow);

        if (Game.GameType == GameTypeEnum.Public)
            Task.Run(async () => { await BotPlayerHandler.RequestBotPlayersToGame(Game.GameId, 2, 500); });
    }

    public GameDto GetGameDto()
    {
        if (Game is null) throw new NullReferenceException("No game for handler");
        return Game.GetDto();
    }


    public async Task AddPlayerWithGameId(Player player, string gameId)
    {
        /*  --- VALIDATION --- */
        var game = FindGame(gameId);
        if (game is null) throw new ArgumentException("No game with that ID found.");
        /*  --- VALIDATION END --- */

        game.AddPlayer(player);


        //todo: replace with general notification event 
        await _hubContext.Clients.Group(gameId).SendAsync("player-event", player, "PLAYER_JOIN");

        // send updated game
        await PublishUpdatedGame();

        if (!player.IsBot)
            _logger.LogInformation("{Player} joined game {GameId} at {Time}", player.Name, game.GameId,
                DateTime.UtcNow);
    }

    public async Task PublishUpdatedGame()
    {
        if (Game is null) throw new NullReferenceException();
        await PublishUpdatedGame();

        if (Game.GameViewEnum == GameViewEnum.Solved || Game.GameViewEnum == GameViewEnum.EndedUnsolved)
            await _hubContext.Clients.Group(Game.GameId).SendAsync("state", "solution", Game.Solution);
    }

    public void AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = FindGame(gameId);
        if (game != null)
        {
            var player = game.FindPlayer(playerId);
            if (player is null)
            {
                _logger.LogWarning("No player found for playerId: {PlayerId} in game: {Game}", playerId, game.GameId);
            }
            else
            {
                game.AddPlayerConnection(player, connectionId);
                _connectionsDictionary.AddPlayerConnection(connectionId, game);
            }
        }
        else
        {
            _logger.LogWarning("Player with connection id {ConnId} connected, but no game with id {GameId} was found",
                connectionId, gameId);
        }
    }

    public void HandleDisconnectedPlayer(string connectionId)
    {
        var game = _connectionsDictionary.GetGameFromConnectionId(connectionId);
        if (game is null)
        {
            _logger.LogWarning("No game was found for {Conn}", connectionId);
        }
        else
        {
            game.RemovePlayer(connectionId);
            _connectionsDictionary.RemovePlayerConnection(connectionId);

            //todo: broadcast to game that user disconnected

            // Check if all players have disconnected from game. If so, no need to run remaining rounds. clean up and remove game.
            if (game.CurrentConnections.Count != 0) return;
            _logger.LogInformation("No more connected clients. Cleaning up");
            game.GameViewEnum = GameViewEnum.Abandoned;
            RemoveGameFromGamePool(game);
            RemoveAllGameConnections(game);
        }
    }

    private void RemoveGameFromGamePool(Game game)
    {
        _gamePool.RemoveGame(game);
        _logger.LogInformation("Removed game from gamePool");
    }

    public Task<IResult> StartGame(string playerId)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));


        if (Game?.HostPlayer?.Id != playerId)
            return Task.FromResult(Results.BadRequest("Only host player can start the game"));

        if (Game.GameViewEnum != GameViewEnum.Lobby)
            return Task.FromResult(Results.BadRequest("Game not in 'Lobby' state, can't start this game."));
        /*  --- VALIDATION END --- */

        Task.Run(async () =>
        {
            await Game.StartGame();

            // --- When game has ended --- // 
            if (Game.GameViewEnum != GameViewEnum.Solved &&
                Game.GameViewEnum != GameViewEnum.EndedUnsolved &&
                Game.GameViewEnum != GameViewEnum.Abandoned)
            {
                _logger.LogWarning(
                    "Game with id {ID} should be in ended state, but isn`t. Was not removed from game pool at {Time}",
                    Game.GameId, DateTime.UtcNow);
            }
            else
            {
                RemoveAllGameConnections(Game);
                RemoveGameFromGamePool(Game);
                _logger.LogInformation("Game with id {ID} has ended and is removed from game pool at {Time}",
                    Game.GameId,
                    DateTime.UtcNow);
            }
        });

        return Task.FromResult(Results.Ok());
    }


    private void RemoveAllGameConnections(Game game)
    {
        foreach (var connection in game.CurrentConnections)
            //todo: Disconnect every client and remove them from array
            _connectionsDictionary.RemovePlayerConnection(connection);

        game.CurrentConnections = new List<string>();

        _logger.LogInformation("Removed all game connections");
    }

    public async Task<IResult> SubmitWord(string playerId, string word)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));


        var player = Game?.Players.FirstOrDefault(e => e.Id == playerId);
        if (player is null) throw new ArgumentException("No player with that id found");

        if (Game?.GameViewEnum != GameViewEnum.Started)
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");

        if (word.Length != Game.Config.WordLength)
            throw new ArgumentException("Length of word does not match current game's word length");
        /*  --- VALIDATION END --- */

        var validWords = ValidWordsSingleton.GetInstance;

        if (!validWords.IsValidWord(word)) return Results.BadRequest("Submitted word is not valid");

#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
        {
            Game.AddRoundSubmission(player, word);
            Game.AddPlayerLetterHints(player);
            Game.Persist();
            await PublishUpdatedGame();
            _logger.LogInformation("Word: {Word} submitted for Game with Id {ID} at {Time}", word, Game.GameId,
                DateTime.UtcNow);

            if (player.ConnectionId != null)
                // Todo: Replace with more general notification type
                // Inform other players that this player has submitted a  word.
                await _hubContext.Clients.GroupExcept(Game.GameId, player.ConnectionId)
                    .SendAsync("word-submitted", player.Name);

            CheckIfRoundShouldEnd();
        });

        return await Task.FromResult(Results.Ok());
    }

    private Game? FindGame(string gameId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        return game;
    }

    private void CheckIfRoundShouldEnd()
    {
        if (Game is null) return;

        var submissionsCount =
            Game.RoundSubmissions.Where(e => e.RoundNumber == Game.CurrentRoundNumber).ToList().Count;
        var playersCount = Game.Players.Count;

        if (submissionsCount == playersCount)
        {
            var round = Game.Rounds.FirstOrDefault(e =>
                e.RoundNumber == Game.CurrentRoundNumber && Game.GameId == e.Game.GameId);
            round?.EndRoundEndEarly();
        }
    }
}