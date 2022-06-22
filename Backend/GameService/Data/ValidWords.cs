using System.Text.Json;

namespace Backend.GameService.Data;

public class ValidWords
{
    private readonly Dictionary<string, string> _dictionary = new();

    public ValidWords(ILogger<ValidWords> logger)
    {
        var logger1 = logger;
        var file = new StreamReader("GameService/Data/validwords.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            try
            {
                _dictionary.TryAdd(word, "");
            }
            catch (ArgumentNullException ex)
            {
                logger1.LogError("Error adding word to dictionary. {Ex}", ex.Message);
            }

        logger1.LogInformation("Added {Count} words to dictionary", _dictionary.Count);
    }

    public bool IsValidWord(string word)
    {
        return _dictionary.ContainsKey(word);
    }
}