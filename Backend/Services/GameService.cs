using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameService
{
    private readonly GameRunner _gameRunner;
    private readonly IHubContext<Hub> _hubContext;

    public GameService(IHubContext<Hub> hubContext, GameRunner gameRunner)
    {
        _hubContext = hubContext;
        _gameRunner = gameRunner;
    }

    public async Task<GameDto> CreateGame(string playerName, string playerId)
    {
        var config = new GameConfig
        {
            Language = Language.Norwegian,
            RoundLengthSeconds = 60
        };
        var hostPlayer = new Player(playerName, playerId);
        var game = new Game(config, Utils.GenerateGameId(), Utils.GenerateSolution(), hostPlayer);

        await _gameRunner.Add(game);
        return new GameDto(game.Players, game.HostPlayer, game.GameId, game.GameStateEnum, game.StartedAtUtc,
            game.EndedTime, game.CurrentRoundNumber, game.RoundInfos, game.RoundStateEnum);
    }

    public async Task<GameDto> AddPlayer(string playerName, string playerId, string gameId)
    {
        var game = _gameRunner.CurrentGames.FirstOrDefault(g => g.GameId == gameId);

        if (game is null) throw new ArgumentException("No game with that ID found.");

        var player = new Player(playerName, playerId);
        game.Players.Add(player);
        await _gameRunner.Persist(gameId);

        //todo:  Also, check if game is full. If so, trigger game start event.


        //send player event
        await _hubContext.Clients.Group(gameId).SendAsync("player-event", player, "PLAYER_JOIN");

        // send updated game
        var gameDto = new GameDto(game.Players, game.HostPlayer, game.GameId, game.GameStateEnum,
            game.StartedAtUtc,
            game.EndedTime,
            game.CurrentRoundNumber, game.RoundInfos, game.RoundStateEnum);
        await _hubContext.Clients.Group(gameId).SendAsync("game-update", gameDto);

        return gameDto;
    }

    public async Task AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = _gameRunner.CurrentGames.FirstOrDefault(g => g.GameId == gameId);
        var player = game?.Players.FirstOrDefault(e => e.Id == playerId);
        if (player != null) player.ConnectionId = connectionId;
        await _gameRunner.Persist(gameId);
    }

    public async Task Start(string gameId, string playerId)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = _gameRunner.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game is null) throw new ArgumentException("No game with that id found");

        if (game.HostPlayer.Id != playerId) throw new ArgumentException("Only host player can start the game.");

        if (game.GameStateEnum.Value != GameStateEnum.Lobby.Value)
            throw new ArgumentException("Game not in 'Lobby' state, can't start this game.");

        await _gameRunner.StartGame(game);
    }

    public async Task SubmitWord(string playerId, string gameId, string word)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = _gameRunner.CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game is null) throw new ArgumentException("No game with that id found");

        var player = game.Players.FirstOrDefault(e => e.Id == playerId);
        if (player is null) throw new ArgumentException("No player with that id found");

        if (game.GameStateEnum.Value != GameStateEnum.Started.Value)
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");

        if (word.Length != game.Config.WordLength)
            throw new ArgumentException("Length of word does not match current game's word length");

        var isCorrect = ScoreCalculator.IsCorrectWord(game, word);

        var evaluation = ScoreCalculator.CalculateLetterEvaluations(game, word);
        var submission =
            new RoundSubmission(player, game.CurrentRoundNumber, word, DateTime.UtcNow, evaluation, isCorrect);

        game.RoundSubmissions.Add(submission);
        await _gameRunner.Persist(gameId);

        if (player.ConnectionId != null)
        {
            // Set this players round-state  to submitted
            await _hubContext.Clients.Client(player.ConnectionId)
                .SendAsync("round-state", RoundStateEnum.PlayerSubmitted);

            // Inform other players that this player has submitted a  word.
            await _hubContext.Clients.GroupExcept(game.GameId, player.ConnectionId)
                .SendAsync("word-submitted", player.Name);
        }

        var submissionsCount = game.RoundSubmissions.Where(e => e.Round == game.CurrentRoundNumber).ToList().Count;
        var playersCount = game.Players.Count;

        if (submissionsCount == playersCount)
        {
            var round = _gameRunner.CurrentRounds.FirstOrDefault(e =>
                e.RoundNumber == game.CurrentRoundNumber && game.GameId == e.Game.GameId);
            round?.EndRoundEndEarly();
        }
    }
}