using Microsoft.AspNetCore.SignalR;

namespace Backend.Models;

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
            return; // todo: Should trigger gameCompleted event here

        Game.CurrentRoundNumber = RoundNumber;
        //todo: need to persist game after setting currentRoundNumber;
        
        SetRoundStarted();
        
        
        // We now wait for the configured round length, until ending the round. Except if all players have submitted a word,
        // where a cancellationToken is passed, which throws an exception, and the round ends early.
        try
        {
            // Wait for the configured round length
            await Task.Delay(Game.Config.RoundLengthSeconds * 1000, Token.Token);
        }
        catch (TaskCanceledException)
        {
            // Round ended early since all players submitted
            Console.WriteLine("Catch: Round cancelled");
            await Task.Delay(2 * 1000);
        }
        finally
        {
            
            await RoundEnded();

          
        }
    }

    public void EndEarly()
    {
        Console.WriteLine("Ending round early");
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

    private async Task RoundEnded()
    {
        // check if anyone has correct word, and if so set game state to solved.
        var roundPoints = CalculateRoundPoints();
        
        var winners = roundPoints.FindAll(e => e.isCorrectWord).Select(e => e.Player).ToList();

        if (winners.Count > 0)
        {
            Game.State = GameState.Solved;
        }
        
        // Send round-state: Summary View
        if (Game.State.Value != GameState.Solved.Value)
        {
            await _hubContext.Clients.Group(Game.GameId)
                .SendAsync("round-state", RoundState.Summary);
        }

        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("points", roundPoints);
        
        // Wait for the configured Summary length time. Unless game is solved
        if (Game.State.Value != GameState.Solved.Value)
        {
            await Task.Delay(Game.Config.RoundSummaryLengthSeconds * 1000);
        }
        
    }

    private List<WordEvaluation> CalculateRoundPoints()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var roundPoints = Game.RoundSubmissions.Where(r => r.Round == Game.CurrentRoundNumber)
            .Select(e => new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc)).ToList();
        return roundPoints;
    }
}