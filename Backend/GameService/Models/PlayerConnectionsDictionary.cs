namespace Backend.GameService.Models;

public class PlayerConnectionsDictionary
{
    private readonly Dictionary<string, IGame> _playerConnections = new();


    public bool AddPlayerConnection(string connectionId, IGame game)
    {
        Console.WriteLine($"number of connections: {_playerConnections.Count}");
        return _playerConnections.TryAdd(connectionId, game);
    }

    public bool RemovePlayerConnection(string connectionId)
    {
        return _playerConnections.Remove(connectionId);
    }

    public IGame? GetGameFromConnectionId(string connId)
    {
        return _playerConnections.GetValueOrDefault(connId);
    }
}