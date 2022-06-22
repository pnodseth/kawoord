using System.Text.Json;

namespace Backend.GameService.Data;

public sealed class SolutionsSingleton
{
    private static readonly Random Random = new();

    private static SolutionsSingleton? _instance;

    // mutex lock used for thread-safety.
    private static readonly object Mutex = new();
    private readonly Dictionary<string, string> _dictionary = new();

    private SolutionsSingleton()
    {
        var file = new StreamReader("GameService/Data/solutions.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            _dictionary.TryAdd(word, "");
    }

    public static SolutionsSingleton GetInstance
    {
        get
        {
            lock (Mutex)
            {
                return _instance ??= new SolutionsSingleton();
            }
        }
    }

    public string GetRandomSolution()
    {
        var randomIdx = Random.Next(_dictionary.Count);
        return _dictionary.ElementAt(randomIdx).Key;
    }
}