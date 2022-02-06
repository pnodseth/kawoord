using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class Round
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;
    private Game Game { get; }
    public Boolean Completed { get; set; } // Flag if all players have submitted before time is out, then we can cancel task.
    private CancellationTokenSource Token { get; } = new CancellationTokenSource();
    public int RoundNumber { get; set; }
    public Round(IHubContext<Hub> hubContext, GameRepository repository, Game game, int roundNumber)
    {
        _hubContext = hubContext;
        _repository = repository;
        Game = game;
        RoundNumber = roundNumber;
    }
    

    public async Task PlayRound()
    {
        if (Game.CurrentRoundNumber >= Game.Config.NumberOfRounds) return; //todo: Should trigger gameCompleted event here
        
        Game.CurrentRoundNumber = RoundNumber;  
        SetRoundStarted();
        await Task.Delay(Game.Config.RoundLengthSeconds * 1000, Token.Token);
        await SetRoundEnded(); //show summary view
        await Task.Delay(Game.Config.RoundSummaryLengthSeconds * 1000);
    }

    public async Task EndEarly()
    {
        Token.Cancel();
    }

    private async void SetRoundStarted()
    {
        Console.WriteLine($"Starting round {Game.CurrentRoundNumber}");
        await _repository.Update(Game);

        var roundEndsUtc = DateTime.UtcNow.AddSeconds(Game.Config.RoundLengthSeconds);
        var roundInfo = new RoundInfo(Game.CurrentRoundNumber, Game.Config.RoundLengthSeconds, roundEndsUtc);
        await _hubContext.Clients.Group(Game.GameId).SendAsync("round-info", roundInfo);
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", RoundState.Started);
    }

    private async Task SetRoundEnded()
    {
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", RoundState.Summary);
        
        //todo: Send points
        // Schedule next round from here.
        var points = await GetRoundSummary();
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("points", points);
        
        //todo: Dispose timer here

        
    }

    private async Task<RoundAndTotalPoints> GetRoundSummary()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var game = await _repository.Get(Game.GameId);
        var roundPoints = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber).Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var points = new RoundAndTotalPoints(roundPoints, roundPoints, 7);
        return points;
    }
}