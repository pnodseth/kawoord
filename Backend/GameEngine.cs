using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;


public record RoundAndTotalPoints(List<PlayerPoints> RoundPoints, List<PlayerPoints> TotalPoints, int ViewLengthSeconds);
public record PlayerPoints(Player Player, int Points);
public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);


public class GameEngine
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;
    

    

    public GameEngine(IHubContext<Hub> hubContext, GameRepository repository, Game game)
    {
        _hubContext = hubContext;
        _repository = repository;
        Game = game;
    }

    public int SummaryViewLengthSeconds { get; set; } = 7;
    private Game Game { get; }
    public int RoundsPlayed { get; set; } = 0;

    public async Task RunGame()
    {
        await PreGameCountdown();
        await StartGame();
        var roundNumbers = Enumerable.Range(1, Game.Config.NumberOfRounds).ToList();
        foreach (var round in roundNumbers)
        {
            await PlayRound(round);
        }
    }

    private async Task PreGameCountdown()
    {
        // Set GameState "Starting" in X seconds
        var startUtc = DateTime.UtcNow.AddSeconds(Game.Config.PreGameCountdownSeconds);
        Game.State = GameState.Starting;
        Game.StartedTime = startUtc;
        await _repository.Update(Game);

        // GameState: Starting  - NOTIFY PLAYERS THAT GAME WILL START SOON - 
        Console.WriteLine(($"game will start in {Game.Config.PreGameCountdownSeconds}"));
        await _hubContext.Clients.Group(Game.GameId).SendAsync("gamestate", Game.State.Value,
            new GameDto(Game.Players, Game.HostPlayer, Game.GameId, Game.State.Value, Game.StartedTime, Game.EndedTime,
                Game.CurrentRoundNumber));

        // WAIT UNTIL GAME SHOULD START
        await Task.Delay(Game.Config.PreGameCountdownSeconds * 1000);
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

        var roundEndsUtc = DateTime.UtcNow.AddSeconds(Game.Config.RoundLengthSeconds);
        var roundInfo = new RoundInfo(Game.CurrentRoundNumber, Game.Config.RoundLengthSeconds, roundEndsUtc);

        Console.WriteLine($"Round ends: {roundEndsUtc}");
        // SEND ROUND INFO ROUND 1 
        await _hubContext.Clients.Group(Game.GameId).SendAsync("round-info", roundInfo);
        
        //SEND ROUND STATE **STARTED** ROUND 1
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", RoundState.Started);
                

        // Wait for round to end
        await Task.Delay(Game.Config.RoundLengthSeconds * 1000);
        await RoundSummary();
    }

    private async Task RoundSummary()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var game = await _repository.Get(Game.GameId);
        var roundPoints = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber).Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var points = new RoundAndTotalPoints(roundPoints, roundPoints, 7);
        
        
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("round-state", RoundState.Summary);
        
        // Also send Summary of points for this round and total points 
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("points", points);
        //Also send event with Points data
        await Task.Delay(SummaryViewLengthSeconds * 1000);
    }
    
}