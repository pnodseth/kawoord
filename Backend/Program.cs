using Backend;
using Backend.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((_) => true)
                .AllowCredentials();
        });
});
builder.Services.AddTransient<GameRepository>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<Dictionary<string, List<PlayerConnection>>>();
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/create", async (GameRepository repository, string playerName) =>
{
    var gameId = await new GamePlayerHandler(repository).CreateGame(new Player(playerName));
    return gameId;
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/add/:playerName", async (string playerName, string gameId) =>
{
    var result = await new GamePlayerHandler(new GameRepository()).AddPlayer(new Player(playerName), gameId);
    return result;
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/start/:gameId", async (string playerName, string gameId) =>
{
    var result = new GameHandler(gameId).Start();
    return result;
});


app.MapHub<Hub>("/gameplay");

app.Run();

public class GameHandler
{
    private readonly string _gameId;

    public GameHandler(string gameId)
    {
        _gameId = gameId;
    }

    public string Start()
    {
        return "";
        //var game = _repo.Get(_gameId);
        //Todo: set relevant game states
        // Todo: Broadcast to players that game is about to start.
        //https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/hubs-api-guide-server#callfromoutsidehub
    }
}
