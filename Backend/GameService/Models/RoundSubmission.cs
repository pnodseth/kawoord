using Backend.Shared.Models;

namespace Backend.GameService.Models;

public record RoundSubmission(IPlayer Player, int RoundNumber, string Word, DateTime SubmittedAtUtc,
    List<LetterEvaluation> LetterEvaluations,
    bool IsCorrectWord);