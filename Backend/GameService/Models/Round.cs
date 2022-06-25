using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;

namespace Backend.GameService.Models;

public class Round
{
    public DateTime RoundEndsUtc;

    public RoundViewEnum RoundViewEnum = RoundViewEnum.NotStarted;

    public Round(Game game, int roundNumber)
    {
        Game = game;
        RoundNumber = roundNumber;
        RoundEndsUtc = DateTime.UtcNow.AddSeconds(Game.Config.RoundLengthSeconds);
        RoundLengthSeconds = Game.Config.RoundLengthSeconds;
    }

    public Game Game { get; }
    private CancellationTokenSource Token { get; } = new();
    public int RoundNumber { get; }
    public int RoundLengthSeconds { get; }

    public async Task StartRound()
    {
        RoundViewEnum = RoundViewEnum.Playing;

        // We now wait for the configured round length, until ending the round. Except if all players have submitted a word,
        // where a cancellationToken is passed, which throws an exception, and the round ends early.
        try
        {
            // Wait for the configured round length
            await Task.Delay(RoundLengthSeconds * 1000, Token.Token);
        }
        catch (TaskCanceledException)
        {
            // Round ended early since all players submitted
            Console.WriteLine("Catch: Round cancelled");
            await Task.Delay(2 * 1000);
        }
        finally
        {
            await EndRound();
        }
    }

    public void EndRoundEndEarly()
    {
        Console.WriteLine("Ending round early");
        Token.Cancel();
    }


    private async Task EndRound()
    {
        // check if anyone has correct word, and if so set game state to solved.
        var roundPoints = CalculateRoundPoints();

        var winners = roundPoints.FindAll(e => e.IsCorrectWord).Select(e => e.Player).ToList();

        if (winners.Count > 0) Game.GameViewEnum = GameViewEnum.Solved;


        RoundViewEnum = RoundViewEnum.Summary;
        Game.Persist();

        await Game.Handler.PublishUpdatedGame();


        // Wait for the configured Summary length time. Unless game is solved
        if (Game.GameViewEnum != GameViewEnum.Solved)
            await Task.Delay(Game.Config.RoundSummaryLengthSeconds * 1000);
    }

    private List<WordEvaluation> CalculateRoundPoints()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var roundPoints = Game.RoundSubmissions.Where(r => r.RoundNumber == Game.CurrentRoundNumber)
            .Select(e =>
                new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc, RoundNumber))
            .ToList();
        return roundPoints;
    }
}