namespace Backend.Models;

public record RoundSubmission(Player Player, int Round, string Word, DateTime SubmittedAtUtc, int Score,
    bool IsCorrectWord);