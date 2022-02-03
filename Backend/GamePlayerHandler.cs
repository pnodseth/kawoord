using System.Text.Json;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class GamePlayerHandler
{
    private GameRepository _repository;
    private static  Random _random = new Random();
    private IHubContext<Hub> _hubContext;

    public GamePlayerHandler(GameRepository repository, IHubContext<Hub> hubContext)
    {
        _repository = repository;
        _hubContext = hubContext;
    }

    public async Task<GameDto> CreateGame(string playerName, string playerId)
    {
        var config = new GameConfig(5, 5, 5, true, 120, Language.English);
        var hostPlayer = new Player(playerName, playerId);
        var game = new Game(config, GenerateGameId(), GenerateSolution(), hostPlayer);
        await _repository.Add(game);
        return new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber);
    }

    public async Task<GameDto> AddPlayer(string playerName, string playerId, string gameId)
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
        await _hubContext.Clients.Group(gameId).SendAsync("game-player-join", player, new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber));

        return new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber);
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



    public async Task AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = await _repository.Get(gameId);
        var player = game.Players.FirstOrDefault(e => e.Id == playerId);
        if (player != null) player.ConnectionId = connectionId;
        await _repository.Update(game);
    }
}

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, string State, DateTime? StartedTime, DateTime? EndedTime, int? CurrentRoundNumber);
