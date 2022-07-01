using Backend.BotPlayerService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IRound
{
    DateTime RoundEndsUtc { get; set; }
    DateTime PreRoundEndsUtc { get; set; }
    int RoundNumber { get; set; }
    Task PlayRound();
    void EndRoundEndEarly();
    Task ShowSummary();

    Round SetRoundOptions(int roundNumber, int roundLengthSeconds, int summaryLengthSeconds,
        int preRoundCountdownSeconds);

    RoundDto GetDto();
}

public class Round : IRound
{
    private int _preRoundCountdownSeconds;
    private int _roundLengthSeconds;
    private RoundViewEnum _roundViewEnum = RoundViewEnum.NotStarted;
    private int _summaryLengthSeconds;
    private string? GameId { get; set; }
    private IGamePublisher? GamePublisher { get; set; }
    private IBotPlayerHandler? BotPlayerHandler { get; set; }


    private CancellationTokenSource Token { get; } = new();
    public int RoundNumber { get; set; }
    public DateTime RoundEndsUtc { get; set; }
    public DateTime PreRoundEndsUtc { get; set; }

    public async Task PlayRound()
    {
        RoundEndsUtc = DateTime.UtcNow.AddSeconds(_roundLengthSeconds);
        _roundViewEnum = RoundViewEnum.Playing;

        if (GameId is not null)
        {
            BotPlayerHandler?.RequestBotsRoundSubmission(GameId);
            GamePublisher?.PublishUpdatedGame(GameId);
        }

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
        if (GameId is not null && GamePublisher is not null) GamePublisher.PublishUpdatedGame(GameId);
        await Task.Delay(_summaryLengthSeconds * 1000);
    }

    public RoundDto GetDto()
    {
        return new RoundDto(RoundNumber, _roundLengthSeconds, RoundEndsUtc, _roundViewEnum, PreRoundEndsUtc);
    }

    public Round SetRoundOptions(int roundNumber, int roundLengthSeconds, int summaryLengthSeconds,
        int preRoundCountdownSeconds)
    {
        RoundNumber = roundNumber;
        _roundLengthSeconds = roundLengthSeconds;
        _summaryLengthSeconds = summaryLengthSeconds;
        _preRoundCountdownSeconds = preRoundCountdownSeconds;
        return this;
    }

    public Round SetPublisher(string gameId, IGamePublisher gamePublisher)
    {
        GameId = gameId;
        GamePublisher = gamePublisher;
        return this;
    }

    public Round SetBotPlayerHandler(IBotPlayerHandler botPlayerHandler)
    {
        BotPlayerHandler = botPlayerHandler;
        return this;
    }

    public async Task PreRoundCountdown()
    {
        RoundEndsUtc = DateTime.UtcNow.AddSeconds(_roundLengthSeconds);
        PreRoundEndsUtc = DateTime.UtcNow.AddSeconds(_preRoundCountdownSeconds);
        if (GameId is not null && GamePublisher is not null) GamePublisher.PublishUpdatedGame(GameId);
        await Task.Delay(_preRoundCountdownSeconds * 1000);
    }
}