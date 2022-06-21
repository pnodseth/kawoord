using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameHandler
{
    private readonly PlayerConnectionsDictionary _connectionsDictionary;
    private readonly GamePool _gamePool;
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<GameHandler> _logger;
    private readonly ValidWords _validWords;

    public GameHandler(IHubContext<Hub> hubContext, GamePool gamePool, ILogger<GameHandler> logger,
        ValidWords validWords, PlayerConnectionsDictionary connectionsDictionary)
    {
        _hubContext = hubContext;
        _gamePool = gamePool;
        _logger = logger;
        _validWords = validWords;
        _connectionsDictionary = connectionsDictionary;
    }

    public void SetupGame(Game game, Player player)
    {
        game.Config.Language = Language.English;
        game.Config.RoundLengthSeconds = 60;

        game.HostPlayer = player;
        game.Players.Add(player);

        _gamePool.Add(game);
        _logger.LogInformation("{Player} created game {GameId} at {Time}", player.Name, game.GameId, DateTime.UtcNow);
    }


    public async Task<GameDto> AddPlayer(string playerName, string playerId, string gameId)
    {
        /*  --- VALIDATION --- */
        var game = _gamePool.CurrentGames.FirstOrDefault(g => g.GameId == gameId);
        if (game is null) throw new ArgumentException("No game with that ID found.");
        /*  --- VALIDATION END --- */

        var player = new Player(playerName, playerId);
        game.Players.Add(player);

        //todo:  Also, check if game is full. If so, trigger game start event.


        //send player event
        await _hubContext.Clients.Group(gameId).SendAsync("player-event", player, "PLAYER_JOIN");

        // send updated game
        var dto = await game.PublishUpdatedGame();
        _logger.LogInformation("{Player} joined game {GameId} at {Time}", playerName, game.GameId, DateTime.UtcNow);
        return dto;
    }

    public void AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(g => g.GameId == gameId);
        if (game != null)
        {
            var player = game.Players.FirstOrDefault(e => e.Id == playerId);
            if (player != null) player.ConnectionId = connectionId;
            game.CurrentConnections.Add(connectionId);
            _connectionsDictionary.AddPlayerConnection(connectionId, game);
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
            game.CurrentConnections.Remove(connectionId);
            _connectionsDictionary.RemovePlayerConnection(connectionId);

            var player = game.Players.FirstOrDefault(e => e.ConnectionId == connectionId);
            if (player is not null) game.Players.Remove(player);

            //todo: broadcast to game that user disconnected

            // Check if all players have disconnected from game. If so, no need to run remaining rounds. clean up and remove game.
            if (game.CurrentConnections.Count != 0) return;
            _logger.LogInformation("No more connected clients. Cleaning up");
            game.GameViewEnum = GameViewEnum.Abandoned;
            RemoveGameFromGamePool(game);
            RemoveAllGameConnections(game);
        }
    }

    public void RemoveGameFromGamePool(Game game)
    {
        _gamePool.CurrentGames.Remove(game);
        game.Rounds.ForEach(r => _gamePool.CurrentRounds.Remove(r));
        _logger.LogInformation("Removed game from gamePool");
    }

    public Task<IResult> StartGame(string gameId, string playerId)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game is null) return Task.FromResult(Results.BadRequest("No game with that id found"));

        if (game.HostPlayer.Id != playerId)
            return Task.FromResult(Results.BadRequest("Only host player can start the game"));

        if (game.GameViewEnum.Value != GameViewEnum.Lobby.Value)
            return Task.FromResult(Results.BadRequest("Game not in 'Lobby' state, can't start this game."));
        /*  --- VALIDATION END --- */

        Task.Run(async () =>
        {
            await game.Start();

            // --- When game has ended --- // 
            if (game.GameViewEnum.Value != GameViewEnum.Solved.Value &&
                game.GameViewEnum.Value != GameViewEnum.EndedUnsolved.Value &&
                game.GameViewEnum.Value != GameViewEnum.Abandoned.Value)
            {
                _logger.LogWarning(
                    "Game with id {ID} should be in ended state, but isnt. Was not removed from game pool at {Time}",
                    game.GameId, DateTime.UtcNow);
            }
            else
            {
                RemoveAllGameConnections(game);
                RemoveGameFromGamePool(game);
                _logger.LogInformation("Game with id {ID} has ended and is removed from game pool at {Time}",
                    game.GameId,
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

    public async Task<IResult> SubmitWord(string playerId, string gameId, string word)
    {
        /*  --- VALIDATION --- */
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = FindGame(gameId);
        if (game is null) throw new ArgumentException("No game with that id found");

        var player = game.Players.FirstOrDefault(e => e.Id == playerId);
        if (player is null) throw new ArgumentException("No player with that id found");

        if (game.GameViewEnum.Value != GameViewEnum.Started.Value)
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
            game.Persist();
            await game.PublishUpdatedGame();
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

    private Game? FindGame(string gameId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        return game;
    }

    private static void CheckIfRoundShouldEnd(Game game)
    {
        var submissionsCount =
            game.RoundSubmissions.Where(e => e.RoundNumber == game.CurrentRoundNumber).ToList().Count;
        var playersCount = game.Players.Count;

        if (submissionsCount == playersCount)
        {
            var round = game.Rounds.FirstOrDefault(e =>
                e.RoundNumber == game.CurrentRoundNumber && game.GameId == e.Game.GameId);
            round?.EndRoundEndEarly();
        }
    }
}