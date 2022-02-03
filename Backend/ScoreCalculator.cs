using Backend.Models;

namespace Backend;

public static class ScoreCalculator
{
    public static int CalculateSubmissionScore(Game game, string word)
    {
        return 10;
    }

    public static bool IsCorrectWord(Game game, string word)
    {
        return game.Solution.Equals(word);
    }
}