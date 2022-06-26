using Backend.GameService.Models.Enums;

namespace Backend.Shared.Models;

public record RoundDto(int RoundNumber, int RoundLengthSeconds, DateTime RoundEndsUtc, RoundViewEnum RoundViewEnum);