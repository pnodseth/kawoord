using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class GameService
{
    private readonly GameRepository _repository;
    private readonly IHubContext<Hub> _hubContext;

    public GameService(IHubContext<Hub> hubContext, GameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task Start(string gameId, string playerId)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));
        
        var game = await _repository.Get(gameId);
        if (game is null)
        {
            throw new ArgumentException("No game with that id found");
        }
        if (game.HostPlayer.Id != playerId)
        {
            throw new ArgumentException("Only host player can start the game.");
        }
        if (game.State.Value != GameState.Lobby.Value)
        {
            throw new ArgumentException("Game not in 'Lobby' state, can't start this game.");
        }

        const int startingInSeconds = (10);
        var startUtc = DateTime.UtcNow.AddSeconds(startingInSeconds);
        game.State = GameState.Starting;
        game.StartedTime = startUtc;
        await _repository.Update(game);
        
        // Notify players that game state has changed
        Console.WriteLine(("game will start in 10"));
        await _hubContext.Clients.Group(gameId).SendAsync("gamestate", game.State.Value, new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber));
        
        await Task.Delay(3000);
        game.State = GameState.Started;
        await _repository.Update(game);
        Console.WriteLine(("game has started!"));
        await _hubContext.Clients.Group(gameId).SendAsync("gamestate", game.State.Value, new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber));
        await Task.Delay(2000);

        const int roundLengthSeconds = 5;
        var roundEndsUtc = DateTime.UtcNow.AddSeconds(roundLengthSeconds);
        var RoundOneInfo = new RoundInfo(game.CurrentRoundNumber, roundLengthSeconds, roundEndsUtc);
        
        Console.WriteLine($"Round ends: {roundEndsUtc}");
        // SEND ROUND INFO ROUND 1 
        await _hubContext.Clients.Group(gameId).SendAsync("round-info", RoundOneInfo);
        //SEND ROUND STATE **STARTED** ROUND 1
        await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateInfo(RoundState.Started, roundLengthSeconds));
        await Task.Delay(roundLengthSeconds * 1000);
        
        //SEND ROUND STATE **SUMMARY** ROUND 1
        const int summaryLengthSeconds = 3;
        await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateInfo(RoundState.Summary, summaryLengthSeconds));
        await Task.Delay(roundLengthSeconds * 1000);
        
        //SEND ROUND STATE **POINTS** ROUND 1
        const int pointsLengthSeconds = 3;
        await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateInfo(RoundState.Points, pointsLengthSeconds));
        await Task.Delay(pointsLengthSeconds * 1000);
    }

    public async Task SubmitWord(string playerId, string gameId, string word)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));
        
        var game = await _repository.Get(gameId);
        var player = game.Players.FirstOrDefault(e => e.Id == playerId);
        
        if (player is null)
        {
            throw new ArgumentException("No player with that id found");
        }
        if (game is null)
        {
            throw new ArgumentException("No game with that id found");
        }
        if (game.State.Value != GameState.Started.Value)
        {
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");
        }

        if (word.Length != game.GameConfig.WordLength)
        {
            throw new ArgumentException("Length of word does not match current game's word length");
        }

        var isCorrect = ScoreCalculator.IsCorrectWord(game, word);
        var score = ScoreCalculator.CalculateSubmissionScore(game, word);
        var submission = new RoundSubmission(player, game.CurrentRoundNumber, word, DateTime.UtcNow, score, isCorrect);
        game.RoundSubmissions.Add(submission);
        _repository.Update(game);
        
        //await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateInfo(RoundState.PlayerSubmitted, 0 ));
        await _hubContext.Clients.Client(player.ConnectionId)
            .SendAsync("round-state", new RoundStateInfo(RoundState.PlayerSubmitted, 0));

    }
}

public record RoundSubmission(Player Player, int Round, string Word, DateTime SubmittedAtUtc, int Score, bool IsCorrectWord);

public static class ScoreCalculator
{
    public static int CalculateSubmissionScore(Game game, string word)
    {
        return 10;
    }

    public static bool IsCorrectWord(Game game, string word)
    {
        return game.Solution.Equals(word);
    }
}

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record RoundStateInfo(RoundState State, int? DurationSec);
