using Backend;
using Backend.BotPlayerService.Data;
using Backend.BotPlayerService.Models;
using Backend.GameService.Models;
using Backend.GameService.Providers;
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
builder.Services.AddTransient<IGamePool, GamePool>();
builder.Services.AddSingleton<IConnectionsDictionary, ConnectionsDictionary>();
builder.Services.AddSignalR();
builder.Services.AddTransient<IGameHandler, GameHandler>();
builder.Services.AddTransient<IGame, Game>();
builder.Services.AddTransient<IGamePublisher, GamePublisher>();
builder.Services.AddTransient<IBotPlayerHandler, BotPlayerHandler>();
builder.Services.AddTransient<IScoreCalculator, ScoreCalculator>();
builder.Services.AddSingleton<ISolutionWords, SolutionWords>();
builder.Services.AddSingleton<IValidWords, ValidWords>();
builder.Services.AddTransient<IConnectionsHandler, ConnectionsHandler>();
builder.Services.AddTransient<IRandomProvider, RandomProvider>();
builder.Services.AddTransient<IBotPlayerGenerator, BotPlayerGenerator>();
builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<IBotNames, BotNames>();
builder.Services.AddMemoryCache();


builder.Services.AddLogging(configure => configure.AddAzureWebAppDiagnostics());

var app = builder.Build();

app.UseCors("SignalRPolicy");

app.MapGet("/", () => "Hello World!");

app.MapPost("/game/create",
    (IGameHandler gameHandler, IBotPlayerHandler botPlayerHandler, IGame game, string playerName, string playerId,
        bool isPublic) =>
    {
        game.SetPublic(isPublic);

        try
        {
            gameHandler.SetupNewGame(game, new Player(playerName, playerId));

            if (game.Config.Public)
                Task.Run(async () => { await botPlayerHandler.RequestBotPlayersToGame(game.GameId, 3, 0, 15000); });

            return Results.Ok(game.GetDto());
        }
        catch (Exception)
        {
            return Results.BadRequest();
        }
    });

app.MapPost("/game/join", (IGameHandler gameHandler, string playerName, string playerId, string gameId) =>
{
    try
    {
        var player = new Player(playerName, playerId);
        var gameDto = gameHandler.AddPlayerWithGameId(player, gameId);
        return Results.Ok(gameDto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/start", async (IGameHandler gameService, string playerId, string gameId) =>
{
    var result = await gameService.HandleStartGame(gameId, playerId);
    return result;
});

app.MapPost("/game/submitword", async (IGameHandler gameService, string playerId, string gameId, string word) =>
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

app.MapGet("/random-name", (IBotNames botNames) =>
{
    var name = botNames.GetRandomName();
    return Results.Ok(name);
});


app.MapHub<Hub>("/gameplay");

app.Run();