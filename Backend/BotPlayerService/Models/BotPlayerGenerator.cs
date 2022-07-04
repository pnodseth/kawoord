using Backend.BotPlayerService.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public interface IBotPlayerGenerator
{
    IPlayer GeneratePlayer();
}

public class BotPlayerGenerator : IBotPlayerGenerator
{
    private readonly IBotNames _botNames;
    private readonly Random _random = new();

    public BotPlayerGenerator(IBotNames botNames)
    {
        _botNames = botNames;
    }

    public IPlayer GeneratePlayer()
    {
        var botPlayer = new Player(_botNames.GetRandomName(), GenerateBotId())
        {
            IsBot = true
        };

        return botPlayer;
    }

    private string GenerateBotId()
    {
        const int length = 9;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var id = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
        return id + "--BOT";
    }
}