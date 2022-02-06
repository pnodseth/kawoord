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
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<Dictionary<string, List<PlayerConnection>>>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddTransient<GamePlayerHandler>();
builder.Services.AddTransient<GameService>();
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/create", async (GamePlayerHandler gamePlayerHandler, string playerName, string playerId) =>
{
    var gameId = await gamePlayerHandler.CreateGame(playerName, playerId);
    return gameId;
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/join", async (GamePlayerHandler gamePlayerHandler, string playerName,string playerId, string gameId) =>
{
    try
    {
        var result = await gamePlayerHandler.AddPlayer(playerName, playerId, gameId);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    
    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/start", async (GameService gameService, string playerId, string gameId) =>
{
    try
    {
        await gameService.Start(gameId, playerId);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/game/submitword", async (GameService gameService, string playerId, string gameId, string word) =>
{
    try
    {
        await gameService.SubmitWord(playerId, gameId, word);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


app.MapHub<Hub>("/gameplay");

app.Run();