using Backend.GameService.Models;

namespace Backend;

public class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IConnectionsHandler _connectionsHandler;
    private readonly ILogger<Hub> _logger;

    // Groups: https://docs.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-6.0

    public Hub(ILogger<Hub> logger, IConnectionsHandler connectionsHandler)
    {
        _logger = logger;
        _connectionsHandler = connectionsHandler;
    }

    public async Task ConnectToGame(string gameId, string playerName, string playerId)
    {
        // Add player to socket game group
        Context.Items.Add(Context.ConnectionId, gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _connectionsHandler.AddPlayerConnectionId(gameId, playerId, Context.ConnectionId);
        _logger.LogInformation("Player {Player} connected to game {Game} at {Time}", playerName, gameId,
            DateTime.UtcNow);
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var gameId = Context.Items[Context.ConnectionId] as string;
        if (gameId is not null) Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

        _connectionsHandler.HandleDisconnectedPlayer(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}