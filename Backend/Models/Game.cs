using Backend.Models.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Models;

public interface IGame
{
    string GameId { get; set; }
    Task Start();
    void Persist();
}

public class Game : IGame
{
    private IHubContext<Hub> HubContext;
    private readonly ILogger<Game> _logger;

    public Game(IHubContext<Hub> hubContext, ILogger<Game> logger)
    {
        HubContext = hubContext;
        _logger = logger;
    }

    public List<Player> Players { get; } = new();
    public Player HostPlayer { get; set; }
    public GameConfig Config { get; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    private DateTime? StartedAtUtc { get; set; }
    private DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; }
    public RoundViewEnum RoundViewEnum { get; set; } = RoundViewEnum.NotStarted;
    public string Solution { get; } = Utils.GenerateSolution();
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public List<RoundInfo> RoundInfos { get; } = new();
    public List<Round> Rounds { get; } = new();
    public string GameId { get; set; } = Utils.GenerateGameId();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();

    public async Task Start()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameViewEnum = GameViewEnum.Started;
        StartedAtUtc = DateTime.UtcNow;
        Persist();

        await HubContext.Clients.Group(GameId).SendAsync("game-update",
            GetDto());

        Console.WriteLine($"Game has started! Solution: {Solution}");
        _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,
            Solution);


        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            if (GameViewEnum.Value == GameViewEnum.Solved.Value)
            {
                Console.WriteLine("Game is solved, skipping round");
                continue;
            }

            var round = new Round(this, roundNumber);
            Rounds.Add(round);
            await round.StartRound();
        }

        await GameEnded();
    }

    public void Persist()
    {
        // var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        // if (game != null) await _repository.Update(game);
    }

    public GameDto GetDto()
    {
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, RoundInfos, RoundViewEnum, RoundSubmissions, PlayerLetterHints);
    }

    public async Task<GameDto> PublishUpdatedGame()
    {
        await HubContext.Clients.Group(GameId).SendAsync("game-update", GetDto());

        return GetDto();
    }

    private async Task GameEnded()

    {
        Console.WriteLine("Going to GameEnded");
        if (GameViewEnum.Value != GameViewEnum.Solved.Value) GameViewEnum = GameViewEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        Persist();

        // Send game status update
        var updatedGame = GetDto();
        await HubContext.Clients.Group(GameId).SendAsync("state", "solution", Solution);


        await HubContext.Clients.Group(GameId).SendAsync("game-update", updatedGame);
    }

    public void AddRoundSubmission(Player player, string word)
    {
        var isCorrect = ScoreCalculator.IsCorrectWord(this, word);

        var evaluation = ScoreCalculator.CalculateLetterEvaluations(this, word);

        var roundSubmission = new RoundSubmission(player, CurrentRoundNumber, word, DateTime.UtcNow, evaluation,
            isCorrect);
        RoundSubmissions.Add(roundSubmission);
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
}

public class GameViewEnum
{
    private GameViewEnum(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static GameViewEnum Lobby => new("Lobby");
    public static GameViewEnum Starting => new("Starting");
    public static GameViewEnum Started => new("Started");
    public static GameViewEnum EndedUnsolved => new("EndedUnsolved");
    public static GameViewEnum Solved => new("Solved");
}

public class RoundViewEnum
{
    public RoundViewEnum(string value)
    {
        Value = value;
    }

    public string Value { get; }


    public static RoundViewEnum NotStarted => new("NotStarted");
    public static RoundViewEnum Started => new("Playing");
    public static RoundViewEnum PlayerSubmitted => new("PlayerSubmitted");
    public static RoundViewEnum Summary => new("Summary");
    public static RoundViewEnum Solved => new("Solved");
}