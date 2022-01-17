namespace Backend.Models;

public class GameConfig
{
    public GameConfig(int maxPlayers, int wordLength, int numberOfRounds, bool allowRandomPlayers, int roundLength, Language language)
    {
        if (numberOfRounds > 8 | numberOfRounds <= 2)
        {
            throw new ArgumentOutOfRangeException("Number of rounds must be  between 2 and 8");
        }

        if (wordLength < 3 | wordLength > 8)
        {
            throw new ArgumentOutOfRangeException("wordLength must be between 3 and 8 characters");
        }

        if (maxPlayers < 2 | maxPlayers > 10)
        {
            throw new ArgumentOutOfRangeException("maxPlayers must be between 2 and 10");
        }

        if (roundLength < 30 | roundLength > 240)
        {
            throw new ArgumentOutOfRangeException("roundLength must be between 30 and 240 seconds");
        }
        
        MaxPlayers = maxPlayers;
        WordLength = wordLength;
        NumberOfRounds = numberOfRounds;
        AllowRandomPlayers = allowRandomPlayers;
        RoundLength = roundLength;
        Language = language;
    }

    private int MaxPlayers { get; set; }
    public int WordLength { get; set; }
    public int NumberOfRounds { get; set; }
    public bool AllowRandomPlayers { get; set; }
    public int RoundLength { get; set; }
    public Language Language { get; set; }
}