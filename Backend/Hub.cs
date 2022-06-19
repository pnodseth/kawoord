using Backend.Services;

namespace Backend;

public class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly GameHandler _gameHandler;

    // Groups: https://docs.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-6.0

    public Hub(GameHandler gameHandler)
    {
        _gameHandler = gameHandler;
    }

    public async Task ConnectToGame(string gameId, string playerName, string playerId)
    {
        Console.WriteLine($"{playerName} connected with id: {playerId}");
        // Add player to socket game group
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _gameHandler.AddPlayerConnectionId(gameId, playerId, Context.ConnectionId);
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Todo: Handle disconnects. Remove 
        // Find gameId from dictionary, and broadcast to game that user disconnected
        // var player = dictionary[Context.ConnectionId].GetValue()
        //Clients.Group(player.gameId).SendAsync("game-player-disconnect", player.playerName)

        return base.OnDisconnectedAsync(exception);
    }
}