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
    private readonly IHubContext<Hub> _hubContext;

    public Game(GameConfig config, string gameId, string solution, Player hostPlayer, IHubContext<Hub> hubContext)
    {
        _hubContext = hubContext;
        Config = config;
        GameId = gameId;
        Solution = solution;
        HostPlayer = hostPlayer;
        Players.Add(hostPlayer);
    }

    public List<Player> Players { get; set; } = new();
    public Player HostPlayer { get; set; }
    public GameConfig Config { get; set; }
    public GameStateEnum GameStateEnum { get; set; } = GameStateEnum.Lobby;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; }
    public RoundStateEnum RoundStateEnum { get; } = RoundStateEnum.NotStarted;
    public string Solution { get; set; }
    public List<RoundSubmission> RoundSubmissions { get; set; } = new();
    public List<RoundInfo> RoundInfos { get; set; } = new();
    public List<Round> Rounds { get; set; } = new();
    public string GameId { get; set; }


    public async Task Start()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameStateEnum = GameStateEnum.Started;
        StartedAtUtc = DateTime.UtcNow;
        Persist();

        await _hubContext.Clients.Group(GameId).SendAsync("gamestate", GameStateEnum.Value,
            new GameDto(Players, HostPlayer, GameId, GameStateEnum, StartedAtUtc,
                EndedTime,
                CurrentRoundNumber, RoundInfos, RoundStateEnum));

        Console.WriteLine($"Game has started! Solution: {Solution}");


        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            if (GameStateEnum.Value == GameStateEnum.Solved.Value)
            {
                Console.WriteLine("Game is solved, skipping round");
                continue;
            }

            var round = new Round(_hubContext, this, roundNumber);
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

    private async Task GameEnded()

    {
        Console.WriteLine("Going to GameEnded");
        if (GameStateEnum.Value != GameStateEnum.Solved.Value) GameStateEnum = GameStateEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        Persist();

        // Send game status update
        var updatedGame = new GameDto(Players, HostPlayer, GameId, GameStateEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, RoundInfos, RoundStateEnum);

        await _hubContext.Clients.Group(GameId).SendAsync("gamestate", GameStateEnum.Value, updatedGame);

        // send round evaluations
        var roundEvaluations = RoundSubmissions.Where(r => r.Round == CurrentRoundNumber)
            .Select(e => new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc,
                CurrentRoundNumber)).ToList();


        // Send Points
        await _hubContext.Clients.Group(GameId)
            .SendAsync("points", roundEvaluations);
    }
}

public class GameStateEnum
{
    private GameStateEnum(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static GameStateEnum Lobby => new("Lobby");
    public static GameStateEnum Starting => new("Starting");
    public static GameStateEnum Started => new("Started");
    public static GameStateEnum EndedUnsolved => new("EndedUnsolved");
    public static GameStateEnum Solved => new("Solved");
}

public class RoundStateEnum
{
    public RoundStateEnum(string value)
    {
        Value = value;
    }

    public string Value { get; }


    public static RoundStateEnum NotStarted => new("NotStarted");
    public static RoundStateEnum Started => new("Playing");
    public static RoundStateEnum PlayerSubmitted => new("PlayerSubmitted");
    public static RoundStateEnum Summary => new("Summary");
    public static RoundStateEnum Solved => new("Solved");
}