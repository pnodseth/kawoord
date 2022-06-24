using Backend.BotPlayerService.Models;
using Backend.GameService.Models;
using Backend.Shared.Models;

namespace Backend.CommunicationService;

public interface ICommunicationHandler
{
    public Task AddPlayer(Player player, string gameId);
    public void SubmitWord(string playerId, string gameId, string word);
    public void RequestBotsRoundSubmission(GameDto gameDto);

    public Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs,
        int maxTimeToLastAddedMs);
}

public class CommunicationHandler : ICommunicationHandler
{
    private readonly BotPlayerHandler _botPlayerHandler;
    private readonly GameHandler _gameHandler;

    public CommunicationHandler(GameHandler gameHandler, BotPlayerHandler botPlayerHandler)
    {
        _gameHandler = gameHandler;
        _botPlayerHandler = botPlayerHandler;
    }

    public async void SubmitWord(string playerId, string gameId, string word)
    {
        await _gameHandler.SubmitWord(playerId, gameId, word);
    }

    public void RequestBotsRoundSubmission(GameDto gameDto)
    {
        _botPlayerHandler.RequestBotsRoundSubmission(gameDto);
    }

    public async Task AddPlayer(Player player, string gameId)
    {
        await _gameHandler.AddPlayer(player, gameId);
    }

    public async Task RequestBotPlayersToGame(string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 30000)
    {
        await _botPlayerHandler.RequestBotPlayersToGame(gameId, numberOfBots, timeToFirstAddedMs, maxTimeToLastAddedMs);
    }
}