using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

[BsonIgnoreExtraElements]
public class Game
{
    public Game(GameConfig config, string gameId, string solution, Player hostPlayer)
    {
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
    public string GameId { get; set; }
    public string Solution { get; set; }
    public List<RoundSubmission> RoundSubmissions { get; set; } = new();
    public List<RoundInfo> RoundInfos { get; set; } = new();
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