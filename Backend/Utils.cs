using System.Text.Json;

namespace Backend;

public class Utils
{
    private static readonly Random Random = new();

    public static string GenerateGameId()
    {
        const int length = 7;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
    
    public static string GenerateSolution()
    {
        var file = new StreamReader("Data/solutions.json");
        var jsonString = file.ReadToEnd();
        
        var worDArr = (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        var randomIdx = Random.Next(worDArr.Count);
        
        return worDArr[randomIdx];
    }
}