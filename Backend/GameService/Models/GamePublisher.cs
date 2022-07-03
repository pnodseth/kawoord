using Backend.GameService.Models.Enums;
using Backend.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.GameService.Models;

public interface IGamePublisher
{
    public void PublishUpdatedGame(IGame game);
    public void PublishUpdatedGame(string gameId);
    public void PublishPlayerJoined(IGame game, IPlayer player);
    public void PublishWordSubmitted(string gameId, IPlayer player);
}

public class GamePublisher : IGamePublisher
{
    private readonly IGamePool _gamePool;
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<GamePublisher> _logger;

    public GamePublisher(IHubContext<Hub> hubContext, IGamePool gamePool, ILogger<GamePublisher> logger)
    {
        _hubContext = hubContext;
        _gamePool = gamePool;
        _logger = logger;
    }

    public void PublishUpdatedGame(IGame game)
    {
        Task.Run(async () =>
        {
            await _hubContext.Clients.Group(game.GameId).SendAsync("game-update", game.GetDto());

            if (game.GameViewEnum is GameViewEnum.Solved or GameViewEnum.EndedUnsolved)
                await _hubContext.Clients.Group(game.GameId).SendAsync("state", "solution", game.Solution);
        });
    }


    public void PublishPlayerJoined(IGame game, IPlayer player)
    {
        Task.Run(async () =>
        {
            await _hubContext.Clients.Group(game.GameId).SendAsync("playerEvent", player, "PLAYER_JOIN");
        });
    }

    public void PublishWordSubmitted(string gameId, IPlayer player)
    {
        Task.Run(async () =>
        {
            await _hubContext.Clients.GroupExcept(gameId, player.ConnectionId ?? "")
                .SendAsync("playerEvent",
                    new PlayerEventData(NotificationsEnum.WordSubmitted, player.Name, Guid.NewGuid().ToString()));
        });
    }

    public void PublishUpdatedGame(string gameId)
    {
        var game = _gamePool.FindGame(gameId);
        if (game is null)
        {
            _logger.LogWarning("No game with {Id} found", gameId);
            return;
        }

        Task.Run(async () =>
        {
            await _hubContext.Clients.Group(game.GameId).SendAsync("game-update", game.GetDto());

            if (game.GameViewEnum is GameViewEnum.Solved or GameViewEnum.EndedUnsolved)
                await _hubContext.Clients.Group(game.GameId).SendAsync("state", "solution", game.Solution);
        });
    }
}

public record PlayerEventData(NotificationsEnum Type, string PlayerName, string Id);