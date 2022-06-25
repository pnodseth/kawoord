namespace Backend.GameService.Models;

public class GamePool
{
    public List<Round> CurrentRounds { get; } = new();
    public List<Game> CurrentGames { get; } = new();

    public void AddGame(Game game)
    {
        CurrentGames.Add(game);
    }

    public void RemoveGame(Game game)
    {
        CurrentGames.Remove(game);
    }

    public void AddRound(Round round)
    {
        CurrentRounds.Add(round);
    }

    public void RemoveRound(Round round)
    {
        CurrentRounds.Remove(round);
    }
}