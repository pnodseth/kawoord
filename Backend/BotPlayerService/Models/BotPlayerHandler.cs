using Backend.CommunicationService;
using Backend.GameService.Models.Dtos;
using Backend.Shared.Data.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public class BotPlayerHandler

{
    private readonly ICommunicationHandler _communicationHandler;
    private readonly Random _random = new();
    private readonly SolutionsSingleton _solutions = SolutionsSingleton.GetInstance;
    private readonly ValidWordsSingleton _validWords = ValidWordsSingleton.GetInstance;

    public BotPlayerHandler(ICommunicationHandler communicationHandler)
    {
        _communicationHandler = communicationHandler;
    }

    public async Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 30000)
    {
        var addTimeRemaining = maxTimeToLastAddedMs;
        var botGenerator = new BotPlayerGenerator();

        // Add players at random intervals
        foreach (var unused in Enumerable.Range(1, numberOfBots))
        {
            var randomWaitTime = _random.Next(4000, addTimeRemaining);
            Console.WriteLine($"Waiting {randomWaitTime} before adding next player");
            await Task.Delay(randomWaitTime);

            await _communicationHandler.AddPlayer(botGenerator.GeneratePlayer(), gameId);
            addTimeRemaining -= randomWaitTime;
        }

        Console.WriteLine("Done adding bot players");
    }

    public void RequestBotsRoundSubmission(GameDto gameDto)
    {
        var botPlayers = gameDto.Players.Where(p => p.IsBot).ToList();

        foreach (var botPlayer in botPlayers)
            Task.Run(() => { SubmitWord(botPlayer, gameDto); });
    }

    private void SubmitWord(Player botPlayer, GameDto game)
    {
        var currentRound = game.Rounds.Find(r => r.RoundNumber == game.CurrentRoundNumber);
        if (currentRound is null) throw new ArgumentException("CurrentRound not found");

        var minSubmissionTimeMs = 4000;
        var maxSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).Milliseconds;

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
        if (minSubmissionTimeMs > (currentRound.RoundEndsUtc - DateTime.UtcNow).Milliseconds)
            minSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).Milliseconds - 10000;

        if (maxSubmissionTimeMs > (currentRound.RoundEndsUtc - DateTime.UtcNow).Milliseconds)
            maxSubmissionTimeMs = (currentRound.RoundEndsUtc - DateTime.UtcNow).Milliseconds - 1000;

        // Get a random submission time from min and max, and submit.
        var submissionTime = _random.Next(minSubmissionTimeMs, maxSubmissionTimeMs);
        var word = FindWordToSubmit(botPlayer, game);

        Task.Delay(submissionTime);
        _communicationHandler.SubmitWord(botPlayer.Id, game.GameId, word);
    }

    private string FindWordToSubmit(Player player, GameDto game)
    {
        var currentRound = game.Rounds.Find(r => r.RoundNumber == game.CurrentRoundNumber);
        if (currentRound is null) throw new ArgumentException("CurrentRound not found");

        if (currentRound.RoundNumber == 1) return _validWords.GetRandomWord();

        var playerLetterHints = game.PlayerLetterHints.Find(e => e.Player.Id == player.Id);
        return playerLetterHints is null
            ? _validWords.GetRandomWord()
            : GetWordBasedOnPlayerLetterHints(playerLetterHints);
    }

    private string GetWordBasedOnPlayerLetterHints(PlayerLetterHintsDto playerLetterHints)
    {
        if (playerLetterHints.Correct.Count <= 0) return _validWords.GetRandomWord();

        var word = _solutions.GetWordBasedOnCorrectLetters(playerLetterHints.Correct);
        return word;
    }
}