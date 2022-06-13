using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameService
{
    private readonly GamePool _gamePool;
    private readonly IHubContext<Hub> _hubContext;

    public GameService(IHubContext<Hub> hubContext, GamePool gamePool)
    {
        _hubContext = hubContext;
        _gamePool = gamePool;
    }

    public Task<GameDto> CreateGame(string playerName, string playerId)
    {
        var config = new GameConfig
        {
            Language = Language.Norwegian,
            RoundLengthSeconds = 60
        };
        var hostPlayer = new Player(playerName, playerId);
        var game = new Game(config, Utils.GenerateGameId(), Utils.GenerateSolution(), hostPlayer, _hubContext);

        _gamePool.Add(game);
        return Task.FromResult(game.GetDto());
    }

    public async Task<GameDto> AddPlayer(string playerName, string playerId, string gameId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(g => g.GameId == gameId);

        if (game is null) throw new ArgumentException("No game with that ID found.");

        var player = new Player(playerName, playerId);
        game.Players.Add(player);

        //todo:  Also, check if game is full. If so, trigger game start event.


        //send player event
        await _hubContext.Clients.Group(gameId).SendAsync("player-event", player, "PLAYER_JOIN");

        // send updated game
        var dto = await game.PublishUpdatedGame();
        return dto;
    }

    public void AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = _gamePool.CurrentGames.FirstOrDefault(g => g.GameId == gameId);
        var player = game?.Players.FirstOrDefault(e => e.Id == playerId);
        if (player != null) player.ConnectionId = connectionId;
    }

    public async Task Start(string gameId, string playerId)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game is null) throw new ArgumentException("No game with that id found");

        if (game.HostPlayer.Id != playerId) throw new ArgumentException("Only host player can start the game.");

        if (game.GameViewEnum.Value != GameViewEnum.Lobby.Value)
            throw new ArgumentException("Game not in 'Lobby' state, can't start this game.");

        await game.Start();
    }

    public async Task SubmitWord(string playerId, string gameId, string word)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = _gamePool.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game is null) throw new ArgumentException("No game with that id found");

        var player = game.Players.FirstOrDefault(e => e.Id == playerId);
        if (player is null) throw new ArgumentException("No player with that id found");

        if (game.GameViewEnum.Value != GameViewEnum.Started.Value)
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");

        if (word.Length != game.Config.WordLength)
            throw new ArgumentException("Length of word does not match current game's word length");

        var isCorrect = ScoreCalculator.IsCorrectWord(game, word);

        var evaluation = ScoreCalculator.CalculateLetterEvaluations(game, word);
        var submission =
            new RoundSubmission(player, game.CurrentRoundNumber, word, DateTime.UtcNow, evaluation, isCorrect);

        game.RoundSubmissions.Add(submission);

        game.Persist();

        await game.PublishUpdatedGame();

        if (player.ConnectionId != null)
            // Todo: Replace with more general notification type
            // Inform other players that this player has submitted a  word.
            await _hubContext.Clients.GroupExcept(game.GameId, player.ConnectionId)
                .SendAsync("word-submitted", player.Name);

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