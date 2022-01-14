namespace Backend.Models;

public class Game
{
    public Game(GameConfig gameConfig)
    {
        GameConfig = gameConfig;
        SolutionWord = GenerateSolutionoWord();
    }

    public List<Player> Players { get; set; } = new List<Player>();
    public GameConfig GameConfig { get; }
    public GameStateEnum State { get; set; } = GameStateEnum.Created;
    public DateTime? StartedTime { get; set; }
    public DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; } = 1;
    public string SolutionWord { get; }

    public List<RoundSubmission> RoundSubmissions { get; set; } = new List<RoundSubmission>();


    static string GenerateSolutionoWord()
    {
        return "Hello";
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


