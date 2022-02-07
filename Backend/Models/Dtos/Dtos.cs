using Backend.Models;

namespace Backend;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, string State, DateTime? StartedTime, DateTime? EndedTime, int? CurrentRoundNumber);

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record PlayerPoints(Player Player, int Points);

public record RoundAndTotalPoints(List<PlayerPoints> RoundPoints, List<PlayerPoints> TotalPoints,
    int ViewLengthSeconds);