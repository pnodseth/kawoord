namespace Backend.GameService.Models;

public interface IScoreCalculator
{
    List<LetterEvaluation> CalculateLetterEvaluations(IGame game, string submittedWord);
    bool IsCorrectWord(string solution, string word);
}

public class ScoreCalculator : IScoreCalculator
{
    public List<LetterEvaluation> CalculateLetterEvaluations(IGame game, string submittedWord)
    {
        if (game.Solution is null) throw new NullReferenceException();
        var result = new List<LetterEvaluation>();
        var wordArr = submittedWord.Select(letter => (char?) letter).ToList();
        var solutionArr = game.Solution.Select(letter => (char?) letter).ToList();


        /*Check for green letters*/
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            if (wordArr[letterIdx] != solutionArr[letterIdx]) continue;
            if (wordArr[letterIdx].ToString() is null) continue;

            var letter = wordArr[letterIdx].ToString();
            if (letter is null) continue;

            var evaluation = new LetterEvaluation(letter, LetterValueType.Correct, letterIdx, game.CurrentRoundNumber);
            result.Add(evaluation);

            solutionArr[letterIdx] = null;
            wordArr[letterIdx] = null;
        }

        /*Check for yellow letters*/
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            var letter = wordArr[letterIdx];
            var letterString = letter.ToString();
            if (letter is null) continue;
            if (!solutionArr.Contains(letter)) continue;

            if (letterString is null) continue;

            var evaluation = new LetterEvaluation(letterString, LetterValueType.WrongPlacement, letterIdx,
                game.CurrentRoundNumber);

            result.Add(evaluation);

            var idx = solutionArr.IndexOf(letter);
            solutionArr[idx] = null;
            wordArr[letterIdx] = null;
        }

        /* Not present letters */
        foreach (var letterIdx in Enumerable.Range(0, game.Config.WordLength))
        {
            if (wordArr[letterIdx] == null) continue;
            var letter = wordArr[letterIdx].ToString();

            if (letter is null) continue;

            var evaluation = new LetterEvaluation(letter, LetterValueType.Wrong, letterIdx, game.CurrentRoundNumber);

            result.Add(evaluation);
        }

        var test = result.Select(e => e.Letter).ToList().ToString();
        Console.WriteLine($"Evaluation: {test}");
        return result;
    }

    public bool IsCorrectWord(string solution, string word)
    {
        return solution.Equals(word);
    }
}