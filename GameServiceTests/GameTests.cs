using System;
using System.Threading.Tasks;
using Backend.BotPlayerService.Models;
using Backend.GameService.Models;
using Backend.GameService.Models.Enums;
using Backend.GameService.Providers;
using Backend.Shared.Data;
using Backend.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameServiceTests;

public class GameTests
{
    [Fact]
    public async Task RunGame_Should_End_In_Unsolved_State()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var date = new DateTime(2010, 10, 10, 10, 10, 10);
        dateTimeProviderMock.Setup(e => e.GetNowUtc()).Returns(date);

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");


        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config =
            {
                RoundLengthSeconds = 1,
                RoundSummaryLengthSeconds = 1,
                NumberOfRounds = 1
            }
        };
        await game.RunGame();

        var expected = GameViewEnum.EndedUnsolved;
        var actual = game.GameViewEnum;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_Have_Correct_EndTime()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var date = new DateTime(2010, 10, 10, 10, 10, 10);
        dateTimeProviderMock.Setup(e => e.GetNowUtc()).Returns(date);

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config =
            {
                RoundLengthSeconds = 1,
                RoundSummaryLengthSeconds = 1,
                NumberOfRounds = 1
            }
        };
        await game.RunGame();

        var expected = date;
        var actual = game.EndedAtUtc;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_Have_Correct_StartTime()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var date = new DateTime(2010, 10, 10, 10, 10, 10);
        dateTimeProviderMock.Setup(e => e.GetNowUtc()).Returns(date);

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config =
            {
                RoundLengthSeconds = 1,
                RoundSummaryLengthSeconds = 1,
                NumberOfRounds = 1
            }
        };
        await game.RunGame();

        var expected = date;
        var actual = game.StartedAtUtc;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_Publish_Updated_Game()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var date = new DateTime(2010, 10, 10, 10, 10, 10);
        dateTimeProviderMock.Setup(e => e.GetNowUtc()).Returns(date);

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config =
            {
                NumberOfRounds = 0
            }
        };

        await game.RunGame();

        publisherMock.Verify(e => e.PublishUpdatedGame(It.IsAny<IGame>()), Times.AtLeastOnce);
    }

    [Fact]
    public void FindPlayer_Finds_Player()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddPlayer(playerMock.Object);

        var actual = game.FindPlayer(playerMock.Object.Id);
        var expected = playerMock.Object;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddPlayer_Adds_HostPlayer()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddPlayer(playerMock.Object, true);

        var actual = game.HostPlayer;
        var expected = playerMock.Object;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddPlayer_Adds_Player()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddPlayer(playerMock.Object, true);

        var actual = game.Players.Count;
        const int expected = 1;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RemovePlayerWithConnectionId_Removes_Player()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.ConnectionId, "0");

        game.AddPlayer(playerMock.Object, true);
        game.RemovePlayerWithConnectionId("0");
        var actual = game.Players.Count;
        const int expected = 0;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddPlayerLetterHints_Should_Add_PlayerLetterHints()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddPlayerLetterHints(playerMock.Object);

        var actual = game.PlayerLetterHints.Count;
        var expected = 1;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddRoundSubmission_Should_Add_RoundSubmission()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddRoundSubmission(playerMock.Object, It.IsAny<string>());
        var actual = game.RoundSubmissions.Count;
        const int expected = 1;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_End_With_Correct_CurrentRoundNumber()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config = {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 2}
        };

        await game.RunGame();
        var actual = game.CurrentRoundNumber;
        const int expected = 2;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_Request_Submissions_From_BotPlayers()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config = {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 1}
        };


        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");
        playerMock.SetupProperty(e => e.IsBot, true);
        game.AddPlayer(playerMock.Object);

        await game.RunGame();
        botPlayerHandlerMock.Verify(e => e.RequestBotsRoundSubmission(It.IsAny<IGame>()), Times.Exactly(1));
    }

    [Fact]
    public async Task RunGame_Should_Not_Request_Submissions_From_BotPlayers_When_There_Are_None()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config = {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 1}
        };


        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");
        game.AddPlayer(playerMock.Object);

        await game.RunGame();
        botPlayerHandlerMock.Verify(e => e.RequestBotsRoundSubmission(It.IsAny<IGame>()), Times.Never);
    }

    [Fact]
    public void AddRoundSubmission_Should_Set_GameView_To_Solved_When_Correct_Word_Submitted()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var calculatorMock = new Mock<IScoreCalculator>();
        calculatorMock.Setup(e => e.IsCorrectWord(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var loggerMock = new Mock<ILogger<Game>>();

        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object);

        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        game.AddRoundSubmission(playerMock.Object, It.IsAny<string>());

        var actual = game.GameViewEnum;
        var expected = GameViewEnum.Solved;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RunGame_Should_End_In_Solved_State_If_Correct_Word_Was_Submitted()
    {
        var publisherMock = new Mock<IGamePublisher>();
        var botPlayerHandlerMock = new Mock<IBotPlayerHandler>();
        var loggerMock = new Mock<ILogger<Game>>();

        var calculatorMock = new Mock<IScoreCalculator>();
        calculatorMock.Setup(e => e.IsCorrectWord(It.IsAny<string>(), It.IsAny<string>())).Returns(true);


        var solutionWordsMock = new Mock<ISolutionWords>();
        solutionWordsMock.Setup(e => e.GetRandomSolution()).Returns("mock");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var game = new Game(publisherMock.Object, botPlayerHandlerMock.Object, calculatorMock.Object,
            solutionWordsMock.Object, loggerMock.Object,
            utilsMock.Object, dateTimeProviderMock.Object)
        {
            Config = {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 1}
        };


        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");
        game.AddPlayer(playerMock.Object);
        game.AddRoundSubmission(playerMock.Object, It.IsAny<string>());

        await game.RunGame();

        var actual = game.GameViewEnum;
        var expected = GameViewEnum.Solved;
    }
}