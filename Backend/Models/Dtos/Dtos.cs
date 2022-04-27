using Backend.Models;

namespace Backend;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, GameStateEnum GameStateEnum,
    DateTime? StartedTime,
    DateTime? EndedTime, int? CurrentRoundNumber, List<RoundInfo> Rounds, RoundStateEnum RoundStateEnum);

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record WordEvaluation(Player Player, List<LetterEvaluation> Evaluation, bool isCorrectWord,
    DateTime SubmittedDateTime, int RoundNumber);