using Backend;
using Backend.GameService.Models;
using Backend.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy
            .WithOrigins("https://kawoord.com", "http://localhost:3000")
            .WithMethods("GET", "POST")
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddSingleton<GamePool>();
builder.Services.AddSingleton<PlayerConnectionsDictionary>();
builder.Services.AddSignalR();
builder.Services.AddTransient<GameHandler>();
builder.Services.AddLogging(configure => configure.AddAzureWebAppDiagnostics());

var app = builder.Build();

app.UseCors("SignalRPolicy");

app.MapGet("/", () => "Hello World!");

app.MapPost("/game/create", (GameHandler gameHandler, string playerName, string playerId) =>
{
    try
    {
        gameHandler.CreateGame(new Player(playerName, playerId));
        return Results.Ok(gameHandler.GetGameDto());
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }

    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/join", async (GameHandler gameHandler, string playerName, string playerId, string gameId) =>
{
    try
    {
        var player = new Player(playerName, playerId);
        await gameHandler.SetGame(gameId).AddPlayer(player, gameId);
        var gameDto = gameHandler.GetGameDto();

        return Results.Ok(gameDto);
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
        var result = await gameService.SetGame(gameId).StartGame(playerId);
        return result;
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
        var result = await gameService.SetGame(gameId).SubmitWord(playerId, word);
        return result;
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


app.MapHub<Hub>("/gameplay");

app.Run();