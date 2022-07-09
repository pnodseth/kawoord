using Microsoft.Extensions.Caching.Memory;

namespace Backend.GameService.Models;

public interface IGamePool
{
    List<IGame> CurrentGames { get; }
    void AddGame(IGame game);
    IGame? FindGame(string gameId);
    void RemoveGame(string gameId);
    IGame? GetFirstAvailableGame();
    void AddToAvailableGames(IGame game);
}

public class GamePool : IGamePool
{
    private readonly IMemoryCache _memoryCache;
    private readonly Queue<IGame> _publicGames = new();

    public GamePool(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public List<IGame> CurrentGames { get; } = new();

    public void AddGame(IGame game)
    {
        _memoryCache.Set(game.GameId, game, TimeSpan.FromMinutes(10));
    }

    public IGame? FindGame(string gameId)
    {
        _memoryCache.TryGetValue(gameId, out Game? game);
        return game;
    }

    public void RemoveGame(string gameId)
    {
        _memoryCache.Remove(gameId);
    }

    public void AddToAvailableGames(IGame game)
    {
        _publicGames.Enqueue(game);
    }

    public IGame? GetFirstAvailableGame()
    {
        var game = _publicGames.Peek();

        /* If game will be full after player joins, dequeue it*/
        if (game.PlayerCount + 1 == game.Config.MaxPlayers) _publicGames.TryDequeue(out game);

        return game;
    }
}