using Backend.BotPlayerService.Models;
using Backend.GameService.Models.Dto;
using Backend.GameService.Models.Enums;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGame
{
    List<Player> Players { get; }
    Player? HostPlayer { get; }
    GameConfig Config { get; }
    GameViewEnum GameViewEnum { get; set; }
    int CurrentRoundNumber { get; }
    string? Solution { get; set; }
    List<RoundSubmission> RoundSubmissions { get; }
    List<string> CurrentConnections { get; set; }
    Round? CurrentRound { get; }
    List<Player> BotPlayers { get; }
    string GameId { get; set; }
    Task StartGame();
    GameDto GetDto();
    void AddRoundSubmission(Player player, string word);
    void AddPlayerLetterHints(Player player);
    void AddPlayer(Player player, bool isHostPlayer = false);
    Player? FindPlayer(string playerId);
    void AddPlayerConnection(Player player, string connectionId);
    void RemovePlayer(string connectionId);
}

public class Game : IGame
{
    public const GameTypeEnum GameType = GameTypeEnum.Public;
    private readonly BotPlayerHandler _botPlayerHandler;
    private readonly ScoreCalculator _calculator;
    private readonly IGamePublisher _publisher;

    public Game(IGamePublisher publisher, BotPlayerHandler botPlayerHandler, ScoreCalculator calculator)
    {
        _publisher = publisher;
        _botPlayerHandler = botPlayerHandler;
        _calculator = calculator;
    }

    public List<Player> Players { get; } = new();
    public Player? HostPlayer { get; private set; }
    public GameConfig Config { get; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    private DateTime? StartedAtUtc { get; set; }
    private DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; private set; }
    public string? Solution { get; set; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    private List<Round> Rounds { get; } = new();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public List<string> CurrentConnections { get; set; } = new();
    public Round? CurrentRound { get; private set; }

    public List<Player> BotPlayers
    {
        get { return Players.Where(p => p.IsBot).ToList(); }
    }

    public string GameId { get; set; } = Utils.GenerateGameId();

    public async Task StartGame()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameViewEnum = GameViewEnum.Started;
        StartedAtUtc = DateTime.UtcNow;

        await _publisher.PublishUpdatedGame(this);


        Console.WriteLine($"Game has started! Solution: {Solution}");
        //todo: Find out how to add logger without DI
        // _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,Solution);


        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            // If game is solved, or abandoned, we dont want to continue with next rounds
            if (GameViewEnum is GameViewEnum.Solved or GameViewEnum.Abandoned) continue;
            await RunRound(roundNumber);
        }

        if (GameViewEnum == GameViewEnum.Abandoned) return;
        await SetGameEnded();
    }


    private async Task RunRound(int roundNumber)
    {
        CurrentRoundNumber = roundNumber;


        var round = new Round().SetRoundOptions(CurrentRoundNumber, Config.RoundLengthSeconds,
            Config.RoundSummaryLengthSeconds);
        Rounds.Add(round);
        CurrentRound = round;

        if (BotPlayers.Count > 0) _botPlayerHandler.RequestBotsRoundSubmission(this);
        await _publisher.PublishUpdatedGame(this);

        await round.PlayRound();
        await round.ShowSummary();
    }


    public GameDto GetDto()
    {
        if (HostPlayer is null) throw new ArgumentNullException();

        var roundsDto = Rounds.Select(r => r.GetDto()).ToList();
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, roundsDto, RoundSubmissions, PlayerLetterHints);
    }


    private async Task SetGameEnded()

    {
        if (GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        await _publisher.PublishUpdatedGame(this);
    }

    public void AddRoundSubmission(Player player, string word)
    {
        var isCorrect = _calculator.IsCorrectWord(word);
        var evaluation = _calculator.CalculateLetterEvaluations(word);

        var roundSubmission = new RoundSubmission(player, CurrentRoundNumber, word, DateTime.UtcNow, evaluation,
            isCorrect);
        RoundSubmissions.Add(roundSubmission);

        if (isCorrect && GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.Solved;
    }


    public void AddPlayerLetterHints(Player player)
    {
        var playerLetterHints = new PlayerLetterHints(this, player);
        playerLetterHints.CalculatePlayerLetterHints();

        var existingHints = PlayerLetterHints.FirstOrDefault(e => e.Player == player);
        if (existingHints is not null) PlayerLetterHints.Remove(existingHints);
        PlayerLetterHints.Add(new PlayerLetterHintsDto(player, playerLetterHints.Correct,
            playerLetterHints.WrongPosition, playerLetterHints.Wrong, playerLetterHints.RoundNumber));
    }

    public void AddPlayer(Player player, bool isHostPlayer = false)
    {
        Players.Add(player);
        if (isHostPlayer) HostPlayer = player;
        //todo:  Also, check if game is full. If so, trigger game start event.
    }

    public Player? FindPlayer(string playerId)
    {
        return Players.FirstOrDefault(e => e.Id == playerId);
    }

    public void AddPlayerConnection(Player player, string connectionId)
    {
        player.ConnectionId = connectionId;
        CurrentConnections.Add(connectionId);
    }

    public void RemovePlayer(string connectionId)
    {
        CurrentConnections.Remove(connectionId);
        var player = Players.FirstOrDefault(e => e.ConnectionId == connectionId);
        if (player is not null) Players.Remove(player);
    }
}