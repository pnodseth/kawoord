using Backend.GameService.Providers;

namespace Backend.GameService.Models;

public interface IUtils
{
    string GenerateGameId();
}

public class Utils : IUtils
{
    public string GenerateGameId()
    {
        var random = new RandomProvider();


        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.RandomFromMax(s.Length)]).ToArray());
    }
}