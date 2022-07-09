using System.Threading.Tasks;
using Backend.GameService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Data;
using Backend.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameServiceTests;

public class GameHandlerTests
{
    [Fact]
    public void SetupNewGame_Should_Call_Methods()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig());
        gameMock.SetupProperty(e => e.Config.Public, true);
        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        gameHandler.SetupNewGame(gameMock.Object, playerMock.Object);

        gamePoolMock.Verify(e => e.AddGame(It.IsAny<IGame>()), Times.Once);
        gameMock.Verify(e => e.AddPlayer(It.IsAny<IPlayer>(), true), Times.Once);
    }

    [Fact]
    public void SetupNewGame_Should_Add_Public_Games_To_Queue()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig());
        gameMock.SetupProperty(e => e.Config.Public, true);
        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        gameHandler.SetupNewGame(gameMock.Object, playerMock.Object);

        gamePoolMock.Verify(e => e.AddToAvailableGames(It.IsAny<IGame>()), Times.Once);
    }

    [Fact]
    public void SetupNewGame_Should_Not_Add_Private_Games_To_Public_Queue()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig());
        gameMock.SetupProperty(e => e.Config.Public, false);
        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        gameHandler.SetupNewGame(gameMock.Object, playerMock.Object);

        gamePoolMock.Verify(e => e.AddToAvailableGames(It.IsAny<IGame>()), Times.Never);
    }

    [Fact]
    public void AddPlayerWithGameId_Should_Add_Player_To_Game()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig());

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        gameHandler.AddPlayerWithGameId(playerMock.Object, It.IsAny<string>());

        gameMock.Verify(e => e.AddPlayer(It.IsAny<IPlayer>(), false), Times.Once);
    }

    [Fact]
    public void AddPlayerWithGameId_Should_Publish_Updates()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig());

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var playerMock = new Mock<IPlayer>();

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        gameHandler.AddPlayerWithGameId(playerMock.Object, It.IsAny<string>());

        gamePublisherMock.Verify(e => e.PublishPlayerJoined(It.IsAny<IGame>(), It.IsAny<IPlayer>()), Times.Once);
        gamePublisherMock.Verify(e => e.PublishUpdatedGame(It.IsAny<IGame>()), Times.Once);
    }

    [Theory]
    [InlineData(GameViewEnum.Solved)]
    [InlineData(GameViewEnum.EndedUnsolved)]
    [InlineData(GameViewEnum.Abandoned)]
    public async Task RunGame_Should_Call_Methods_When_GameState_Is_Correct(GameViewEnum gameViewEnum)
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");
        gameMock.SetupGet(e => e.HostPlayer).Returns(playerMock.Object);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig
            {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 1});

        gameMock.SetupProperty(e => e.GameViewEnum, gameViewEnum);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);


        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.RunGame(gameMock.Object);

        gameMock.Verify(e => e.RunGame(), Times.Once);
        connectionsHandlerMock.Verify(e => e.RemoveGameConnections(gameMock.Object.GameId), Times.Once);
        gamePoolMock.Verify(e => e.RemoveGame(It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData(GameViewEnum.Lobby)]
    public async Task RunGame_Should_Not_Call_Methods_When_GameState_Is_Correct(GameViewEnum gameViewEnum)
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");
        gameMock.SetupGet(e => e.HostPlayer).Returns(playerMock.Object);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig
            {RoundLengthSeconds = 1, RoundSummaryLengthSeconds = 1, NumberOfRounds = 1});

        gameMock.SetupProperty(e => e.GameViewEnum, gameViewEnum);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);


        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.RunGame(gameMock.Object);

        connectionsHandlerMock.Verify(e => e.RemoveGameConnections(gameMock.Object.GameId), Times.Never);
        gamePoolMock.Verify(e => e.RemoveGame(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("", "", "")]
    public async Task SubmitWord_Should_Invalidate_Bad_Input(string playerId, string gameId, string word)
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        var actual = await gameHandler.SubmitWord(playerId, gameId, word);

        // todo: not working: https://github.com/dotnet/aspnetcore/issues/37502
        // Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Theory]
    [InlineData(GameViewEnum.Abandoned)]
    [InlineData(GameViewEnum.Lobby)]
    [InlineData(GameViewEnum.Solved)]
    [InlineData(GameViewEnum.EndedUnsolved)]
    public async Task SubmitWord_Incorrect_GameState_Should_Invalidate(GameViewEnum gameViewEnum)
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, gameViewEnum);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        var actual = await gameHandler.SubmitWord("0", "0", "0");

        // todo: not working: https://github.com/dotnet/aspnetcore/issues/37502
        // Assert.IsType<IResult>(actual);
        // Assert.IsAssignableFrom(BadHttpRequestException);
    }

    [Fact]
    public async Task SubmitWord_Invalid_Word_Should_Invalidate()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(false);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        var actual = await gameHandler.SubmitWord("0", "0", "aaaaa");

        // todo: not working: https://github.com/dotnet/aspnetcore/issues/37502
        // Assert.IsAssignableFrom<BadHttpRequestException>(actual);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("aasd")]
    [InlineData("aasdf")]
    [InlineData("aasdfas")]
    public async Task SubmitWord_Incorrect_Word_Length_Should_Invalidate(string word)
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(false);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", word);

        // todo: not working: https://github.com/dotnet/aspnetcore/issues/37502
        // Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task SubmitWord_Validated_Inputs_Should_Add_RoundSubmission()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.Setup(e => e.GetCurrentRoundSubmissionsCount()).Returns(1);
        gameMock.SetupGet(e => e.PlayerAndBotCount).Returns(1);

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(true);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", "aaaaa");

        gameMock.Verify(e => e.AddRoundSubmission(It.IsAny<IPlayer>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SubmitWord_Validated_Inputs_Should_Add_PlayerLetterHints()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.Setup(e => e.GetCurrentRoundSubmissionsCount()).Returns(1);
        gameMock.SetupGet(e => e.PlayerAndBotCount).Returns(1);

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(true);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", "aaaaa");

        gameMock.Verify(e => e.AddPlayerLetterHints(It.IsAny<IPlayer>()), Times.Once);
    }

    [Fact]
    public async Task SubmitWord_Validated_Inputs_Should_Publish_Updates()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");
        playerMock.SetupProperty(e => e.ConnectionId, "0");

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.Setup(e => e.GetCurrentRoundSubmissionsCount()).Returns(1);
        gameMock.SetupGet(e => e.PlayerAndBotCount).Returns(1);

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(true);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", "aaaaa");

        gamePublisherMock.Verify(e => e.PublishUpdatedGame(It.IsAny<IGame>()), Times.Once);
        gamePublisherMock.Verify(e => e.PublishWordSubmitted(It.IsAny<string>(), It.IsAny<IPlayer>()), Times.Once);
    }

    [Fact]
    public async Task SubmitWord_Round_Should_End_Early_When_All_Players_Submitted()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");
        playerMock.SetupProperty(e => e.ConnectionId, "0");

        var roundMock = new Mock<IRound>();

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.Setup(e => e.GetCurrentRoundSubmissionsCount()).Returns(1);
        gameMock.SetupGet(e => e.PlayerAndBotCount).Returns(1);
        gameMock.SetupGet(e => e.CurrentRound).Returns(roundMock.Object);

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(true);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", "aaaaa");

        roundMock.Verify(e => e.EndRoundEndEarly(), Times.Once);
    }

    [Fact]
    public async Task SubmitWord_Round_Should_Not_End_Early_When_Not_All_Players_Submitted()
    {
        var loggerMock = new Mock<ILogger<GameHandler>>();
        var connectionsHandlerMock = new Mock<IConnectionsHandler>();
        var gamePublisherMock = new Mock<IGamePublisher>();
        var validWordsMock = new Mock<IValidWords>();
        var gamePoolMock = new Mock<IGamePool>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupProperty(e => e.Id, "0");
        playerMock.SetupProperty(e => e.ConnectionId, "0");

        var roundMock = new Mock<IRound>();

        var gameMock = new Mock<IGame>();
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.SetupProperty(e => e.GameViewEnum, GameViewEnum.Started);
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.Setup(e => e.GetCurrentRoundSubmissionsCount()).Returns(1);
        gameMock.SetupGet(e => e.PlayerAndBotCount).Returns(2);
        gameMock.SetupGet(e => e.CurrentRound).Returns(roundMock.Object);

        validWordsMock.Setup(e => e.IsValidWord(It.IsAny<string>())).Returns(true);

        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);

        var gameHandler = new GameHandler(gamePoolMock.Object, loggerMock.Object, connectionsHandlerMock.Object,
            gamePublisherMock.Object, validWordsMock.Object);

        await gameHandler.SubmitWord("0", "0", "aaaaa");

        roundMock.Verify(e => e.EndRoundEndEarly(), Times.Never);
    }
}