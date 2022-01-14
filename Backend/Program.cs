using Backend;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/game/new", () =>
{
    var config = new GameConfig(5, 5, 5, true, 120, Language.English);
    var game = new Game(config);
});

app.Run();
