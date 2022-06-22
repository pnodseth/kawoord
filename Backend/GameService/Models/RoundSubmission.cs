namespace Backend.GameService.Models;

public record RoundSubmission(Player Player, int RoundNumber, string Word, DateTime SubmittedAtUtc,
    List<LetterEvaluation> LetterEvaluations,
    bool IsCorrectWord);