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

        var gameEngine = new GameEngine(_hubContext, _repository, game);
        await gameEngine.RunGame();
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
        await _repository.Update(game);

        // Set this players round-state  to submitted
        if (player.ConnectionId != null)
        {
            await _hubContext.Clients.Client(player.ConnectionId)
                .SendAsync("round-state", new RoundState(Models.RoundState.PlayerSubmitted, null));

            // Inform other players that this player has submitted a  word.
            await _hubContext.Clients.GroupExcept(game.GameId, player.ConnectionId)
                .SendAsync("word-submitted", player.Name);
        }
    }
}

public class GameEngine
{
    private const int PreGameCountdownSeconds = (5);
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;

    public GameEngine(IHubContext<Hub> hubContext, GameRepository repository, Game game)
    {
        _hubContext = hubContext;
        _repository = repository;
        Game = game;
    }

    private int RoundLengthSeconds { get; set; } = 12;
    public int SummaryViewLengthSeconds { get; set; } = 7;
    public int PointsViewLengthSeconds { get; set; } = 7;
    private Game Game { get; }
    public int RoundsPlayed { get; set; } = 0;

    public async Task RunGame()
    {
        await PreGameCountdown();
        await StartGame();
        var roundNumbers = Enumerable.Range(1, Game.GameConfig.NumberOfRounds).ToList();
        foreach (var round in roundNumbers)
        {
            await PlayRound(round);
        }
    }

    private async Task PreGameCountdown()
    {
        // Set GameState "Starting" in X seconds
        var startUtc = DateTime.UtcNow.AddSeconds(PreGameCountdownSeconds);
        Game.State = GameState.Starting;
        Game.StartedTime = startUtc;
        await _repository.Update(Game);

        // GameState: Starting  - NOTIFY PLAYERS THAT GAME WILL START SOON - 
        Console.WriteLine(($"game will start in {PreGameCountdownSeconds}"));
        await _hubContext.Clients.Group(Game.GameId).SendAsync("gamestate", Game.State.Value,
            new GameDto(Game.Players, Game.HostPlayer, Game.GameId, Game.State.Value, Game.StartedTime, Game.EndedTime,
                Game.CurrentRoundNumber));

        // WAIT UNTIL GAME SHOULD START
        await Task.Delay(PreGameCountdownSeconds * 1000);
    }

    private async Task StartGame()
    {
        Game.State = GameState.Started;
        await _repository.Update(Game);
        await _hubContext.Clients.Group(Game.GameId).SendAsync("gamestate", Game.State.Value,
            new GameDto(Game.Players, Game.HostPlayer, Game.GameId, Game.State.Value, Game.StartedTime, Game.EndedTime,
                Game.CurrentRoundNumber));
        Console.WriteLine(("Game has started!"));
    }

    private async Task PlayRound(int roundNumber)
    {
        Console.WriteLine($"Starting round {roundNumber}");
        Game.CurrentRoundNumber = roundNumber;
        await _repository.Update(Game);

        var roundEndsUtc = DateTime.UtcNow.AddSeconds(RoundLengthSeconds);
        var roundInfo = new RoundInfo(Game.CurrentRoundNumber, RoundLengthSeconds, roundEndsUtc);

        Console.WriteLine($"Round ends: {roundEndsUtc}");
        // SEND ROUND INFO ROUND 1 
        await _hubContext.Clients.Group(Game.GameId).SendAsync("round-info", roundInfo);
        //SEND ROUND STATE **STARTED** ROUND 1
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", new RoundState(Models.RoundState.Started, new RoundStateData(new List<PlayerPoints>(), new List<PlayerPoints>(), 7)));

        // Wait for round to end
        await Task.Delay(RoundLengthSeconds * 1000);
        await RoundSummary();
        await RoundPoints();
    }

    private async Task RoundSummary()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var game = await _repository.Get(Game.GameId);
        
        var roundPoints = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber).Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var data = new RoundStateData(roundPoints, roundPoints, 7);
        
        //todo: Send points list with event
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("round-state", new RoundState(Models.RoundState.Summary, data));
        await Task.Delay(SummaryViewLengthSeconds * 1000);
    }
    //todo: RoindPoints not needed, summary is enough
    private async Task RoundPoints()
    {
        //SEND ROUND STATE **POINTS** ROUND 1
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", new RoundState(Models.RoundState.Points, new RoundStateData(null, null, 7)));
        await Task.Delay(PointsViewLengthSeconds * 1000);
    }
}

public record PlayerPoints(Player Player, int Points);
public record RoundStateData(List<PlayerPoints> RoundPoints, List<PlayerPoints> TotalPoints, int ViewLengthSeconds);
public record RoundSubmission(Player Player, int Round, string Word, DateTime SubmittedAtUtc, int Score,
    bool IsCorrectWord);
public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);
public record RoundState(Models.RoundState State, RoundStateData? Data = null);