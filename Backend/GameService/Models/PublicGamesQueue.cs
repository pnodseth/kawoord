namespace Backend.GameService.Models;

public interface IPublicGamesQueue
{
    IGame? GetFirstAvailableGame();
    void AddToAvailableGames(IGame game);
    void RemoveGame(IGame value);
}

public class PublicGamesQueue : IPublicGamesQueue
{
    private Queue<IGame> _publicGames = new();

    public void RemoveGame(IGame game)
    {
        var exists = _publicGames.Contains(game);
        if (exists)
            _publicGames = new Queue<IGame>(_publicGames.Where(e => e.GameId != game.GameId));
    }

    public void AddToAvailableGames(IGame game)
    {
        _publicGames.Enqueue(game);
    }

    public IGame? GetFirstAvailableGame()
    {
        var res = _publicGames.TryPeek(out var game);
        if (!res) return null;

        if (game is not null)
            /* If game will be full after player joins, dequeue it*/
            if (game.PlayerCount + 1 == game.Config.MaxPlayers)
                _publicGames.TryDequeue(out game);

        return game;
    }
}