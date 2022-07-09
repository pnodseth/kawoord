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
builder.Services.AddSignalR().AddAzureSignalR();
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
builder.Services.AddTransient<IGameConfig, GameConfig>();
builder.Services.AddSingleton<IBotNames, BotNames>();
builder.Services.AddMemoryCache();


builder.Services.AddLogging(configure => configure.AddAzureWebAppDiagnostics());

var app = builder.Build();

app.UseCors("SignalRPolicy");

app.MapGet("/", () => "Hello World!");

app.MapPost("/game/create", async (IGameHandler gameHandler, IGamePool gamePool, IBotPlayerHandler botPlayerHandler,
    IGame game,
    string playerName, string playerId,
    bool isPublic) =>
{
    game.SetPublic(isPublic);

    if (isPublic)
    {
        /* Check if there are any existing games available */
        var availableGame = gamePool.GetFirstAvailableGame();
        if (availableGame is not null)
        {
            gameHandler.AddPlayerWithGameId(new Player(playerName, playerId), game.GameId);
            return Results.Ok(availableGame.GetDto());
        }

        /* If no existing games available, create a new with bots */
        var random = new Random();
        var maxBotPlayers = game.Config.MaxPlayers - 1;
        var startWithBotPlayersCount = random.Next(1, maxBotPlayers);
        foreach (var i in Enumerable.Range(1, startWithBotPlayersCount))
        {
            var firstBotPlayer = botPlayerHandler.GetNewBotPlayer();
            if (i == 1)
                gameHandler.SetupNewGame(game, firstBotPlayer);
            else
                gameHandler.AddPlayerWithGameId(firstBotPlayer, game.GameId);
        }

        await Task.Delay(3000);
        gameHandler.AddPlayerWithGameId(new Player(playerName, playerId), game.GameId);

        Task.Run(async () =>
        {
            var remainingBotPlayersCount = game.Config.MaxPlayers - startWithBotPlayersCount;
            await botPlayerHandler.RequestBotPlayersToGame(game.GameId,
                remainingBotPlayersCount, 0, 30000);
        });
    }
    else
    {
        gameHandler.SetupNewGame(game, new Player(playerName, playerId));
    }

    return Results.Ok(game.GetDto());
});

app.MapPost("/game/join", (IGameHandler gameHandler, string playerName, string playerId, string gameId) =>
{
    var player = new Player(playerName, playerId);
    var result = gameHandler.AddPlayerWithGameId(player, gameId);
    return result;

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