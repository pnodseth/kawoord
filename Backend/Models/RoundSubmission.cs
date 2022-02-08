namespace Backend.Models;

public record RoundSubmission(Player Player, int Round, string Word, DateTime SubmittedAtUtc, List<LetterEvaluation> LetterEvaluations,
    bool IsCorrectWord);