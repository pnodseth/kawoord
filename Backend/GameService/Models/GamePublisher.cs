using Backend.GameService.Models.Enums;
using Backend.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.GameService.Models;

public interface IGamePublisher
{
    public Task PublishUpdatedGame(IGame game);
    public Task PublishPlayerJoined(IGame game, Player player);
}

public class GamePublisher : IGamePublisher
{
    private readonly IHubContext<Hub> _hubContext;

    public GamePublisher(IHubContext<Hub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishUpdatedGame(IGame game)
    {
        await _hubContext.Clients.Group(game.GameId).SendAsync("game-update", game.GetDto());

        if (game.GameViewEnum is GameViewEnum.Solved or GameViewEnum.EndedUnsolved)
            await _hubContext.Clients.Group(game.GameId).SendAsync("state", "solution", game.Solution);
    }

    public async Task PublishPlayerJoined(IGame game, Player player)
    {
        await _hubContext.Clients.Group(game.GameId).SendAsync("player-event", player, "PLAYER_JOIN");
    }
}