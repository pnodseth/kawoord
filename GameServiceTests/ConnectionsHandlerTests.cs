using System.Collections.Generic;
using Backend.GameService.Models;
using Backend.GameService.Models.Enums;
using Backend.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameServiceTests;

public class ConnectionsHandlerTests
{
    [Fact]
    public void AddPlayerConnectionId_Adds_ConnectionId_To_Player_And_ConnectionsDictionary()
    {
        var loggerMock = new Mock<ILogger<ConnectionsHandler>>();
        var connectionsDictMock = new Mock<IConnectionsDictionary>();
        var playerMock = new Mock<IPlayer>();
        playerMock.SetupAllProperties();
        playerMock.SetupProperty(e => e.Id, "0");

        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");
        gameMock.Setup(e => e.FindPlayer(It.IsAny<string>())).Returns(playerMock.Object);
        gameMock.Setup(e => e.Players).Returns(new List<IPlayer> {playerMock.Object});

        var gamePoolMock = new Mock<IGamePool>();
        gamePoolMock.Setup(e => e.FindGame(It.IsAny<string>())).Returns(gameMock.Object);
        var connectionsHandler =
            new ConnectionsHandler(gamePoolMock.Object, loggerMock.Object, connectionsDictMock.Object);

        connectionsHandler.AddPlayerConnectionId(gameMock.Object.GameId, playerMock.Object.Id, "123");

        var actual = playerMock.Object.ConnectionId;
        var expected = "123";

        Assert.Equal(expected, actual);
        connectionsDictMock.Verify(e => e.AddPlayerConnection(It.IsAny<string>(), It.IsAny<IGame>()), Times.Once);
    }

    [Fact]
    public void RemoveGameConnections_Calls_ConnectionDictionary_RemoveAllGameConnections()
    {
        var loggerMock = new Mock<ILogger<ConnectionsHandler>>();
        var connectionsDictMock = new Mock<IConnectionsDictionary>();
        var gamePoolMock = new Mock<IGamePool>();

        var connectionsHandler =
            new ConnectionsHandler(gamePoolMock.Object, loggerMock.Object, connectionsDictMock.Object);
        connectionsHandler.RemoveGameConnections(It.IsAny<string>());

        connectionsDictMock.Verify(e => e.RemoveAllGameConnections(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void HandleDisconnectedPlayer_Calls_Methods()
    {
        var loggerMock = new Mock<ILogger<ConnectionsHandler>>();
        var connectionsDictMock = new Mock<IConnectionsDictionary>();
        var gamePoolMock = new Mock<IGamePool>();
        var gameMock = new Mock<IGame>();

        connectionsDictMock.Setup(e => e.GetGameFromConnectionId(It.IsAny<string>())).Returns(gameMock.Object);


        var connectionsHandler =
            new ConnectionsHandler(gamePoolMock.Object, loggerMock.Object, connectionsDictMock.Object);
        connectionsHandler.HandleDisconnectedPlayer(It.IsAny<string>());

        gameMock.Verify(e => e.DisconnectPlayer(It.IsAny<string>()), Times.Once);
        connectionsDictMock.Verify(e => e.RemovePlayerConnection(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        gamePoolMock.Verify(e => e.RemoveGame(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void HandleDisconnectedPlayer_Sets_GameViewEnum_To_Abandoned()
    {
        var loggerMock = new Mock<ILogger<ConnectionsHandler>>();
        var connectionsDictMock = new Mock<IConnectionsDictionary>();
        var gamePoolMock = new Mock<IGamePool>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();

        connectionsDictMock.Setup(e => e.GetGameFromConnectionId(It.IsAny<string>())).Returns(gameMock.Object);
        connectionsDictMock.Setup(e => e.PlayersConnectedCount(It.IsAny<string>())).Returns(0);

        var connectionsHandler =
            new ConnectionsHandler(gamePoolMock.Object, loggerMock.Object, connectionsDictMock.Object);
        connectionsHandler.HandleDisconnectedPlayer(It.IsAny<string>());

        var expected = GameViewEnum.Abandoned;
        var actual = gameMock.Object.GameViewEnum;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HandleDisconnectedPlayer_Does_Not_Set_GameViewEnum_To_Abandoned()
    {
        var loggerMock = new Mock<ILogger<ConnectionsHandler>>();
        var connectionsDictMock = new Mock<IConnectionsDictionary>();
        var gamePoolMock = new Mock<IGamePool>();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();

        connectionsDictMock.Setup(e => e.GetGameFromConnectionId(It.IsAny<string>())).Returns(gameMock.Object);
        connectionsDictMock.Setup(e => e.PlayersConnectedCount(It.IsAny<string>())).Returns(1);

        var connectionsHandler =
            new ConnectionsHandler(gamePoolMock.Object, loggerMock.Object, connectionsDictMock.Object);
        connectionsHandler.HandleDisconnectedPlayer(It.IsAny<string>());

        var expected = GameViewEnum.Lobby;
        var actual = gameMock.Object.GameViewEnum;
        Assert.Equal(expected, actual);
    }
}