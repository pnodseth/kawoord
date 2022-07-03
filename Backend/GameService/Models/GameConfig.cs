namespace Backend.GameService.Models;

public class GameConfig
{
    public int PreGameCountdownSeconds { get; set; } = 5;
    public int MaxPlayers { get; set; } = 4;
    public int WordLength { get; set; } = 5;
    public int NumberOfRounds { get; set; } = 6;
    public bool Public { get; set; } = false;
    public int RoundLengthSeconds { get; set; } = 60;
    public int RoundSummaryLengthSeconds { get; set; } = 10;
    public Language Language { get; set; } = Language.English;
    public int PreRoundCountdownSeconds { get; } = 4;
}