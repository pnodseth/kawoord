using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class PlayerConnection
{
    public PlayerConnection(string playerName, string connectionId)
    {
        PlayerName = playerName;
        ConnectionId = connectionId;
    }

    public string PlayerName { get; set; }
    public string ConnectionId { get; set; }
}



public class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    // Groups: https://docs.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-6.0
    private readonly GamePlayerHandler _gamePlayerHandler;

    public Hub(GamePlayerHandler gamePlayerHandler)
    {
        _gamePlayerHandler = gamePlayerHandler;
    }
    
    public async Task ConnectToGame(string gameId, string playerName, string playerId)
    {
        Console.WriteLine($"{playerName} connected with id: {playerId}");
        // Add player to socket game group
        //Todo: Save to dictionary, to keep track of playerName and connectionId in case of disconnects
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await _gamePlayerHandler.AddPlayerConnectionId(gameId, playerId, Context.ConnectionId);
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