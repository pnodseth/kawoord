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
        
        game.State = GameState.Starting;
        await _repository.Update(game);
        
        // Notify players that game state has changed
        await _hubContext.Clients.Group(gameId).SendAsync("gamestate", game.State.Value);
        
    }
}