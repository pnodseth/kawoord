namespace Backend;

public class Game
{
    public Game(Config config)
    {
        Config = config;
        SolutionWord = GenerateSolutionoWord();
    }

    public List<Player> Players { get; set; } = new List<Player>();
    public Config Config { get; }
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

public class Player
{
    public Player(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class Config
{
    public Config(int maxPlayers, int wordLength, int numberOfRounds, bool allowRandomPlayers, int roundLength)
    {
        MaxPlayers = maxPlayers;
        WordLength = wordLength;
        NumberOfRounds = numberOfRounds;
        AllowRandomPlayers = allowRandomPlayers;
        RoundLength = roundLength;
        InviteCode = GenerateInviteCode();
    }

    private int MaxPlayers { get;  }
    public int WordLength { get;  }
    public int NumberOfRounds { get;  }
    public string InviteCode { get;  }
    public bool AllowRandomPlayers { get;  }
    public int RoundLength { get; }

    static string GenerateInviteCode()
    {
        return "XF652S";
    }
}

public class RoundSubmission
{
    public RoundSubmission(Player player, int round, string submittedWord, DateTime submittedTime)
    {
        Player = player;
        Round = round;
        SubmittedWord = submittedWord;
        SubmittedTime = submittedTime;
    }

    public Player Player { get; }
    public int Round { get; }
    public string SubmittedWord { get; }
    public DateTime SubmittedTime { get; }

    public int CalculateScore()
    {
        return 10;
    }

    public bool IsCorrectWord()
    {
        return false;
    }
}

public class CorrectPlayerLetter
{
    public CorrectPlayerLetter(string letter, int solutionWordIndex, CorrectLetterEnum type, int round)
    {
        Letter = letter;
        SolutionWordIndex = solutionWordIndex;
        Type = type;
        Round = round;
    }

    public string Letter { get; }
    public int SolutionWordIndex { get; }
    public CorrectLetterEnum Type { get; }
    public int Round { get; }
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