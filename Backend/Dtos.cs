using Backend.Models;

namespace Backend;

public record RoundInfo(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc);

public record PlayerPoints(Player Player, int Points);

public record RoundAndTotalPoints(List<PlayerPoints> RoundPoints, List<PlayerPoints> TotalPoints,
    int ViewLengthSeconds);