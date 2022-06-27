namespace Backend.GameService.Models;

public interface IConnectionsDictionary
{
    void AddPlayerConnection(string connectionId, IGame game);
    void RemovePlayerConnection(string id, string connectionId);
    IGame? GetGameFromConnectionId(string connId);
    int PlayersConnectedCount(string gameId);
    void RemoveAllGameConnections(string gameId);
}

public class ConnectionsDictionary : IConnectionsDictionary
{
    private readonly Dictionary<string, List<string>> _gameConnectionsDictionary = new();
    private readonly Dictionary<string, IGame> _playerConnectionsDictionary = new();


    public void AddPlayerConnection(string connectionId, IGame game)
    {
        Console.WriteLine($"number of connections: {_playerConnectionsDictionary.Count}");

        _playerConnectionsDictionary.TryAdd(connectionId, game);

        var gameConnections = _gameConnectionsDictionary.GetValueOrDefault(game.GameId);
        if (gameConnections is not null)
        {
            gameConnections.Add(connectionId);
        }
        else
        {
            gameConnections = new List<string> {connectionId};
            _gameConnectionsDictionary.Add(game.GameId, gameConnections);
        }
    }

    public void RemovePlayerConnection(string gameId, string connectionId)
    {
        _playerConnectionsDictionary.Remove(connectionId);
        var gameConnections = _gameConnectionsDictionary.GetValueOrDefault(gameId);
        gameConnections?.Remove(connectionId);
        //todo: broadcast to game that user disconnected
    }

    public IGame? GetGameFromConnectionId(string connId)
    {
        return _playerConnectionsDictionary.GetValueOrDefault(connId);
    }

    public int PlayersConnectedCount(string gameId)
    {
        var gameConnections = _gameConnectionsDictionary.GetValueOrDefault(gameId);
        return gameConnections?.Count ?? 0;
    }

    public void RemoveAllGameConnections(string gameId)
    {
        _gameConnectionsDictionary.Remove(gameId);
    }
}