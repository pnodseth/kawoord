using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class Round
{
    private readonly IHubContext<Hub> _hubContext;
    public Game Game { get; }
    private CancellationTokenSource Token { get; } = new();
    public int RoundNumber { get; }

    public Round(IHubContext<Hub> hubContext, Game game, int roundNumber)
    {
        _hubContext = hubContext;
        Game = game;
        RoundNumber = roundNumber;
    }


    public async Task PlayRound()
    {
        if (Game.CurrentRoundNumber >= Game.Config.NumberOfRounds)
            return; //todo: Should trigger gameCompleted event here

        Game.CurrentRoundNumber = RoundNumber;
        SetRoundStarted();

        try
        {
            await Task.Delay(Game.Config.RoundLengthSeconds * 1000, Token.Token);
            //await SetRoundEnded(); //show summary view
        }
        catch (TaskCanceledException)
        {
            // Round ended early since all players submitted
            await Task.Delay(2 * 1000);
        }
        finally
        {
            await SetRoundEnded();
            await Task.Delay(Game.Config.RoundSummaryLengthSeconds * 1000);
        }
    }

    public void EndEarly()
    {
        Token.Cancel();
    }

    private async void SetRoundStarted()
    {
        Console.WriteLine($"Starting round {Game.CurrentRoundNumber}");

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

        var points = GetRoundSummary();
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("points", points);
    }

    private RoundAndTotalPoints GetRoundSummary()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var roundPoints = Game.RoundSubmissions.Where(r => r.Round == Game.CurrentRoundNumber)
            .Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var points = new RoundAndTotalPoints(roundPoints, roundPoints, 7);
        return points;
    }
}