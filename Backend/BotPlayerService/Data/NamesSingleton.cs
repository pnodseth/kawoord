using System.Text.Json;

namespace Backend.BotPlayerService.Data;

public class NamesSingleton
{
    private static readonly Random Random = new();

    private static NamesSingleton? _instance;

    // mutex lock used for thread-safety.
    private static readonly object Mutex = new();
    private readonly Dictionary<string, string> _adjectivesDictionary = new();
    private readonly Dictionary<string, string> _nounsDictionary = new();

    private NamesSingleton()
    {
        // Get nouns
        var file = new StreamReader("BotPlayerService/Data/nouns.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            _nounsDictionary.TryAdd(word, "");

        // Get adjectives
        var adjectivesFile = new StreamReader("BotPlayerService/Data/adjectives.json");
        var adjString = adjectivesFile.ReadToEnd();

        var adjArr =
            (JsonSerializer.Deserialize<string[]>(adjString) ?? throw new InvalidOperationException()).ToList();

        foreach (var word in adjArr)
            _adjectivesDictionary.TryAdd(word, "");
    }

    public static NamesSingleton GetInstance
    {
        get
        {
            lock (Mutex)
            {
                return _instance ??= new NamesSingleton();
            }
        }
    }

    public string GetRandomName()
    {
        var randomIdx = Random.Next(_nounsDictionary.Count);
        var noun = _nounsDictionary.ElementAt(randomIdx).Key;

        randomIdx = Random.Next(_adjectivesDictionary.Count);
        var adjective = _adjectivesDictionary.ElementAt(randomIdx).Key;

        var name = adjective[0].ToString().ToUpper() + adjective.Substring(1) + noun[0].ToString().ToUpper() +
                   noun.Substring(1);
        return name;
    }
}