using System.Text.Json;
using Backend.Models;

namespace Backend;

public class GamePlayerHandler
{
    private GameRepository _repository;
    private static  Random _random = new Random();

    public GamePlayerHandler(GameRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddPlayerGameDto> CreateGame(string playerName, string playerId)
    {
        var config = new GameConfig(5, 5, 5, true, 120, Language.English);
        var hostPlayer = new Player(playerName, playerId);
        var game = new Game(config, GenerateGameId(), GenerateSolution(), hostPlayer);
        await _repository.Add(game);
        return new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value);
    }

    public async Task<AddPlayerGameDto> AddPlayer(string playerName, string playerId, string gameId)
    {
        var game = await _repository.Get(gameId);
        if (game is null)
        {
            throw new ArgumentException("No game with that ID found.");
        }
        var player = new Player(playerName, playerId);
        game.Players.Add(player);
        await _repository.Update(game);
        
        // Also, check if game is full. If so, trigger game start event.
        return new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value);
    }

    public string GenerateGameId()
    {
        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());

        
    }

    public string GenerateSolution()
    {
        var file = new StreamReader("Data/solutions.json");
        var jsonString = file.ReadToEnd();
        
        var worDArr = (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        var randomIdx = _random.Next(worDArr.Count);
        
        return worDArr[randomIdx];
    }

    public async Task<AddPlayerGameDto> FindGame(string gameId)
    {
        var game = await _repository.Get(gameId);
        return new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value);
    }
}

public record AddPlayerGameDto(List<Player> Players, Player HostPlayer, string GameId, string State);
