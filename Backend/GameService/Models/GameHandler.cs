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
    private readonly IGamePublisher _publisher;
    private readonly ValidWords _validWords;

    public GameHandler(IHubContext<Hub> hubContext, GamePool gamePool, ILogger<GameHandler> logger,
        PlayerConnectionsDictionary connectionsDictionary, IGamePublisher publisher, ValidWords validWords)
    {
        _hubContext = hubContext;
        _gamePool = gamePool;
        _logger = logger;
        _connectionsDictionary = connectionsDictionary;
        _publisher = publisher;
        _validWords = validWords;
    }


    public void SetupNewGame(IGame game, Player player)
    {
        game.AddPlayer(player, true);

        _gamePool.AddGame(game);
        _logger.LogInformation("{Player} created game {GameId} at {Time}", player.Name, game.GameId, DateTime.UtcNow);
    }


    public async Task<GameDto> AddPlayerWithGameId(Player player, string gameId)
    {
        /*  --- VALIDATION --- */
        var game = FindGame(gameId);
        if (game is null) throw new ArgumentException("No game with that ID found.");
        /*  --- VALIDATION END --- */

        game.AddPlayer(player);


        //todo: replace with general notification event 
        await _publisher.PublishPlayerJoined(game, player);

        // send updated game
        await _publisher.PublishUpdatedGame(game);

        if (!player.IsBot)
            _logger.LogInformation("{Player} joined game {GameId} at {Time}", player.Name, game.GameId,
                DateTime.UtcNow);

        return game.GetDto();
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

    //TODO: Create ConnectionsHandler class and move this and method above to that
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

            _gamePool.RemoveGame(game);
            RemoveAllGameConnections(game);
        }
    }

    public Task<IResult> StartGame(string gameId, string playerId)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));
        var game = FindGame(gameId);
        if (game is null) throw new NullReferenceException();

        if (game.HostPlayer?.Id != playerId)
            return Task.FromResult(Results.BadRequest("Only host player can start the game"));

        if (game.GameViewEnum != GameViewEnum.Lobby)
            return Task.FromResult(Results.BadRequest("Game not in 'Lobby' state, can't start this game."));
        /*  --- VALIDATION END --- */

        Task.Run(async () =>
        {
            await game.RunGame();

            // --- When game has ended --- // 
            if (game.GameViewEnum != GameViewEnum.Solved &&
                game.GameViewEnum != GameViewEnum.EndedUnsolved &&
                game.GameViewEnum != GameViewEnum.Abandoned)
            {
                _logger.LogWarning(
                    "Game with id {ID} should be in ended state, but isn`t. Was not removed from game pool at {Time}",
                    game.GameId, DateTime.UtcNow);
            }
            else
            {
                RemoveAllGameConnections(game);
                _gamePool.RemoveGame(game);

                _logger.LogInformation("Game with id {ID} has ended and is removed from game pool at {Time}",
                    game.GameId,
                    DateTime.UtcNow);
            }
        });

        return Task.FromResult(Results.Ok());
    }


    private void RemoveAllGameConnections(IGame game)
    {
        foreach (var connection in game.CurrentConnections)
            //todo: Disconnect every client and remove them from array
            _connectionsDictionary.RemovePlayerConnection(connection);

        game.CurrentConnections = new List<string>();

        _logger.LogInformation("Removed all game connections");
    }

    public async Task<IResult> SubmitWord(string gameId, string playerId, string word)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));
        var game = FindGame(gameId);

        var player = game?.Players.FirstOrDefault(e => e.Id == playerId);
        if (player is null) throw new ArgumentException("No player with that id found");

        if (game?.GameViewEnum != GameViewEnum.Started)
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");

        if (word.Length != game.Config.WordLength)
            throw new ArgumentException("Length of word does not match current game's word length");
        /*  --- VALIDATION END --- */

        if (!_validWords.IsValidWord(word)) return Results.BadRequest("Submitted word is not valid");

#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
        {
            game.AddRoundSubmission(player, word);
            game.AddPlayerLetterHints(player);

            await _publisher.PublishUpdatedGame(game);
            _logger.LogInformation("Word: {Word} submitted for Game with Id {ID} at {Time}", word, game.GameId,
                DateTime.UtcNow);

            if (player.ConnectionId != null)
                // Todo: Replace with more general notification type
                // Inform other players that this player has submitted a  word.
                await _hubContext.Clients.GroupExcept(game.GameId, player.ConnectionId)
                    .SendAsync("word-submitted", player.Name);

            CheckIfRoundShouldEnd(game);
        });

        return await Task.FromResult(Results.Ok());
    }

    private IGame? FindGame(string gameId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        return game;
    }

    private void CheckIfRoundShouldEnd(IGame game)
    {
        var submissionsCount =
            game.RoundSubmissions.Where(e => e.RoundNumber == game.CurrentRoundNumber).ToList().Count;
        var playersCount = game.Players.Count;

        if (submissionsCount == playersCount) game.CurrentRound?.EndRoundEndEarly();
    }
}