using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;


public record RoundAndTotalPoints(List<PlayerPoints> RoundPoints, List<PlayerPoints> TotalPoints, int ViewLengthSeconds);
public record PlayerPoints(Player Player, int Points);
public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);


public class GameEngine
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;
    public List<Round> Rounds { get; set; } = new();

    public GameEngine(IHubContext<Hub> hubContext, GameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
        
    }


    public async Task InitiateGame(Game game)
    {
        // SET GAME STARTED AND SEND EVENTS
        game.State = GameState.Started;
        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime,
                game.CurrentRoundNumber));
        Console.WriteLine(("Game has started!"));
        
        
        foreach (var roundNumber in Enumerable.Range(1, game.Config.NumberOfRounds))
        {
            var round = new Round(_hubContext, _repository, game, roundNumber);
            Rounds.Add(round);
            await round.PlayRound();
        }
    }
    
    

   
    
}