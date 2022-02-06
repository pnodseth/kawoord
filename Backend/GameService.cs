using System.Text.Json;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class GameService
{
    private readonly GameRepository _repository;
    private readonly GameEngine _gameEngine;
    private readonly IHubContext<Hub> _hubContext;
    private static readonly Random Random = new();

    public GameService(IHubContext<Hub> hubContext, GameRepository repository, GameEngine gameEngine)
    {
        _hubContext = hubContext;
        _repository = repository;
        _gameEngine = gameEngine;
    }
    
    public async Task<GameDto> CreateGame(string playerName, string playerId)
    {
        var config = new Config
        {
            Language = Language.Norwegian,
            RoundLengthSeconds = 10
            
        };
        var hostPlayer = new Player(playerName, playerId);
        var game = new Game(config, GenerateGameId(), GenerateSolution(), hostPlayer);
        
        await _gameEngine.Add(game);
        return new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime, game.CurrentRoundNumber);
    }
    
    public async Task<GameDto> AddPlayer(string playerName, string playerId, string gameId)
    {
        var game = _gameEngine.GamesCache.FirstOrDefault(g => g.GameId == gameId);
        
        if (game is null)
        {
            throw new ArgumentException("No game with that ID found.");
        }
        var player = new Player(playerName, playerId);
        game.Players.Add(player);
        await _gameEngine.Persist(gameId);
        // Also, check if game is full. If so, trigger game start event.
        await _hubContext.Clients.Group(gameId).SendAsync("game-player-join", player, new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime, game.CurrentRoundNumber));

        return new GameDto(game.Players, game.HostPlayer, game.GameId, game.State.Value, game.StartedAtUTC, game.EndedTime, game.CurrentRoundNumber);
    }
    
    public async Task AddPlayerConnectionId(string gameId, string playerId, string connectionId)
    {
        var game = _gameEngine.GamesCache.FirstOrDefault(g => g.GameId == gameId);
        var player = game?.Players.FirstOrDefault(e => e.Id == playerId);
        if (player != null) player.ConnectionId = connectionId;
        await _gameEngine.Persist(gameId);
    }
    

    public async Task Start(string gameId, string playerId)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = await _repository.Get(gameId);
        if (game is null)
        {
            throw new ArgumentException("No game with that id found");
        }

        if (game.HostPlayer.Id != playerId)
        {
            throw new ArgumentException("Only host player can start the game.");
        }

        if (game.State.Value != GameState.Lobby.Value)
        {
            throw new ArgumentException("Game not in 'Lobby' state, can't start this game.");
        }
        
        
        await _gameEngine.StartGame(game);
        
        
    }

    public async Task SubmitWord(string playerId, string gameId, string word)
    {
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(playerId)) throw new ArgumentNullException(nameof(playerId));

        var game = await _repository.Get(gameId);
        var player = game.Players.FirstOrDefault(e => e.Id == playerId);

        if (player is null)
        {
            throw new ArgumentException("No player with that id found");
        }

        if (game is null)
        {
            throw new ArgumentException("No game with that id found");
        }

        if (game.State.Value != GameState.Started.Value)
        {
            throw new ArgumentException("Game not in 'Started' state, can't submit word.");
        }

        if (word.Length != game.Config.WordLength)
        {
            throw new ArgumentException("Length of word does not match current game's word length");
        }

        var isCorrect = ScoreCalculator.IsCorrectWord(game, word);
        var score = ScoreCalculator.CalculateSubmissionScore(game, word);
        var submission = new RoundSubmission(player, game.CurrentRoundNumber, word, DateTime.UtcNow, score, isCorrect);
        game.RoundSubmissions.Add(submission);
        await _repository.Update(game);

        // Set this players round-state  to submitted
        if (player.ConnectionId != null)
        {
            await _hubContext.Clients.Client(player.ConnectionId)
                .SendAsync("round-state", RoundState.PlayerSubmitted);

            // Inform other players that this player has submitted a  word.
            await _hubContext.Clients.GroupExcept(game.GameId, player.ConnectionId)
                .SendAsync("word-submitted", player.Name);
        }
        
        // Test if it works to end round early
        var round = _gameEngine.Rounds.FirstOrDefault(e =>
            e.RoundNumber == game.CurrentRoundNumber && game.GameId == e.Game.GameId);
        round?.EndEarly();
    }
    
    private static string GenerateGameId()
    {
        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());

        
    }

    private static string GenerateSolution()
    {
        var file = new StreamReader("Data/solutions.json");
        var jsonString = file.ReadToEnd();
        
        var worDArr = (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        var randomIdx = Random.Next(worDArr.Count);
        
        return worDArr[randomIdx];
    }
}

