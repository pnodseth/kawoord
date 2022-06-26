using Backend.GameService.Models.Enums;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public class Round
{
    private int _roundLengthSeconds;
    private RoundViewEnum _roundViewEnum = RoundViewEnum.NotStarted;
    private int _summaryLengthSeconds;
    public DateTime RoundEndsUtc;
    public int RoundNumber;
    private CancellationTokenSource Token { get; } = new();

    public async Task PlayRound()
    {
        _roundViewEnum = RoundViewEnum.Playing;

        // We now wait for the configured round length, until ending the round. Except if all players have submitted a word,
        // where a cancellationToken is passed, which throws an exception, and the round ends early.
        try
        {
            // Wait for the configured round length
            await Task.Delay(_roundLengthSeconds * 1000, Token.Token);
        }
        catch (TaskCanceledException)
        {
            // Round ended early since all players submitted
            await Task.Delay(2 * 1000);
        }
    }

    public void EndRoundEndEarly()
    {
        Token.Cancel();
    }

    public async Task ShowSummary()
    {
        _roundViewEnum = RoundViewEnum.Summary;
        await Task.Delay(_summaryLengthSeconds * 1000);
    }

    public Round SetRoundOptions(int roundNumber, int roundLengthSeconds, int summaryLengthSeconds)
    {
        RoundNumber = roundNumber;
        _roundLengthSeconds = roundLengthSeconds;
        RoundEndsUtc = DateTime.UtcNow.AddSeconds(roundLengthSeconds);
        _summaryLengthSeconds = summaryLengthSeconds;
        return this;
    }

    public RoundDto GetDto()
    {
        return new RoundDto(RoundNumber, _roundLengthSeconds, RoundEndsUtc, _roundViewEnum);
    }
}