using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameEngine
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly IGameRepository _repository;
    public List<Round> Rounds { get; } = new();
    public List<Game> GamesCache { get; } = new();

    public GameEngine(IHubContext<Hub> hubContext, IGameRepository repository)
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
        Console.WriteLine(($"Game has started! Solution: {game.Solution}"));
        await Persist(game.GameId);

        foreach (var roundNumber in Enumerable.Range(1, game.Config.NumberOfRounds))
        {
            if (game.State.Value == GameState.Solved.Value)
            {
                Console.WriteLine("Game is solved, skipping round");
                continue;
            }

            var round = new Round(_hubContext, game, roundNumber);
            Rounds.Add(round);
            await round.PlayRound();
        }

        await GameEnded(game);
    }

    private async Task GameEnded(Game game)
    {
        if (game.State.Value != GameState.Solved.Value)
        {
            game.State = GameState.EndedUnsolved;
        }

        game.EndedTime = DateTime.UtcNow;
        await Persist(game.GameId);

        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime,
                game.CurrentRoundNumber));


        var roundEvaluations = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber)
            .Select(e => new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc)).ToList();
        var allEvaluations = new RoundAndTotalEvaluations(roundEvaluations, roundEvaluations, 7);


        if (game.State.Value == GameState.Solved.Value)
        {
            var winners = roundEvaluations.FindAll(e => e.isCorrectWord).Select(e => new WinnerSubmission(e.Player, e.SubmittedDateTime)).ToList();
            Console.WriteLine($"Winners: {winners.Count}");
            if (winners.Count > 0)
            {
                game.GameStats = new GameStats() { RoundCompleted = game.CurrentRoundNumber };
                game.GameStats.Winners.AddRange(winners);
                await _hubContext.Clients.Group(game.GameId)
                    .SendAsync("stats", game.GameStats);
            }
        }


        // Points
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("points", allEvaluations);
    }
}