using Backend.Models.Enums;

namespace Backend.Models.Dtos;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, GameViewEnum GameViewEnum,
    DateTime? StartedTime,
    DateTime? EndedTime, int? CurrentRoundNumber, List<RoundInfo> Rounds, RoundViewEnum RoundViewEnum,
    List<RoundSubmission> RoundSubmissions, List<PlayerLetterHintsDto> PlayerLetterHints);

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record WordEvaluation(Player Player, List<LetterEvaluation> Evaluation, bool IsCorrectWord,
    DateTime SubmittedDateTime, int RoundNumber);

public record PlayerLetterHintsDto(Player Player, List<LetterEvaluation> Correct, List<LetterEvaluation> WrongPosition,
    List<LetterEvaluation> Wrong, int RoundNumber);