using Backend.GameService.Models;
using Backend.GameService.Providers;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public interface IBotPlayerHandler
{
    Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 4000);

    void RequestBotsRoundSubmission(string gameId);
}

public class BotPlayerHandler : IBotPlayerHandler
{
    private readonly IBotPlayerGenerator _botPlayerGenerator;
    private readonly IGameHandler _gameHandler;
    private readonly IGamePool _gamePool;
    private readonly ILogger<BotPlayerHandler> _logger;
    private readonly Random _random = new();
    private readonly IRandomProvider _randomProvider;
    private readonly ISolutionWords _solutionWords;
    private readonly IValidWords _validWords;

    public BotPlayerHandler(IGameHandler gameHandler, ISolutionWords solutionWords, IValidWords validWords,
        IRandomProvider randomProvider, IBotPlayerGenerator botPlayerGenerator, IGamePool gamePool,
        ILogger<BotPlayerHandler> logger)
    {
        _gameHandler = gameHandler;
        _solutionWords = solutionWords;
        _validWords = validWords;
        _randomProvider = randomProvider;
        _botPlayerGenerator = botPlayerGenerator;
        _gamePool = gamePool;
        _logger = logger;
    }

    public async Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 4000)
    {
        var addTimeRemaining = maxTimeToLastAddedMs;

        // ONLY FOR DEV

        Console.WriteLine($"addtimeremaining: {addTimeRemaining}");
        // Add players at random intervals
        foreach (var botNumber in Enumerable.Range(1, numberOfBots))
        {
            var waitTime = botNumber == 1 ? timeToFirstAddedMs : _random.Next(addTimeRemaining);
            Console.WriteLine($"Waiting {waitTime} before adding next player");
            await Task.Delay(waitTime);

            _gameHandler.AddPlayerWithGameId(_botPlayerGenerator.GeneratePlayer(), gameId);
            addTimeRemaining -= waitTime;
        }

        Console.WriteLine("Done adding bot players");
    }

    public void RequestBotsRoundSubmission(string gameId)
    {
        var game = _gamePool.FindGame(gameId);
        if (game is null)
        {
            _logger.LogWarning("No game with id {Id} found", gameId);
            return;
        }

        if (game.BotPlayers.Count <= 0) return;
        foreach (var botPlayer in game.BotPlayers) Task.Run(async () => { await SubmitWord(botPlayer, game); });
    }

    private async Task SubmitWord(IPlayer botPlayer, IGame game)
    {
        var dateProvider = new DateTimeProvider();
        if (game.CurrentRound is null) throw new ArgumentException("CurrentRound not found");

        var word = FindWordToSubmit(botPlayer, game);

        double minSubmissionTimeMs = 4000;
        var maxSubmissionTimeMs = (game.CurrentRound.RoundEndsUtc - dateProvider.GetNowUtc()).TotalMilliseconds;

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

        if (minSubmissionTimeMs <= 0)
            minSubmissionTimeMs = 5000;
        if (maxSubmissionTimeMs <= 0)
            maxSubmissionTimeMs = 50000;

        // Get a random submission time from min and max, and submit.
        var submissionTime = _randomProvider.RandomFromMinMax(Convert.ToInt32(minSubmissionTimeMs),
            Convert.ToInt32(maxSubmissionTimeMs));


        Console.WriteLine($"Bot {botPlayer.Name} submitted: {word}");
        await Task.Delay(submissionTime);
        await _gameHandler.SubmitWord(game.GameId, botPlayer.Id, word);
    }

    private string FindWordToSubmit(IPlayer player, IGame game)
    {
        if (game.CurrentRound is null) throw new ArgumentException("CurrentRound not found");

        if (game.CurrentRound.RoundNumber == 1) return _validWords.GetRandomWord();

        var playerLetterHints = game.PlayerLetterHints.Find(e => e.Player.Id == player.Id);
        return playerLetterHints is null
            ? _validWords.GetRandomWord()
            : _solutionWords.GetWordBasedOnCorrectLetters(playerLetterHints);
    }
}