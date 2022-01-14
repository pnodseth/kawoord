namespace Backend;

public class GameConfig
{
    public GameConfig(int maxPlayers, int wordLength, int numberOfRounds, bool allowRandomPlayers, int roundLength, Language language)
    {
        MaxPlayers = maxPlayers;
        WordLength = wordLength;
        NumberOfRounds = numberOfRounds;
        AllowRandomPlayers = allowRandomPlayers;
        RoundLength = roundLength;
        Language = language;
        InviteCode = GenerateInviteCode();
    }

    private int MaxPlayers { get;  }
    public int WordLength { get;  }
    public int NumberOfRounds { get;  }
    public string InviteCode { get;  }
    public bool AllowRandomPlayers { get;  }
    public int RoundLength { get; }
    public Language Language { get; set; }

    static string GenerateInviteCode()
    {
        return "XF652S";
    }
}