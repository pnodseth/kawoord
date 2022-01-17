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
builder.Services.AddSingleton<GameRepository>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<Dictionary<string, List<PlayerConnection>>>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddTransient<GamePlayerHandler>();
builder.Services.AddTransient<GameService>();
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/create", async (GamePlayerHandler gamePlayerHandler, string playername) =>
{
    var gameId = await gamePlayerHandler.CreateGame(new Player(playername));
    return gameId;
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/add", async (GamePlayerHandler gamePlayerHandler, string playername, string gameId) =>
{
    var result = await gamePlayerHandler.AddPlayer(new Player(playername), gameId);
    return result;
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/start/:gameId", async (string playerName, string gameId) =>
{
    var result = new GameService().Start();
    return result;
});


app.MapHub<Hub>("/gameplay");

app.Run();