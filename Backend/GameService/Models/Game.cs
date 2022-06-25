using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;

namespace Backend.GameService.Models;

public interface IGame
{
    string GameId { get; set; }
    Task StartGame();
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
    public Player? HostPlayer { get; private set; }
    public GameConfig Config { get; set; } = new();
    public GameViewEnum GameViewEnum { get; set; } = GameViewEnum.Lobby;
    private DateTime? StartedAtUtc { get; set; }
    private DateTime? EndedTime { get; set; }
    public int CurrentRoundNumber { get; private set; }

    public string Solution { get; }
    public List<RoundSubmission> RoundSubmissions { get; } = new();
    public List<Round> Rounds { get; } = new();
    private List<PlayerLetterHintsDto> PlayerLetterHints { get; } = new();
    public List<string> CurrentConnections { get; set; } = new();
    public GameTypeEnum GameType { get; set; } = GameTypeEnum.Public;

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

            await RunRound(roundNumber);
        }

        if (GameViewEnum == GameViewEnum.Abandoned) return;
        await SetGameEnded();
    }

    public void Persist()
    {
        // var game = CurrentGames.FirstOrDefault(e => e.GameId == gameId);
        // if (game != null) await _repository.Update(game);
    }


    private async Task RunRound(int roundNumber)
    {
        CurrentRoundNumber = roundNumber;


        var round = new Round(this, roundNumber);
        Rounds.Add(round);

        if (BotPlayers.Count > 0) Handler.BotPlayerHandler.RequestBotsRoundSubmission(GetDto());
        await Handler.PublishUpdatedGame();
        Persist();
        await round.StartRound();
    }


    public GameDto GetDto()
    {
        if (HostPlayer is null) throw new ArgumentNullException();

        var roundsDto = Rounds.Select(r => new RoundDto(r.RoundNumber, r.RoundLengthSeconds, r.RoundEndsUtc)).ToList();
        return new GameDto(Players, HostPlayer, GameId, GameViewEnum,
            StartedAtUtc,
            EndedTime, CurrentRoundNumber, roundsDto, RoundSubmissions, PlayerLetterHints);
    }


    private async Task SetGameEnded()

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