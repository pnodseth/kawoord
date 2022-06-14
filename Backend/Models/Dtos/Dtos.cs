namespace Backend.Models.Dtos;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, GameViewEnum GameViewEnum,
    DateTime? StartedTime,
    DateTime? EndedTime, int? CurrentRoundNumber, List<RoundInfo> Rounds, RoundViewEnum RoundViewEnum,
    List<RoundSubmission> RoundSubmissions, List<PlayerLetterHintsDTO> PlayerLetterHints);

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record WordEvaluation(Player Player, List<LetterEvaluation> Evaluation, bool IsCorrectWord,
    DateTime SubmittedDateTime, int RoundNumber);

public record PlayerLetterHintsDTO(Player Player, List<LetterEvaluation> Correct, List<LetterEvaluation> WrongPosition);