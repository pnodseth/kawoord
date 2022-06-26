using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGame
{
    string GameId { get; set; }
    Task StartGame();
}

public class Game : IGame
{
    private readonly GameHandler _handler;

    public Game(GameHandler handler)
    {
        _handler = handler;
    }

    public List<Player> Players { get; } = new();
    public Player? HostPlayer { get; private set; }
    public GameConfig Config { get; set; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    private DateTime? StartedAtUtc { get; set; }
    private DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; private set; }

    public string? Solution { get; set; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public List<Round> Rounds { get; } = new();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public List<string> CurrentConnections { get; set; } = new();
    public GameTypeEnum GameType { get; set; } = GameTypeEnum.Public;
    public Round? CurrentRound { get; set; }

    public List<Player> BotPlayers
    {
        get { return Players.Where(p => p.IsBot).ToList(); }
    }

    public string GameId { get; set; } = Utils.GenerateGameId();

    public async Task StartGame()
    {
        // SET GAME STARTED AND SEND EVENTS
        GameViewEnum = GameViewEnum.Started;
        StartedAtUtc = DateTime.UtcNow;

        await _handler.PublishUpdatedGame();


        Console.WriteLine($"Game has started! Solution: {Solution}");
        //todo: Find out how to add logger without DI
        // _logger.LogInformation("Game with ID {ID} started at {Time}. Solution: {Solution}", GameId, DateTime.UtcNow,Solution);


        // 
        foreach (var roundNumber in Enumerable.Range(1, Config.NumberOfRounds))
        {
            // If game is solved, or abandoned, we dont want to continue with next rounds
            if (GameViewEnum == GameViewEnum.Solved ||
                GameViewEnum == GameViewEnum.Abandoned) continue;

            await RunRound(roundNumber);
        }

        if (GameViewEnum == GameViewEnum.Abandoned) return;
        await SetGameEnded();
    }


    private async Task RunRound(int roundNumber)
    {
        CurrentRoundNumber = roundNumber;


        var round = new Round().SetRoundOptions(CurrentRoundNumber, Config.RoundLengthSeconds,
            Config.RoundSummaryLengthSeconds);
        Rounds.Add(round);
        CurrentRound = round;

        if (BotPlayers.Count > 0) _handler.BotPlayerHandler.RequestBotsRoundSubmission(GetDto());
        await _handler.PublishUpdatedGame();

        await round.PlayRound();
        //todo: Find winners var winners = roundPoints.FindAll(e => e.IsCorrectWord).Select(e => e.Player).ToList();
        // if (winners.Count > 0) Game.GameViewEnum = GameViewEnum.Solved;
        await round.ShowSummary();
    }


    public GameDto GetDto()
    {
        if (HostPlayer is null) throw new ArgumentNullException();

        var roundsDto = Rounds.Select(r => r.GetDto()).ToList();
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, roundsDto, RoundSubmissions, PlayerLetterHints);
    }


    private async Task SetGameEnded()

    {
        if (GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.EndedUnsolved;

        EndedTime = DateTime.UtcNow;

        await _handler.PublishUpdatedGame();
    }

    public void AddRoundSubmission(Player player, string word)
    {
        var isCorrect = ScoreCalculator.IsCorrectWord(this, word);
        var evaluation = ScoreCalculator.CalculateLetterEvaluations(this, word);

        var roundSubmission = new RoundSubmission(player, CurrentRoundNumber, word, DateTime.UtcNow, evaluation,
            isCorrect);
        RoundSubmissions.Add(roundSubmission);

        if (isCorrect && GameViewEnum != GameViewEnum.Solved) GameViewEnum = GameViewEnum.Solved;
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