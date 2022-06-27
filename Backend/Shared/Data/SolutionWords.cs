using System.Text.Json;
using Backend.GameService.Models.Dto;

namespace Backend.Shared.Data;

public interface ISolutionWords
{
    string GetRandomSolution();
    string GetWordBasedOnCorrectLetters(PlayerLetterHintsDto playerLetterHints);
}

public class SolutionWords : ISolutionWords
{
    private readonly Dictionary<string, string> _dictionary = new();
    private readonly List<string> _dictionaryAsList;
    private readonly Random _random = new();

    public SolutionWords()
    {
        var file = new StreamReader("Shared/Data/solutions.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            _dictionary.TryAdd(word, "");

        _dictionaryAsList = new List<string>(_dictionary.Keys);
    }


    public string GetRandomSolution()
    {
        var randomIdx = _random.Next(_dictionary.Count);
        return _dictionary.ElementAt(randomIdx).Key;
    }

    public string GetWordBasedOnCorrectLetters(PlayerLetterHintsDto playerLetterHints)
    {
        var allPossibleSolutions = new List<string>(_dictionaryAsList);

        foreach (var letterEvaluation in playerLetterHints.Correct)
            allPossibleSolutions = allPossibleSolutions
                .Where(v => v[letterEvaluation.WordIndex].ToString() == letterEvaluation.Letter).ToList();

        foreach (var letterEvaluation in playerLetterHints.WrongPosition)
            allPossibleSolutions = allPossibleSolutions.FindAll(w => w.Contains(letterEvaluation.Letter));
        // todo - also take in account letters with wrong position, and letters already tried

        foreach (var letterEvaluation in playerLetterHints.Wrong)
            allPossibleSolutions = allPossibleSolutions.FindAll(w => !w.Contains(letterEvaluation.Letter));

        var randNumber = _random.Next(allPossibleSolutions.Count);
        return allPossibleSolutions[randNumber];
    }
}