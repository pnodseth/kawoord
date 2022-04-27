using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

public class GameRunner
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly IGameRepository _repository;

    public GameRunner(IHubContext<Hub> hubContext, IGameRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public List<Round> CurrentRounds { get; } = new();
    public List<Game> CurrentGames { get; } = new();

    public async Task Add(Game game)
    {
        CurrentGames.Add(game);
        await _repository.Add(game);
    }


    public async Task Persist(string gameId)
    {
        var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        if (game != null) await _repository.Update(game);
    }


    public async Task StartGame(Game game)
    {
        // SET GAME STARTED AND SEND EVENTS
        game.GameStateEnum = GameStateEnum.Started;
        game.StartedAtUtc = DateTime.UtcNow;
        await Persist(game.GameId);

        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.GameStateEnum.Value,
            new GameDto(game.Players, game.HostPlayer, game.GameId, game.GameStateEnum, game.StartedAtUtc,
                game.EndedTime,
                game.CurrentRoundNumber, game.RoundInfos, game.RoundStateEnum));

        Console.WriteLine($"Game has started! Solution: {game.Solution}");


        // 
        foreach (var roundNumber in Enumerable.Range(1, game.Config.NumberOfRounds))
        {
            if (game.GameStateEnum.Value == GameStateEnum.Solved.Value)
            {
                Console.WriteLine("Game is solved, skipping round");
                continue;
            }

            var round = new Round(_hubContext, game, roundNumber);
            CurrentRounds.Add(round);
            await round.StartRound();
        }

        await GameEnded(game);
    }

    private async Task GameEnded(Game game)

    {
        Console.WriteLine("Going to GameEnded");
        if (game.GameStateEnum.Value != GameStateEnum.Solved.Value) game.GameStateEnum = GameStateEnum.EndedUnsolved;

        game.EndedTime = DateTime.UtcNow;

        await Persist(game.GameId);

        // Send game status update
        var updatedGame = new GameDto(game.Players, game.HostPlayer, game.GameId, game.GameStateEnum,
            game.StartedAtUtc,
            game.EndedTime, game.CurrentRoundNumber, game.RoundInfos, game.RoundStateEnum);

        await _hubContext.Clients.Group(game.GameId).SendAsync("gamestate", game.GameStateEnum.Value, updatedGame);

        // send round evaluations
        var roundEvaluations = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber)
            .Select(e => new WordEvaluation(e.Player, e.LetterEvaluations, e.IsCorrectWord, e.SubmittedAtUtc,
                game.CurrentRoundNumber)).ToList();


        // Send Points
        await _hubContext.Clients.Group(game.GameId)
            .SendAsync("points", roundEvaluations);
    }
}