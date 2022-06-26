using Backend;
using Backend.BotPlayerService.Models;
using Backend.GameService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
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
builder.Services.AddTransient<Game>();
builder.Services.AddTransient<IGamePublisher, GamePublisher>();
builder.Services.AddTransient<BotPlayerHandler>();
builder.Services.AddTransient<ScoreCalculator>();
builder.Services.AddSingleton<Solutions>();
builder.Services.AddSingleton<ValidWords>();

builder.Services.AddLogging(configure => configure.AddAzureWebAppDiagnostics());

var app = builder.Build();

app.UseCors("SignalRPolicy");

app.MapGet("/", () => "Hello World!");

app.MapPost("/game/create",
    (GameHandler gameHandler, BotPlayerHandler botPlayerHandler, Game game, string playerName, string playerId) =>
    {
        try
        {
            gameHandler.SetupNewGame(game, new Player(playerName, playerId));

            if (Game.GameType == GameTypeEnum.Public)
                Task.Run(async () => { await botPlayerHandler.RequestBotPlayersToGame(game.GameId, 2, 500); });

            return Results.Ok(game.GetDto());
        }
        catch (Exception)
        {
            return Results.BadRequest();
        }
    });

app.MapPost("/game/join", async (GameHandler gameHandler, string playerName, string playerId, string gameId) =>
{
    try
    {
        var player = new Player(playerName, playerId);
        var gameDto = await gameHandler.AddPlayerWithGameId(player, gameId);
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
        var result = await gameService.StartGame(gameId, playerId);
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
        var result = await gameService.SubmitWord(gameId, playerId, word);
        return result;
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});


app.MapHub<Hub>("/gameplay");

app.Run();