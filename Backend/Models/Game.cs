using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

[BsonIgnoreExtraElements]
public class Game
{
    public List<Player> Players { get; set; } = new List<Player>();
    public Player HostPlayer { get; set; }
    public GameConfig Config { get; set; }
    public GameState State { get; set; } = GameState.Lobby;
    public DateTime? StartedAtUTC { get; set; }
    public DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; } = 0;
    public RoundState CurrentRoundState { get; set; } = RoundState.NotStarted;
    public string GameId { get; set; } = "";
    public string Solution { get; set; } = "";
    public List<RoundSubmission> RoundSubmissions { get; set; } = new ();
    public GameStats? GameStats { get; set; }
    public List<RoundInfo> RoundInfos { get; set; } = new();

    public Game(GameConfig config, string gameId, string solution, Player hostPlayer)
    {
        Config = config;
        GameId = gameId;
        Solution = solution;
        HostPlayer = hostPlayer;
        Players.Add(hostPlayer);
    }
}

public class GameStats
{
    public List<WinnerSubmission> Winners { get; set; } = new();
    public int RoundCompleted { get; set; }
}

public record WinnerSubmission(Player Player, DateTime SubmittedDateTime);

public enum Language
{
    Norwegian,
    English
}

public class GameState
{
    private GameState(string value) { Value = value; }

    public string Value { get; private set; }

    public static GameState Lobby   { get { return new GameState("Lobby"); } }
    public static GameState Starting   { get { return new GameState("Starting"); } }
    public static GameState Started    { get { return new GameState("Started"); } }
    public static GameState EndedUnsolved { get { return new GameState("EndedUnsolved"); } }
    public static GameState Solved { get { return new GameState("Solved"); } }
}

public class RoundState
{
    public RoundState(string value) { Value = value; }

    public string Value { get; private set; }
    
    public static RoundState NotStarted { get { return new RoundState("NotStarted"); } }
    public static RoundState Started   { get { return new RoundState("Playing"); } }
    public static RoundState PlayerSubmitted   { get { return new RoundState("PlayerSubmitted"); } }
    public static RoundState Summary   { get { return new RoundState("Summary"); } }
    public static RoundState Solved    { get { return new RoundState("Solved"); } }
}