using Backend;
using Backend.Data;
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
builder.Services.AddTransient<GameService>();
var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/create", async (GameService gameService, string playerName, string playerId) =>
{
    try
    {
        var game = await gameService.CreateGame(playerName, playerId);
        return Results.Ok(game);
    }
    catch (Exception ex)
    {
        Console.WriteLine("heeey" + ex);
        return Results.BadRequest();
    }

    // player should after this connect to socket with the 'ConnectToGame' keyword
});

app.MapPost("/game/join", async (GameService gameService, string playerName, string playerId, string gameId) =>
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

app.MapPost("/game/start", async (GameService gameService, string playerId, string gameId) =>
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