using Backend.Models;

namespace Backend;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, string State, DateTime? StartedTime, DateTime? EndedTime, int? CurrentRoundNumber, List<RoundInfo> Rounds, RoundState CurrentRoundState);

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record WordEvaluation(Player Player, List<LetterEvaluation> Evaluation, bool isCorrectWord, DateTime SubmittedDateTime);

