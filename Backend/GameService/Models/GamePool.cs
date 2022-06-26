namespace Backend.GameService.Models;

//TODO: Replace with either Redis or in memory database

public class GamePool
{
    public List<IGame> CurrentGames { get; } = new();

    public void AddGame(Game game)
    {
        CurrentGames.Add(game);
    }

    public void RemoveGame(IGame game)
    {
        CurrentGames.Remove(game);
    }
}