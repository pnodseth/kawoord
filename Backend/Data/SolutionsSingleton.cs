using System.Text.Json;

namespace Backend.Data;

public sealed class SolutionsSingleton
{
    private static readonly Random Random = new();

    private static SolutionsSingleton? _instance;

    // mutex lock used for thread-safety.
    private static readonly object Mutex = new();
    private readonly Dictionary<string, string> _dictionary = new();

    private SolutionsSingleton()
    {
        var file = new StreamReader("Data/solutions.json");
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
                // logger1.LogError("Error adding word to Solutions dictionary . {Ex}", ex.Message);
            }
    }

    public static SolutionsSingleton GetInstance
    {
        get
        {
            lock (Mutex)
            {
                if (_instance is null) _instance = new SolutionsSingleton();
                return _instance;
            }
        }
    }

    public string GetRandomSolution()
    {
        var randomIdx = Random.Next(_dictionary.Count);
        return _dictionary.ElementAt(randomIdx).Key;
    }
}