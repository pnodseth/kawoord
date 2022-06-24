using Backend.BotPlayerService.Data;
using Backend.Shared.Models;

namespace Backend.BotPlayerService.Models;

public class BotPlayerGenerator
{
    private readonly Random _random = new();

    public Player GeneratePlayer()
    {
        var botPlayer = new Player(GenerateBotName(), GenerateBotId())
        {
            IsBot = true
        };

        return botPlayer;
    }

    private static string GenerateBotName()
    {
        var namesSingleton = NamesSingleton.GetInstance;
        return namesSingleton.GetRandomName();
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