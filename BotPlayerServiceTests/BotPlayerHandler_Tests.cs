using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Backend.BotPlayerService.Models;
using Backend.GameService.Models;
using Backend.GameService.Models.Dto;
using Backend.GameService.Providers;
using Backend.Shared.Data;
using Backend.Shared.Models;
using Moq;
using Xunit;

namespace BotPlayerServiceTests;

public class BotPlayerHandlerTests
{
    [Fact]
    public async Task RequestBotPlayersToGame_Should_Add_Players_To_GameHandler()
    {
        var gameHandlerMock = new Mock<IGameHandler>();
        var solutionWordsMock = new Mock<ISolutionWords>();
        var validWordsMock = new Mock<IValidWords>();
        var randomProviderMock = new Mock<IRandomProvider>();
        var botPlayerGeneratorMock = new Mock<IBotPlayerGenerator>();

        var botHandler = new BotPlayerHandler(gameHandlerMock.Object, solutionWordsMock.Object, validWordsMock.Object,
            randomProviderMock.Object, botPlayerGeneratorMock.Object);
        await botHandler.RequestBotPlayersToGame("0", 2, 0, 100);
        gameHandlerMock.Verify(foo => foo.AddPlayerWithGameId(It.IsAny<Player>(), "0"), Times.Exactly(2));
    }

    [Fact]
    public async Task RequestBotsRoundSubmission_To_Submit_Words()
    {
        var gameHandlerMock = new Mock<IGameHandler>();
        var solutionWordsMock = new Mock<ISolutionWords>();
        var validWordsMock = new Mock<IValidWords>();
        var gameMock = new Mock<IGame>();
        var player = new Mock<Player>(It.IsAny<string>(), It.IsAny<string>());
        var roundMock = new Mock<Round>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var botPlayers = new List<IPlayer> {player.Object};
        var randomProviderMock = new Mock<IRandomProvider>();
        var botPlayerGeneratorMock = new Mock<IBotPlayerGenerator>();

        randomProviderMock.Setup(e => e.RandomFromMinMax(It.IsAny<int>(), It.IsAny<int>())).Returns(1);
        roundMock.SetupAllProperties();
        dateTimeProviderMock.Setup(e => e.GetNowUtc()).Returns(new DateTime());
        gameMock.SetupAllProperties();
        gameMock.Setup(e => e.CurrentRound).Returns(roundMock.Object);
        gameMock.SetupGet(e => e.BotPlayers).Returns(botPlayers);
        gameMock.Setup(e => e.PlayerLetterHints).Returns(new List<PlayerLetterHintsDto>());
        validWordsMock.Setup(e => e.GetRandomWord()).Returns(It.IsAny<string>());

        // Because we use Task.Run inside botHandler.RequestBotsRoundSubmission which is not awaited, we have to do this 
        var sendCalled = new ManualResetEvent(false);
        gameHandlerMock.Setup(e => e.SubmitWord(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(
            () => { sendCalled.Set(); });

        var botHandler = new BotPlayerHandler(gameHandlerMock.Object, solutionWordsMock.Object, validWordsMock.Object,
            randomProviderMock.Object, botPlayerGeneratorMock.Object);

        await botHandler.RequestBotPlayersToGame(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

        botHandler.RequestBotsRoundSubmission(gameMock.Object);

        Assert.True(sendCalled.WaitOne(TimeSpan.FromSeconds(3)), "Send was never called");
        gameHandlerMock.Verify(foo => foo.SubmitWord(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.AtLeastOnce);
    }
}