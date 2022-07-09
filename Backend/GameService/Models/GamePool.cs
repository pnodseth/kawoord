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
    private readonly IPublicGamesQueue _publicGamesQueue;

    public GamePool(IMemoryCache memoryCache, IPublicGamesQueue publicGamesQueue)
    {
        _memoryCache = memoryCache;
        _publicGamesQueue = publicGamesQueue;
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
        var exists = _memoryCache.TryGetValue(gameId, out IGame game);
        if (exists)
        {
            _memoryCache.Remove(gameId);

            if (game.Config.Public) _publicGamesQueue.RemoveGame(game);
        }
    }

    public void AddToAvailableGames(IGame game)
    {
        _publicGamesQueue.AddToAvailableGames(game);
    }

    public IGame? GetFirstAvailableGame()
    {
        var game = _publicGamesQueue.GetFirstAvailableGame();
        return game;
    }
}