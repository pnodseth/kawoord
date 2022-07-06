using System.Text.Json;
using Backend.GameService.Models.Dto;

namespace Backend.Shared.Data;

public interface IValidWords
{
    bool IsValidWord(string word);
    string GetRandomWord();
    string GetWordBasedOnCorrectLetters(PlayerLetterHintsDto playerLetterHints);
}

public class ValidWords : IValidWords
{
    private static readonly Random Random = new();
    private readonly Dictionary<string, string> _dictionary = new();
    private readonly List<string> _dictionaryAsList;

    public ValidWords()
    {
        var file = new StreamReader("Shared/Data/validwords.json");
        var jsonString = file.ReadToEnd();

        var worDArr =
            (JsonSerializer.Deserialize<string[]>(jsonString) ?? throw new InvalidOperationException()).ToList();
        foreach (var word in worDArr)
            _dictionary.TryAdd(word, "");

        _dictionaryAsList = new List<string>(_dictionary.Keys);


        // logger1.LogInformation("Added {Count} words to dictionary", _dictionary.Count);
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
        if (allPossibleSolutions.Count == 0) return GetRandomWord();

        var randNumber = Random.Next(allPossibleSolutions.Count);
        return allPossibleSolutions[randNumber];
    }
}