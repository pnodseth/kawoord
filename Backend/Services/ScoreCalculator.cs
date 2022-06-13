using Backend.Models;

namespace Backend.Services;

public static class ScoreCalculator
{
    public static List<LetterEvaluation> CalculateLetterEvaluations(Game game, string word)
    {
        var result = new List<LetterEvaluation>();
        var wordArr = word.Select(letter => (char?) letter).ToList();
        var solutionArr = game.Solution.Select(letter => (char?) letter).ToList();


        /*Check for green letters*/
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            if (wordArr[letterIdx] != solutionArr[letterIdx]) continue;

            var evaluation = new LetterEvaluation
            {
                Letter = wordArr[letterIdx].ToString(),
                WordIndex = letterIdx,
                Round = game.CurrentRoundNumber,
                LetterValueType = CorrectLetterValue.Correct
            };
            result.Add(evaluation);

            solutionArr[letterIdx] = null;
            wordArr[letterIdx] = null;
        }

        /*Check for yellow letters*/
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            var letter = wordArr[letterIdx];
            if (letter is null) continue;
            if (!solutionArr.Contains(letter)) continue;

            var evaluation = new LetterEvaluation
            {
                Letter = wordArr[letterIdx].ToString(),
                WordIndex = letterIdx,
                Round = game.CurrentRoundNumber,
                LetterValueType = CorrectLetterValue.WrongPlacement
            };
            result.Add(evaluation);

            var idx = solutionArr.IndexOf(letter);
            solutionArr[idx] = null;
            wordArr[letterIdx] = null;
        }

        /* Not present letters */
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            if (wordArr[letterIdx] == null) continue;
            var evaluation = new LetterEvaluation
            {
                Letter = wordArr[letterIdx].ToString(),
                WordIndex = letterIdx,
                Round = game.CurrentRoundNumber,
                LetterValueType = CorrectLetterValue.Wrong
            };
            result.Add(evaluation);
        }

        var test = result.Select(e => e.Letter).ToList().ToString();
        Console.WriteLine($"Evaluation: {test}");
        return result;
    }

    public static bool IsCorrectWord(Game game, string word)
    {
        return game.Solution.Equals(word);
    }
}