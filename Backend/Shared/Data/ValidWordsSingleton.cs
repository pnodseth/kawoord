using System.Text.Json;

namespace Backend.Shared.Data;

public sealed class ValidWordsSingleton
{
    private static readonly object Mutex = new();
    private static ValidWordsSingleton? _instance;
    private static readonly Random Random = new();
    private readonly Dictionary<string, string> _dictionary = new();

    private ValidWordsSingleton()
    {
        var file = new StreamReader("Shared/Data/validwords.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            try
            {
                _dictionary.TryAdd(word, "");
            }
            catch (ArgumentNullException)
            {
                // logger1.LogError("Error adding word to dictionary. {Ex}", ex.Message);
            }

        // logger1.LogInformation("Added {Count} words to dictionary", _dictionary.Count);
    }

    public static ValidWordsSingleton GetInstance
    {
        get
        {
            lock (Mutex)
            {
                return _instance ??= new ValidWordsSingleton();
            }
        }
    }

    public bool IsValidWord(string word)
    {
        return _dictionary.ContainsKey(word);
    }

    public string GetRandomWord()
    {
        var randomIdx = Random.Next(_dictionary.Count);
        return _dictionary.ElementAt(randomIdx).Key;
    }
}