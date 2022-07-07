using Backend.GameService.Models.Enums;

namespace Backend.GameService.Models;

public interface IConnectionsHandler
{
    public void HandleDisconnectedPlayer(string connectionId);
    public void AddPlayerConnectionId(string gameId, string playerId, string connectionId);
    void RemoveGameConnections(string gameId);
}

public class ConnectionsHandler : IConnectionsHandler
{
    private readonly IConnectionsDictionary _connectionsDictionary;
    private readonly IGamePool _gamePool;
    private readonly ILogger<ConnectionsHandler> _logger;
    private readonly IGamePublisher _publisher;

    public ConnectionsHandler(IGamePool gamePool, ILogger<ConnectionsHandler> logger,
        IConnectionsDictionary connectionsDictionary, IGamePublisher publisher)
    {
        _gamePool = gamePool;
        _logger = logger;
        _connectionsDictionary = connectionsDictionary;
        _publisher = publisher;
    }

    public void AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = _gamePool.FindGame(gameId);
        if (game != null)
        {
            var player = game.FindPlayer(playerId);
            if (player is null)
            {
                _logger.LogWarning("No player found for playerId: {PlayerId} in game: {Game}", playerId, game.GameId);
            }
            else
            {
                player.ConnectionId = connectionId;
                _connectionsDictionary.AddPlayerConnection(connectionId, game);
            }
        }
        else
        {
            _logger.LogWarning("Player with connection id {ConnId} connected, but no game with id {GameId} was found",
                connectionId, gameId);
        }
    }

    public void RemoveGameConnections(string gameId)
    {
        _connectionsDictionary.RemoveAllGameConnections(gameId);
    }


    public void HandleDisconnectedPlayer(string connectionId)
    {
        var game = _connectionsDictionary.GetGameFromConnectionId(connectionId);
        if (game is null)
        {
            _logger.LogWarning("No game was found for {Conn}", connectionId);
            return;
        }

        var player = game.FindPlayerWithConnectionId(connectionId);

        if (player is null)
        {
            _logger.LogWarning("No Player with connection id {ConnectionId} found", connectionId);
            return;
        }

        game.RemovePlayer(player);
        _connectionsDictionary.RemovePlayerConnection(game.GameId, connectionId);

        _publisher.PublishPlayerDisconnected(game, player);
        _publisher.PublishUpdatedGame(game);

        if (game.PlayerCount > 0) return;

        _logger.LogInformation("No more connected clients. Game is set to abandoned");
        game.GameViewEnum = GameViewEnum.Abandoned;


        _gamePool.RemoveGame(game.GameId);
    }
}