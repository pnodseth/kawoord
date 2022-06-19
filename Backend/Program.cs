using Backend;
using Backend.Data;
using Backend.Models;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
        });
});
builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<GamePool>();
builder.Services.AddSignalR();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddTransient<GameHandler>();
builder.Services.AddTransient<Game>();
builder.Services.AddLogging();
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/create",  (GameHandler gameHandler,Game game, string playerName, string playerId) =>
{
    try
    {
        gameHandler.SetupGame(game, new Player(playerName, playerId));
        return Results.Ok(game.GetDto());
    }
    catch (Exception ex)
    {
        Console.WriteLine("heeey" + ex);
        return Results.BadRequest();
    }

    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/join", async (GameHandler gameService, string playerName, string playerId, string gameId) =>
{
    try
    {
        var result = await gameService.AddPlayer(playerName, playerId, gameId);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/start", async (GameHandler gameService, string playerId, string gameId) =>
{
    try
    {
        await gameService.StartGame(gameId, playerId);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/game/submitword", async (GameHandler gameService, string playerId, string gameId, string word) =>
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