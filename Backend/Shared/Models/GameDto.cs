using Backend.GameService.Models;
using Backend.GameService.Models.Dtos;
using Backend.GameService.Models.Enums;

namespace Backend.Shared.Models;

public record GameDto(List<Player> Players, Player HostPlayer, string GameId, GameViewEnum GameViewEnum,
    DateTime? StartedTime,
    DateTime? EndedTime, int? CurrentRoundNumber, List<RoundInfo> Rounds, RoundViewEnum RoundViewEnum,
    List<RoundSubmission> RoundSubmissions, List<PlayerLetterHintsDto> PlayerLetterHints);