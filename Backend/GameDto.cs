using Backend.Models;

namespace Backend;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, string State, DateTime? StartedTime, DateTime? EndedTime, int? CurrentRoundNumber);