namespace Backend.GameService.Models;

public class Utils
{
    public static string GenerateGameId()
    {
        var random = new Random();

        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}