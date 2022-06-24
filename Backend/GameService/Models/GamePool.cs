namespace Backend.GameService.Models;

public class GamePool
{
    public List<Round> CurrentRounds { get; } = new();
    public List<Game> CurrentGames { get; } = new();

    public void Add(Game game)
    {
        CurrentGames.Add(game);
    }
}