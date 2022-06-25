using Backend.GameService.Models;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public class BotPlayerHandler

{
    private readonly GameHandler _gameHandler;
    private readonly Random _random = new();
    private readonly SolutionsSingleton _solutions = SolutionsSingleton.GetInstance;
    private readonly ValidWordsSingleton _validWords = ValidWordsSingleton.GetInstance;

    public BotPlayerHandler(GameHandler gameHandler)
    {
        _gameHandler = gameHandler;
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

    public void RequestBotsRoundSubmission(GameDto gameDto)
    {
        if (_gameHandler.Game?.BotPlayers != null)
            foreach (var botPlayer in _gameHandler.Game.BotPlayers)
                Task.Run(async () => { await SubmitWord(botPlayer, gameDto); });
    }

    private async Task SubmitWord(Player botPlayer, GameDto game)
    {
        var currentRound = game.Rounds.Find(r => r.RoundNumber == game.CurrentRoundNumber);
        if (currentRound is null) throw new ArgumentException("CurrentRound not found");

        var word = FindWordToSubmit(botPlayer, game);

        double minSubmissionTimeMs = 4000;
        var maxSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds;

        switch (currentRound.RoundNumber)
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

        // Make sure minDelayTime and maxDelaytime is not after round ends
        if (minSubmissionTimeMs > (currentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds)
            minSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds - 10000;

        if (maxSubmissionTimeMs > (currentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds)
            maxSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).TotalMilliseconds - 1000;

        // Get a random submission time from min and max, and submit.
        var submissionTime = _random.Next(Convert.ToInt32(minSubmissionTimeMs), Convert.ToInt32(maxSubmissionTimeMs));

        Console.WriteLine($"Bot {botPlayer.Name} submitted: {word}");
        await Task.Delay(submissionTime);
        await _gameHandler.SubmitWord(botPlayer.Id, word);
    }

    private string FindWordToSubmit(Player player, GameDto game)
    {
        var currentRound = game.Rounds.Find(r => r.RoundNumber == game.CurrentRoundNumber);
        if (currentRound is null) throw new ArgumentException("CurrentRound not found");

        if (currentRound.RoundNumber == 1) return _validWords.GetRandomWord();

        var playerLetterHints = game.PlayerLetterHints.Find(e => e.Player.Id == player.Id);
        return playerLetterHints is null
            ? _validWords.GetRandomWord()
            : _solutions.GetWordBasedOnCorrectLetters(playerLetterHints);
    }
}