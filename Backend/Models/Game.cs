using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

[BsonIgnoreExtraElements]
public class Game
{
    public List<Player> Players { get; set; } = new List<Player>();
    public Player HostPlayer { get; set; }
    public GameConfig GameConfig { get; set; }
    public GameState State { get; set; } = GameState.Lobby;
    public DateTime? StartedTime { get; set; }
    public DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; } = 1;
    public string GameId { get; set; } = "";
    public string Solution { get; set; } = "";
    public List<RoundSubmission> RoundSubmissions { get; set; } = new List<RoundSubmission>();

    public Game(GameConfig gameConfig, string gameId, string solution, Player hostPlayer)
    {
        GameConfig = gameConfig;
        GameId = gameId;
        Solution = solution;
        HostPlayer = hostPlayer;
        Players.Add(hostPlayer);
    }
}

public enum Language
{
    Norwegian,
    English
}

public enum CorrectLetterEnum
{
    CorrectPlacement,
    WrongPlacement
}

public enum GameStateEnum
{
    Lobby,
    Starting,
    Started,
    Ended
}

public class GameState
{
    private GameState(string value) { Value = value; }

    public string Value { get; private set; }

    public static GameState Lobby   { get { return new GameState("Lobby"); } }
    public static GameState Starting   { get { return new GameState("Starting"); } }
    public static GameState Started    { get { return new GameState("Started"); } }
    public static GameState Ended { get { return new GameState("Ended"); } }
}