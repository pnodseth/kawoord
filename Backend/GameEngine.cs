using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class GameEngine
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;
    public List<Round> Rounds { get; set; } = new();
    public List<Game> GamesCache { get; set; } = new();

    public GameEngine(IHubContext<Hub> hubContext, GameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task Add(Game game)
    {
        GamesCache.Add(game);
        await _repository.Add(game);
    }


    public async Task Persist(string gameId)
    {
        var game = GamesCache.FirstOrDefault(e => e.GameId == gameId);
        if (game != null)
            await _repository.Update(game);
    }


    public async Task StartGame(Game game)
    {
        // SET GAME STARTED AND SEND EVENTS
        game.State = GameState.Started;
        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime,
                game.CurrentRoundNumber));
        Console.WriteLine(("Game has started!"));
        await Persist(game.GameId);

        foreach (var roundNumber in Enumerable.Range(1, game.Config.NumberOfRounds))
        {
            var round = new Round(_hubContext, game, roundNumber);
            Rounds.Add(round);
            await round.PlayRound();
        }

        await GameEnded(game);
    }

    private async Task GameEnded(Game game)
    {
        game.State = GameState.Ended;
        game.EndedTime = DateTime.UtcNow;
        await Persist(game.GameId);
        
        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime,
                game.CurrentRoundNumber));
        
        // Points
        var roundPoints = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber)
            .Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var points = new RoundAndTotalPoints(roundPoints, roundPoints, 7);
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("points", points);
        
    }
}