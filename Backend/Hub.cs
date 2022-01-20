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
    
    public async Task ConnectToGame(string gameId, string playerName)
    {
        Console.WriteLine($"Player connected: {playerName}");
        // Add player to socket game group
        //Todo: Save to dictionary, to keep track of playerName and connectionId in case of disconnects
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        // var game = await _gamePlayerHandler.FindGame(gameId);
        // await Clients.Group(gameId).SendAsync("game-player-join", playerName, game);
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