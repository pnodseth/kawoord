using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGameHandler
{
    void SetupNewGame(IGame game, IPlayer player);
    IResult AddPlayerWithGameId(IPlayer player, string gameId);
    Task<IResult> HandleStartGame(string gameId, string playerId);
    Task<IResult> SubmitWord(string gameId, string playerId, string word);
}

public class GameHandler : IGameHandler
{
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


    public IResult AddPlayerWithGameId(IPlayer player, string gameId)
    {
        /*  --- VALIDATION --- */
        var game = FindGame(gameId);
        if (game is null) throw new ArgumentException("No game with that ID found.");
        /*  --- VALIDATION END --- */
        if (game.PlayerCount == game.Config.MaxPlayers) return Results.BadRequest("Game is full");
        game.AddPlayer(player);


        //todo: replace with general notification event 
        _publisher.PublishPlayerJoined(game, player);

        // send updated game
        _publisher.PublishUpdatedGame(game);

        if (!player.IsBot)
            _logger.LogInformation("{Player} joined game {GameId} at {Time}", player.Name, game.GameId,
                DateTime.UtcNow);

        if (game.PlayerCount == game.Config.MaxPlayers && game.HostPlayer is not null)
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await HandleStartGame(gameId, game.HostPlayer.Id);
            });


        return Results.Ok(game.GetDto());
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
        var playersCount = game.PlayerCount;
        return submissionsCount == playersCount;
    }
}