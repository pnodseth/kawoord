using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class GameService
{
    private readonly GameRepository _repository;
    private readonly IHubContext<Hub> _hubContext;

    public GameService(IHubContext<Hub> hubContext, GameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task Start(string gameId, string playerId)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));
        
        var game = await _repository.Get(gameId);
        if (game is null)
        {
            throw new ArgumentException("No game with that id found");
        }
        if (game.HostPlayer.Id != playerId)
        {
            throw new ArgumentException("Only host player can start the game.");
        }
        if (game.State.Value != GameState.Lobby.Value)
        {
            throw new ArgumentException("Game not in 'Lobby' state, can't start this game.");
        }

        const int startingInSeconds = (10);
        var startUtc = DateTime.UtcNow.AddSeconds(startingInSeconds);
        game.State = GameState.Starting;
        game.StartedTime = startUtc;
        await _repository.Update(game);
        
        // Notify players that game state has changed
        Console.WriteLine(("game will start in 10"));
        await _hubContext.Clients.Group(gameId).SendAsync("gamestate", game.State.Value, new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber));
        
        await Task.Delay(10000);
        game.State = GameState.Started;
        await _repository.Update(game);
        Console.WriteLine(("game has started!"));
        await _hubContext.Clients.Group(gameId).SendAsync("gamestate", game.State.Value, new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedTime, game.EndedTime, game.CurrentRoundNumber));
        await Task.Delay(2000);

        const int roundLengthSeconds = 30;
        var roundEndsUtc = DateTime.UtcNow.AddSeconds(roundLengthSeconds);
        var RoundOneInfo = new RoundInfo(game.CurrentRoundNumber, roundLengthSeconds, roundEndsUtc);
        
        Console.WriteLine($"Round ends: {roundEndsUtc}");
        // SEND ROUND INFO ROUND 1 
        await _hubContext.Clients.Group(gameId).SendAsync("round-info", RoundOneInfo);
        //SEND ROUND STATE ROUND 1
        await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateStarted(RoundState.Started));
        await Task.Delay(roundLengthSeconds * 1000);
        await _hubContext.Clients.Group(gameId).SendAsync("round-state", new RoundStateStarted(RoundState.Summary));
    }
}

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record RoundStateStarted(RoundState state);
// Roundstate: Playing, Summary, Points