using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

[BsonIgnoreExtraElements]
public class Game
{
    public List<Player> Players { get; set; } = new List<Player>();
    public Player HostPlayer { get; set; }
    public GameConfig GameConfig { get; set; }
    public GameStateEnum State { get; set; } = GameStateEnum.Created;
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
    Created,
    Started,
    Ended
}