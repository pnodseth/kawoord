using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend;

public class Round
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly GameRepository _repository;

    public Round(IHubContext<Hub> hubContext, GameRepository repository, Game game, int roundNumber,
        int roundLengthSeconds, DateTime startsAtUtc)
    {
        _hubContext = hubContext;
        _repository = repository;
        Game = game;
        RoundNumber = roundNumber;
        RoundLengthSeconds = roundLengthSeconds;
        StartsAtUtc = startsAtUtc;

        CalculateFields();
    }
    [BsonIgnore]
    private Game Game { get; }
    private int RoundNumber { get; set; }
    private int RoundLengthSeconds { get; set; }
    private DateTime StartsAtUtc { get; set; }
    private DateTime EndsAtUtc { get; set; }
    private int SummaryLengthSeconds { get; set; }
    private DateTime RoundCompletedUtc { get; set; }

    private System.Timers.Timer _eventTimer;

    public void Start()
    {
        SetRoundStarted();
        SetTimer();
    }

    private void CalculateFields()
    {
        EndsAtUtc = StartsAtUtc.AddSeconds(RoundLengthSeconds).ToUniversalTime();
        RoundCompletedUtc = EndsAtUtc.AddSeconds(SummaryLengthSeconds).ToUniversalTime();
    }

    public void SetTimer()
    {
        var now = DateTime.UtcNow;
        var eventUtc = EndsAtUtc;
        var timespanUntilEvent = eventUtc - now;
        Console.WriteLine($"triggers in: {timespanUntilEvent.Seconds}");
        _eventTimer = new System.Timers.Timer(timespanUntilEvent.Seconds * 1000);
        _eventTimer.Elapsed += OnTimedEvent;
        _eventTimer.AutoReset = false;
        _eventTimer.Enabled = true;
    }

    private async void SetRoundStarted()
    {
        Console.WriteLine($"Starting round {Game.CurrentRoundNumber}");
        await _repository.Update(Game);
        var roundInfo = new RoundInfo(Game.CurrentRoundNumber, Game.Config.RoundLengthSeconds, EndsAtUtc);
        await _hubContext.Clients.Group(Game.GameId).SendAsync("round-info", roundInfo);
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", RoundState.Started);
    }

    private async void SetRoundEndedEvent()
    {
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("round-state", RoundState.Summary);
        
        //todo: Send points
        // Schedule next round from here.
        var points = await GetRoundSummary();
        await _hubContext.Clients.Group(Game.GameId)
            .SendAsync("points", points);
    }

    // Triggers when Summary event should be sent
    private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        SetRoundEndedEvent();
    }
    
    private async Task<RoundAndTotalPoints> GetRoundSummary()
    {
        //SEND ROUND STATE **SUMMARY** ROUND 1
        var game = await _repository.Get(Game.GameId);
        var roundPoints = game.RoundSubmissions.Where(r => r.Round == game.CurrentRoundNumber).Select(e => new PlayerPoints(e.Player, e.Score)).ToList();
        var points = new RoundAndTotalPoints(roundPoints, roundPoints, 7);
        return points;
    }
}