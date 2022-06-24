using Backend.CommunicationService;
using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data.Data;
using Backend.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend.GameService.Models;

public interface IGame
{
    string GameId { get; set; }
    Task Start();
    void Persist();
}

public class Game : IGame
{
    private readonly ICommunicationHandler _communicationHandler;
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<Game> _logger;

    public Game(IHubContext<Hub> hubContext, ILogger<Game> logger, ICommunicationHandler communicationHandler)
    {
        _hubContext = hubContext;
        _logger = logger;
        _communicationHandler = communicationHandler;
        var solutions = SolutionsSingleton.GetInstance;
        Solution = solutions.GetRandomSolution();
    }

    public List<Player> Players { get; } = new();
    public Player? HostPlayer { get; set; }
    public GameConfig Config { get; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    private DateTime? StartedAtUtc { get; set; }
    private DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; }
    public RoundViewEnum RoundViewEnum { get; set; } = RoundViewEnum.NotStarted;
    public string Solution { get; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public List<RoundInfo> RoundInfos { get; } = new();
    public List<Round> Rounds { get; } = new();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public List<string> CurrentConnections { get; set; } = new();
    public GameTypeEnum GameType { get; set; } = GameTypeEnum.Public;

    private List<Player> BotPlayers
    {
        get { return Players.Where(p => p.IsBot).ToList(); }
    }

    public string GameId { get; set; } = GenerateGameId();

    public async Task Start()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameViewEnum = GameViewEnum.Started;
        StartedAtUtc = DateTime.UtcNow;
        Persist();

        await _hubContext.Clients.Group(GameId).SendAsync("game-update",
            GetDto());

        Console.WriteLine($"Game has started! Solution: {Solution}");
        _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,
            Solution);


        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            // If game is solved, or abandoned, we dont want to continue with next rounds
            if (GameViewEnum == GameViewEnum.Solved ||
                GameViewEnum == GameViewEnum.Abandoned) continue;

            var round = new Round(this, roundNumber);
            Rounds.Add(round);
            await round.StartRound();
        }

        if (GameViewEnum == GameViewEnum.Abandoned) return;
        await GameEnded();
    }

    public void Persist()
    {
        // var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        // if (game != null) await _repository.Update(game);
    }

    public GameDto GetDto()
    {
        if (HostPlayer is null) throw new ArgumentNullException();
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, RoundInfos, RoundViewEnum, RoundSubmissions, PlayerLetterHints);
    }

    public async Task<GameDto> PublishUpdatedGame()
    {
        var gameDto = GetDto();
        await _hubContext.Clients.Group(GameId).SendAsync("game-update", gameDto);

        if (BotPlayers.Count > 0) _communicationHandler.RequestBotsRoundSubmission(gameDto);

        return gameDto;
    }

    private async Task GameEnded()

    {
        Console.WriteLine("Going to GameEnded");
        if (GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        Persist();

        // Send game status update
        var updatedGame = GetDto();
        await _hubContext.Clients.Group(GameId).SendAsync("state", "solution", Solution);


        await _hubContext.Clients.Group(GameId).SendAsync("game-update", updatedGame);
    }

    public void AddRoundSubmission(Player player, string word)
    {
        var isCorrect = ScoreCalculator.IsCorrectWord(this, word);

        var evaluation = ScoreCalculator.CalculateLetterEvaluations(this, word);

        var roundSubmission = new RoundSubmission(player, CurrentRoundNumber, word, DateTime.UtcNow, evaluation,
            isCorrect);
        RoundSubmissions.Add(roundSubmission);
    }


    public void AddPlayerLetterHints(Player player)
    {
        var playerLetterHints = new PlayerLetterHints(this, player);
        playerLetterHints.CalculatePlayerLetterHints();

        var existingHints = PlayerLetterHints.FirstOrDefault(e => e.Player == player);
        if (existingHints is not null) PlayerLetterHints.Remove(existingHints);
        PlayerLetterHints.Add(new PlayerLetterHintsDto(player, playerLetterHints.Correct,
            playerLetterHints.WrongPosition, playerLetterHints.Wrong, playerLetterHints.RoundNumber));
    }

    private static string GenerateGameId()
    {
        var random = new Random();

        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}