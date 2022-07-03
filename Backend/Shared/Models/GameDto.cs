using Backend.GameService.Models.Dto;
using Backend.GameService.Models.Enums;

namespace Backend.Shared.Models;

public record GameDto(List<IPlayer> Players, IPlayer HostPlayer, string GameId, GameViewEnum GameViewEnum,
    DateTime? StartedTime,
    DateTime? EndedTime, int? CurrentRoundNumber, List<RoundDto> Rounds,
    List<RoundSubmission> RoundSubmissions, List<PlayerLetterHintsDto> PlayerLetterHints, int MaxPlayers);