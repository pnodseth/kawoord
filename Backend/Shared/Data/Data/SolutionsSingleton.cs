using System.Text.Json;
using Backend.GameService.Models;

namespace Backend.Shared.Data.Data;

public sealed class SolutionsSingleton
{
    private static SolutionsSingleton? _instance;

    // mutex lock used for thread-safety.
    private static readonly object Mutex = new();
    private readonly Dictionary<string, string> _dictionary = new();
    private readonly Random _random = new();

    private SolutionsSingleton()
    {
        var file = new StreamReader("GameService/Data/solutions.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            _dictionary.TryAdd(word, "");

        DictionaryAsList = new List<string>(_dictionary.Values);
    }

    private List<string> DictionaryAsList { get; }

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
        var randomIdx = _random.Next(_dictionary.Count);
        return _dictionary.ElementAt(randomIdx).Key;
    }

    public string GetWordBasedOnCorrectLetters(List<LetterEvaluation> correct)
    {
        var allPossibleSolutions = new List<string>(DictionaryAsList);

        foreach (var letterEvaluation in correct)
            allPossibleSolutions = DictionaryAsList
                .Where(v => v[letterEvaluation.WordIndex].ToString() == letterEvaluation.Letter).ToList();

        var randNumber = _random.Next(allPossibleSolutions.Count);
        return allPossibleSolutions[randNumber];
    }
}