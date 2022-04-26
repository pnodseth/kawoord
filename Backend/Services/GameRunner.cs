using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameRunner
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly IGameRepository _repository;
    public List<Round> CurrentRounds { get; } = new();
    public List<Game> CurrentGames { get; } = new();

    public GameRunner(IHubContext<Hub> hubContext, IGameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task Add(Game game)
    {
        CurrentGames.Add(game);
        await _repository.Add(game);
    }


    public async Task Persist(string gameId)
    {
        var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game != null)
            await _repository.Update(game);
    }


    public async Task StartGame(Game game)
    {
        // SET GAME STARTED AND SEND EVENTS
        game.State = GameState.Started;

        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime,
                game.CurrentRoundNumber, game.RoundInfos, game.CurrentRoundState));

        Console.WriteLine(($"Game has started! Solution: {game.Solution}"));

        await Persist(game.GameId);

        // 
        foreach (var roundNumber in Enumerable.Range(1, game.Config.NumberOfRounds))
        {
            if (game.State.Value == GameState.Solved.Value)
            {
                Console.WriteLine("Game is solved, skipping round");
                continue;
            }

            var round = new Round(_hubContext, game, roundNumber);
            CurrentRounds.Add(round);
            await round.PlayRound();
        }

        await GameEnded(game);
    }

    private async Task GameEnded(Game game)

    {
        Console.WriteLine("Going to GameEnded");
        if (game.State.Value != GameState.Solved.Value)
        {
            game.State = GameState.EndedUnsolved;
        }

        game.EndedTime = DateTime.UtcNow;

        await Persist(game.GameId);

        // Send game status update
        var updatedGame = new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC,
            game.EndedTime, game.CurrentRoundNumber, game.RoundInfos, game.CurrentRoundState);
        
        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.State.Value, updatedGame);

        // send round evaluations
        var roundEvaluations = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber)
            .Select(e => new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc)).ToList();


        if (game.State.Value == GameState.Solved.Value)
        {
            var winners = roundEvaluations.FindAll(e => e.isCorrectWord)
                .Select(e => new WinnerSubmission(e.Player, e.SubmittedDateTime)).ToList();
            Console.WriteLine($"Winners: {winners.Count}");
            if (winners.Count > 0)
            {
                game.GameStats = new GameStats() {RoundCompleted = game.CurrentRoundNumber};
                game.GameStats.Winners.AddRange(winners);
                await _hubContext.Clients.Group(game.GameId)
                    .SendAsync("stats", game.GameStats);
            }
        }


        // Send Points
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("points", roundEvaluations);
    }
}