using Backend.BotPlayerService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGameHandler
{
    void SetupNewGame(IGame game, IPlayer player);
    IResult AddPlayerToGame(IPlayer player, string gameId);
    Task<IResult> HandleStartGame(string gameId, string playerId);
    Task<IResult> SubmitWord(string gameId, string playerId, string word);
    IGame? TryAddToExistingPublicGame(IPlayer player);
    void CreatePublicGameWithPlayerAndBots(IGame game, IPlayer player, IBotPlayerHandler botPlayerHandler);
}

public class GameHandler : IGameHandler
{
    private readonly object _addPlayerLock = new();
    private readonly IConnectionsHandler _connectionsHandler;
    private readonly IGamePool _gamePool;
    private readonly ILogger<GameHandler> _logger;
    private readonly IGamePublisher _publisher;
    private readonly IValidWords _validWords;


    public GameHandler(IGamePool gamePool, ILogger<GameHandler> logger,
        IConnectionsHandler connectionsHandler, IGamePublisher publisher, IValidWords validWords)
    {
        _gamePool = gamePool;
        _logger = logger;
        _connectionsHandler = connectionsHandler;
        _publisher = publisher;
        _validWords = validWords;
    }


    public void SetupNewGame(IGame game, IPlayer player)
    {
        game.AddPlayer(player, true);

        _gamePool.AddGame(game);
        if (game.Config.Public) _gamePool.AddToAvailableGames(game);

        _logger.LogInformation("{Player} created game {GameId} at {Time}", player.Name, game.GameId, DateTime.UtcNow);
    }


    public Task<IResult> HandleStartGame(string gameId, string playerId)
    {
        /*  --- VALIDATION --- */

        // todo: Replace with fluentValidation?
        if (string.IsNullOrEmpty(playerId))
            return Task.FromResult(Results.BadRequest("GameId cannot be null or empty"));
        var game = FindGame(gameId);
        if (game is null) return Task.FromResult(Results.BadRequest("No game with that gameId found"));

        if (game.HostPlayer?.Id != playerId)
            return Task.FromResult(Results.BadRequest("Only host player can start the game"));

        if (game.GameViewEnum != GameViewEnum.Lobby)
            return Task.FromResult(Results.BadRequest("Game not in 'Lobby' state, can't start this game."));
        /*  --- VALIDATION END --- */


        Task.Run(async () => { await RunGame(game); });

        return Task.FromResult(Results.Ok());
    }

    public async Task<IResult> SubmitWord(string gameId, string playerId, string word)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(playerId)) return await Task.FromResult(Results.BadRequest("PlayerId can't be empty"));
        if (string.IsNullOrEmpty(word)) return await Task.FromResult(Results.BadRequest("Word can't be empty"));

        var game = FindGame(gameId);
        if (game is null) return await Task.FromResult(Results.BadRequest("No game with that ID found"));

        var player = game.FindPlayer(playerId);
        if (player is null) return await Task.FromResult(Results.BadRequest("No player with that ID found"));

        if (game.GameViewEnum != GameViewEnum.Started)
            return await Task.FromResult(Results.BadRequest("Incorrect GameState"));

        if (word.Length != game.Config.WordLength)
            return await Task.FromResult(Results.BadRequest("Word does not have correct length"));
        /*  --- VALIDATION END --- */

        if (!_validWords.IsValidWord(word)) return Results.BadRequest("Submitted word is not valid");

        game.AddRoundSubmission(player, word);
        game.AddPlayerLetterHints(player);

        _publisher.PublishUpdatedGame(game);
        _logger.LogInformation("Word: {Word} submitted for Game with Id {ID} at {Time}", word, game.GameId,
            DateTime.UtcNow);


        _publisher.PublishWordSubmitted(game.GameId, player);


        if (ShouldRunEndEarly(game)) game.CurrentRound?.EndRoundEndEarly();


        return await Task.FromResult(Results.Ok());
    }

    public IGame? TryAddToExistingPublicGame(IPlayer player)
    {
        lock (_addPlayerLock)
        {
            /* Check if there are any existing games available */
            var availableGame = _gamePool.GetFirstAvailableGame();
            if (availableGame is null) return null;

            /* Make sure game is not full */
            if (availableGame.PlayerAndBotCount == availableGame.Config.MaxPlayers) return null;

            AddPlayerToGame(player, availableGame.GameId);
            return availableGame;
        }
    }


    public IResult AddPlayerToGame(IPlayer player, string gameId)
    {
        /*  --- VALIDATION --- */
        var game = FindGame(gameId);
        if (game is null) throw new ArgumentException("No game with that ID found.");
        /*  --- VALIDATION END --- */
        if (game.PlayerAndBotCount == game.Config.MaxPlayers) return Results.BadRequest("Game is full");
        if (game.GameViewEnum != GameViewEnum.Lobby) return Results.BadRequest("Game is not in lobby state");

        game.AddPlayer(player);

        // game should no longer be in the public games queue so others can join it.
        if (game.PlayerAndBotCount == game.Config.MaxPlayers && game.Config.Public)
            _gamePool.RemoveFromPublicGamesQueue(game);

        _publisher.PublishPlayerJoined(game, player);
        _publisher.PublishUpdatedGame(game);

        if (!player.IsBot)
            _logger.LogInformation("{Player} joined game {GameId} at {Time}", player.Name, game.GameId,
                DateTime.UtcNow);


        if (game.PlayerAndBotCount == game.Config.MaxPlayers && game.HostPlayer is not null)
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await HandleStartGame(gameId, game.HostPlayer.Id);
            });


        return Results.Ok(game.GetDto());
    }

    public void CreatePublicGameWithPlayerAndBots(IGame game, IPlayer player, IBotPlayerHandler botPlayerHandler)
    {
        /* If no existing games available, create a new with bots */
        /*var random = new Random();
        var maxBotPlayers = game.Config.MaxPlayers - 1;
        var startWithBotPlayersCount = random.Next(1, maxBotPlayers);*/

        var botPlayer = botPlayerHandler.GetNewBotPlayer();
        SetupNewGame(game, botPlayer);
        lock (_addPlayerLock)
        {
            AddPlayerToGame(player, game.GameId);
        }

        Task.Run(async () =>
        {
            var remainingBotPlayersCount = game.Config.MaxPlayers - 2;
            await botPlayerHandler.RequestBotPlayersToGame(game.GameId,
                remainingBotPlayersCount, 0, 30000);
        });
    }

    public async Task RunGame(IGame game)
    {
        await game.RunGame();

        // --- When game has ended --- // 
        if (game.GameViewEnum is GameViewEnum.Solved or GameViewEnum.EndedUnsolved or GameViewEnum.Abandoned)
        {
            _connectionsHandler.RemoveGameConnections(game.GameId);
            _gamePool.RemoveGame(game.GameId);

            _logger.LogInformation("Game with id {ID} has ended and is removed from game pool at {Time}",
                game.GameId,
                DateTime.UtcNow);
        }
        else
        {
            _logger.LogWarning(
                "Game with id {ID} should be in ended state, but isn`t. Was not removed from game pool at {Time}",
                game.GameId, DateTime.UtcNow);
        }
    }


    private IGame? FindGame(string gameId)
    {
        return _gamePool.FindGame(gameId);
    }

    private bool ShouldRunEndEarly(IGame game)
    {
        var submissionsCount =
            game.GetCurrentRoundSubmissionsCount();
        var playersCount = game.PlayerAndBotCount;
        return submissionsCount == playersCount;
    }
}