namespace Backend.GameService.Models;

public interface IGameConfig
{
    int PreGameCountdownSeconds { get; set; }
    int MaxPlayers { get; set; }
    int WordLength { get; set; }
    int NumberOfRounds { get; set; }
    bool Public { get; set; }
    int RoundLengthSeconds { get; set; }
    int RoundSummaryLengthSeconds { get; set; }
    Language Language { get; set; }
    int PreRoundCountdownSeconds { get; }
}

public class GameConfig : IGameConfig
{
    public int PreGameCountdownSeconds { get; set; } = 5;
    public int MaxPlayers { get; set; } = 4;
    public int WordLength { get; set; } = 5;
    public int NumberOfRounds { get; set; } = 6;
    public bool Public { get; set; }
    public int RoundLengthSeconds { get; set; } = 60;
    public int RoundSummaryLengthSeconds { get; set; } = 10;
    public Language Language { get; set; } = Language.English;
    public int PreRoundCountdownSeconds { get; } = 4;
}