using System.Collections.Concurrent;

namespace Backend.GameService.Models;

public interface IPublicGamesQueue
{
    IGame? GetFirstAvailableGame();
    void AddToPublicGamesQueue(IGame game);
    void RemoveFromPublicGamesQueue(IGame value);
}

public class PublicGamesQueue : IPublicGamesQueue
{
    private readonly ILogger<PublicGamesQueue> _logger;
    private ConcurrentQueue<IGame> _publicGames = new();

    public PublicGamesQueue(ILogger<PublicGamesQueue> logger)
    {
        _logger = logger;
    }

    public void RemoveFromPublicGamesQueue(IGame game)
    {
        var exists = _publicGames.Contains(game);
        if (exists)
        {
            _publicGames = new ConcurrentQueue<IGame>(_publicGames.Where(e => e.GameId != game.GameId));
            _logger.LogInformation("Removed game with id {ID} from PublicGameQueue ", game.GameId);
        }
    }

    public void AddToPublicGamesQueue(IGame game)
    {
        _publicGames.Enqueue(game);
        _logger.LogInformation("Added game with id {ID} to PublicGameQueue ", game.GameId);
    }

    public IGame? GetFirstAvailableGame()
    {
        var res = _publicGames.TryPeek(out var game);
        if (!res)
        {
            _logger.LogInformation("Looking for available games in PublicGameQueue, queue is empty");
            return null;
        }

        /* If game will be full after player joins, dequeue it*/
        if (game?.PlayerAndBotCount + 1 == game?.Config.MaxPlayers)
        {
            _publicGames.TryDequeue(out game);
            _logger.LogInformation("Public game with id {game.gameId} is now full, dequeuing it", game?.GameId);
        }

        return game;
    }
}