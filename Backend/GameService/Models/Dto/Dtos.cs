using Backend.Shared.Models;

namespace Backend.GameService.Models.Dto;

public record WordEvaluation(Player Player, List<LetterEvaluation> Evaluation, bool IsCorrectWord,
    DateTime SubmittedDateTime, int RoundNumber);

public record PlayerLetterHintsDto(IPlayer Player, List<LetterEvaluation> Correct, List<LetterEvaluation> WrongPosition,
    List<LetterEvaluation> Wrong, int RoundNumber);