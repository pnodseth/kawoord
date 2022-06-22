using Backend.GameService.Models;

namespace Backend;

public class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly GameHandler _gameHandler;
    private readonly ILogger<Hub> _logger;

    // Groups: https://docs.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-6.0

    public Hub(GameHandler gameHandler, ILogger<Hub> logger)
    {
        _gameHandler = gameHandler;
        _logger = logger;
    }

    public async Task ConnectToGame(string gameId, string playerName, string playerId)
    {
        // Add player to socket game group
        Context.Items.Add(Context.ConnectionId, gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _gameHandler.AddPlayerConnectionId(gameId, playerId, Context.ConnectionId);
        _logger.LogInformation("Player {Player} connected to game {Game} at {Time}", playerName, gameId,
            DateTime.UtcNow);
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var gameId = Context.Items[Context.ConnectionId] as string;
        if (gameId is not null) Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

        _gameHandler.HandleDisconnectedPlayer(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}