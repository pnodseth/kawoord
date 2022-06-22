using Backend.GameService.Models;

namespace Backend.BotPlayerService.Models;

public class BotPlayerHandler
{
    public async Task AddBotPlayersToGame(GameHandler gameHandler, string gameId, int numberOfBots,
        int timeToFirstAddedMs = 0,
        int maxTimeToLastAddedMs = 30000)
    {
        var random = new Random();
        var addTimeRemaining = maxTimeToLastAddedMs;
        var botGenerator = new BotPlayerGenerator();

        // Add players at random intervals
        foreach (var unused in Enumerable.Range(1, numberOfBots))
        {
            var randomWaitTime = random.Next(4000, addTimeRemaining);
            Console.WriteLine($"Waiting {randomWaitTime} before adding next player");
            await Task.Delay(randomWaitTime);

            await gameHandler.AddPlayer(botGenerator.GeneratePlayer(), gameId);
            addTimeRemaining -= randomWaitTime;
        }

        Console.WriteLine("Done adding bot players");
    }
}