namespace Backend.GameService.Models;

//TODO: Replace with either Redis or in memory database

public class GamePool
{
    public List<IGame> CurrentGames { get; } = new();

    public void AddGame(IGame game)
    {
        CurrentGames.Add(game);
    }

    public IGame? FindGame(string gameId)
    {
        var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        return game;
    }

    public void RemoveGame(IGame game)
    {
        CurrentGames.Remove(game);
    }
}