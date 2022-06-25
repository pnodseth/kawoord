using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGame
{
    string GameId { get; set; }
    Task Start();
    void Persist();
}

public class Game : IGame
{
    public readonly GameHandler Handler;

    public Game(GameHandler handler)
    {
        Handler = handler;
        var solutions = SolutionsSingleton.GetInstance;
        Solution = solutions.GetRandomSolution();
    }

    public List<Player> Players { get; } = new();
    public Player? HostPlayer { get; set; }
    public GameConfig Config { get; set; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; set; }
    public RoundViewEnum RoundViewEnum { get; set; } = RoundViewEnum.NotStarted;
    public string Solution { get; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public List<RoundInfo> RoundInfos { get; } = new();
    public List<Round> Rounds { get; } = new();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public List<string> CurrentConnections { get; set; } = new();
    public GameTypeEnum GameType { get; set; } = GameTypeEnum.Public;

    public List<Player> BotPlayers
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

        await Handler.PublishUpdatedGame();

        Console.WriteLine($"Game has started! Solution: {Solution}");
        //todo: Find out how to add logger without DI
        // _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,Solution);


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


    private async Task GameEnded()

    {
        Console.WriteLine("Going to GameEnded");
        if (GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        Persist();
        await Handler.PublishUpdatedGame();
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

    public void AddPlayer(Player player, bool isHostPlayer = false)
    {
        Players.Add(player);
        if (isHostPlayer) HostPlayer = player;
        //todo:  Also, check if game is full. If so, trigger game start event.
    }

    public Player? FindPlayer(string playerId)
    {
        return Players.FirstOrDefault(e => e.Id == playerId);
    }

    public void AddPlayerConnection(Player player, string connectionId)
    {
        player.ConnectionId = connectionId;
        CurrentConnections.Add(connectionId);
    }

    public void RemovePlayer(string connectionId)
    {
        CurrentConnections.Remove(connectionId);
        var player = Players.FirstOrDefault(e => e.ConnectionId == connectionId);
        if (player is not null) Players.Remove(player);
    }
}