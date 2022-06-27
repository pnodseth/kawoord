using Backend.GameService.Models;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public interface IBotPlayerHandler
{
    Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 4000);

    void RequestBotsRoundSubmission(IGame game);
}

public class BotPlayerHandler : IBotPlayerHandler
{
    private readonly IGameHandler _gameHandler;
    private readonly Random _random = new();
    private readonly ISolutionWords _solutionWords;
    private readonly IValidWords _validWords;

    public BotPlayerHandler(IGameHandler gameHandler, ISolutionWords solutionWords, IValidWords validWords)
    {
        _gameHandler = gameHandler;
        _solutionWords = solutionWords;
        _validWords = validWords;
    }

    public async Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 4000)
    {
        var addTimeRemaining = maxTimeToLastAddedMs;
        var botGenerator = new BotPlayerGenerator();

        // ONLY FOR DEV


        // Add players at random intervals
        foreach (var unused in Enumerable.Range(1, numberOfBots))
        {
            var randomWaitTime = _random.Next(timeToFirstAddedMs, addTimeRemaining);
            Console.WriteLine($"Waiting {randomWaitTime} before adding next player");
            await Task.Delay(randomWaitTime);

            await _gameHandler.AddPlayerWithGameId(botGenerator.GeneratePlayer(), gameId);
            addTimeRemaining -= randomWaitTime;
        }

        Console.WriteLine("Done adding bot players");
    }

    public void RequestBotsRoundSubmission(IGame game)
    {
        if (game.BotPlayers.Count <= 0) return;
        foreach (var botPlayer in game.BotPlayers)
            Task.Run(async () => { await SubmitWord(botPlayer, game); });
    }

    private async Task SubmitWord(Player botPlayer, IGame game)
    {
        if (game.CurrentRound is null) throw new ArgumentException("CurrentRound not found");

        var word = FindWordToSubmit(botPlayer, game);

        double minSubmissionTimeMs = 4000;
        var maxSubmissionTimeMs = (game.CurrentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds;

        switch (game.CurrentRound.RoundNumber)
        {
            case 1:
                maxSubmissionTimeMs = 12000;
                break;
            case 2:
                minSubmissionTimeMs = 8000;
                maxSubmissionTimeMs = 18000;
                break;
            case 3:
                minSubmissionTimeMs = 14000;
                break;
            case 4:
                minSubmissionTimeMs = 20000;
                break;
            case 5:
                minSubmissionTimeMs = 26000;
                break;
            case 6:
                minSubmissionTimeMs = 30000;
                break;
        }

        // Make sure minDelayTime and maxDelayTime is not after round ends
        if (minSubmissionTimeMs > (game.CurrentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds)
            minSubmissionTimeMs = (game.CurrentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds - 10000;

        if (maxSubmissionTimeMs > (game.CurrentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds)
            maxSubmissionTimeMs = (game.CurrentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds - 1000;

        // Get a random submission time from min and max, and submit.
        var submissionTime = _random.Next(Convert.ToInt32(minSubmissionTimeMs), Convert.ToInt32(maxSubmissionTimeMs));

        Console.WriteLine($"Bot {botPlayer.Name} submitted: {word}");
        await Task.Delay(submissionTime);
        await _gameHandler.SubmitWord(game.GameId, botPlayer.Id, word);
    }

    private string FindWordToSubmit(Player player, IGame game)
    {
        if (game.CurrentRound is null) throw new ArgumentException("CurrentRound not found");

        if (game.CurrentRound.RoundNumber == 1) return _validWords.GetRandomWord();

        var playerLetterHints = game.PlayerLetterHints.Find(e => e.Player.Id == player.Id);
        return playerLetterHints is null
            ? _validWords.GetRandomWord()
            : _solutionWords.GetWordBasedOnCorrectLetters(playerLetterHints);
    }
}