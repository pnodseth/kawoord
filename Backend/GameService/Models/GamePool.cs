using Microsoft.Extensions.Caching.Memory;

namespace Backend.GameService.Models;

public interface IGamePool
{
    List<IGame> CurrentGames { get; }
    void AddGame(IGame game);
    IGame? FindGame(string gameId);
    void RemoveGame(string gameId);
}

public class GamePool : IGamePool
{
    private readonly IMemoryCache _memoryCache;

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
}