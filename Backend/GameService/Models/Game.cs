using Backend.BotPlayerService.Models;
using Backend.GameService.Models.Dto;
using Backend.GameService.Models.Enums;
using Backend.GameService.Providers;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGame
{
    string Solution { get; }
    string GameId { get; }
    List<IPlayer> Players { get; }
    IPlayer? HostPlayer { get; }
    IGameConfig Config { get; }
    GameViewEnum GameViewEnum { get; set; }
    int CurrentRoundNumber { get; }
    List<RoundSubmission> RoundSubmissions { get; }
    IRound? CurrentRound { get; }
    List<IPlayer> BotPlayers { get; }
    List<PlayerLetterHintsDto> PlayerLetterHints { get; }
    DateTime? EndedAtUtc { get; set; }
    DateTime? StartedAtUtc { get; set; }
    int PlayerCount { get; }
    Task RunGame();
    GameDto GetDto();
    void AddRoundSubmission(IPlayer player, string word);
    void AddPlayerLetterHints(IPlayer player);
    void AddPlayer(IPlayer player, bool isHostPlayer = false);
    IPlayer? FindPlayer(string playerId);
    void RemovePlayer(IPlayer player);
    int GetCurrentRoundSubmissionsCount();
    void SetPublic(bool isPublic);
    IPlayer? FindPlayerWithConnectionId(string connectionId);
}

public class Game : IGame
{
    private readonly IBotPlayerHandler _botPlayerHandler;
    private readonly IScoreCalculator _calculator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<IGame> _logger;
    private readonly IGamePublisher _publisher;

    public Game(IGamePublisher publisher, IBotPlayerHandler
            botPlayerHandler, IScoreCalculator calculator,
        ISolutionWords solutionWords, ILogger<Game> logger, IDateTimeProvider dateTimeProvider, IGameConfig gameConfig)
    {
        _publisher = publisher;
        _botPlayerHandler = botPlayerHandler;
        _calculator = calculator;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        Solution = solutionWords.GetRandomSolution();
        GameId = GenerateGameId();
        Config = gameConfig;
    }

    private List<Round> Rounds { get; } = new();
    private List<IPlayer> Winners { get; } = new();
    public IGameConfig Config { get; }

    public int PlayerCount => Players.Count;

    public DateTime? StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public string GameId { get; set; }
    public List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public string Solution { get; }
    public List<IPlayer> Players { get; } = new();
    public IPlayer? HostPlayer { get; private set; }
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    public int CurrentRoundNumber { get; private set; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public IRound? CurrentRound { get; private set; }

    public List<IPlayer> BotPlayers
    {
        get { return Players.Where(p => p.IsBot).ToList(); }
    }


    public async Task RunGame()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameViewEnum = GameViewEnum.Started;
        StartedAtUtc = _dateTimeProvider.GetNowUtc();

        _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,
            Solution);

        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            // If game is solved, or abandoned, we dont want to continue with next rounds
            if (GameViewEnum is GameViewEnum.Solved or GameViewEnum.Abandoned) continue;

            await RunRound(roundNumber);
        }

        if (GameViewEnum == GameViewEnum.Abandoned) return;
        SetGameEnded();
    }


    public GameDto GetDto()
    {
        if (HostPlayer is null) throw new ArgumentNullException();

        var roundsDto = Rounds.Select(r => r.GetDto()).ToList();
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedAtUtc, CurrentRoundNumber, roundsDto, RoundSubmissions, PlayerLetterHints, Config.MaxPlayers);
    }

    public IPlayer? FindPlayer(string playerId)
    {
        return Players.FirstOrDefault(e => e.Id == playerId);
    }

    public void RemovePlayer(IPlayer player)
    {
        Players.Remove(player);
    }


    public void AddPlayerLetterHints(IPlayer player)
    {
        var playerLetterHints = new PlayerLetterHints(this, player);
        playerLetterHints.CalculatePlayerLetterHints();

        var existingHints = PlayerLetterHints.FirstOrDefault(e => e.Player == player);
        if (existingHints is not null) PlayerLetterHints.Remove(existingHints);
        PlayerLetterHints.Add(new PlayerLetterHintsDto(player, playerLetterHints.Correct,
            playerLetterHints.WrongPosition, playerLetterHints.Wrong, playerLetterHints.RoundNumber));
    }

    public void AddRoundSubmission(IPlayer player, string word)
    {
        var isCorrect = _calculator.IsCorrectWord(Solution, word);
        if (isCorrect) Winners.Add(player);

        var evaluation = _calculator.CalculateLetterEvaluations(this, word);

        var roundSubmission = new RoundSubmission(player, CurrentRoundNumber, word, DateTime.UtcNow, evaluation,
            isCorrect);
        RoundSubmissions.Add(roundSubmission);
    }

    public void AddPlayer(IPlayer player, bool isHostPlayer = false)
    {
        Players.Add(player);
        if (isHostPlayer) HostPlayer = player;

        //todo:  Also, check if game is full. If so, trigger game start event.
    }

    public int GetCurrentRoundSubmissionsCount()
    {
        return RoundSubmissions.Where(e => e.RoundNumber == CurrentRoundNumber).ToList().Count;
    }

    public void SetPublic(bool isPublic)
    {
        Config.Public = isPublic;
    }


    public IPlayer? FindPlayerWithConnectionId(string connectionId)
    {
        return Players.Find(e => e.ConnectionId == connectionId);
    }


    public void RemovePlayerWithConnectionId(string connectionId)
    {
        var player = Players.FirstOrDefault(e => e.ConnectionId == connectionId);
        if (player is not null) Players.Remove(player);
    }


    private async Task RunRound(int roundNumber)
    {
        CurrentRoundNumber = roundNumber;


        var round = new Round()
            .SetRoundOptions(CurrentRoundNumber, Config.RoundLengthSeconds,
                Config.RoundSummaryLengthSeconds, Config.PreRoundCountdownSeconds)
            .SetPublisher(GameId, _publisher);
        if (BotPlayers.Count > 0) round.SetBotPlayerHandler(_botPlayerHandler);

        Rounds.Add(round);
        CurrentRound = round;


        await round.PreRoundCountdown();
        await round.PlayRound();
        await round.ShowSummary();

        if (Winners.Count > 0)
        {
            GameViewEnum = GameViewEnum.Solved;
            _publisher.PublishUpdatedGame(this);
        }
    }


    private void SetGameEnded()

    {
        if (GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.EndedUnsolved;
        EndedAtUtc = _dateTimeProvider.GetNowUtc();

        _publisher.PublishUpdatedGame(this);
    }

    private string GenerateGameId()
    {
        var random = new RandomProvider();


        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.RandomFromMax(s.Length)]).ToArray());
    }
}