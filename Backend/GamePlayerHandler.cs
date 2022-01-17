using Backend.Models;

namespace Backend;

public class GamePlayerHandler
{
    private GameRepository _repository;

    public GamePlayerHandler(GameRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> CreateGame(Player hostPlayer)
    {
        var config = new GameConfig(5, 5, 5, true, 120, Language.English);
        var game = new Game(config, GenerateGameId(), GenerateSolution(), hostPlayer);
        await _repository.Add(game);
        return game.GameId;
    }

    public async Task<AddPlayerGameDto> AddPlayer(Player player, string gameId)
    {
        var game = await _repository.Get(gameId);
        game.Players.Add(player);
        
        // Also, check if game is full. If so, trigger game start event.
        return new AddPlayerGameDto(game.Players, game.HostPlayer, game.GameId);

    }
    
    public string GenerateGameId()
    {
        return "MOCK_GAME_ID";
    }

    public string GenerateSolution()
    {
        return "TEMPO";
    }
}

public record AddPlayerGameDto(List<Player> Players, Player HostPlayer, string GameId);
