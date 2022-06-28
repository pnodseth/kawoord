using Backend.GameService.Models;
using Moq;
using Xunit;

namespace GameServiceTests;

public class ConnectionsDictionaryTests
{
    [Fact]
    public void AddPlayerConnection_Adds_Player_To_Dictionary()
    {
        var connectionsDictionary = new ConnectionsDictionary();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        connectionsDictionary.AddPlayerConnection("0", gameMock.Object);
        var actual = connectionsDictionary.PlayersConnectedCount("0");
        var expected = 1;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RemovePlayerConnection_Removes_Player_From_Dictionary()
    {
        var connectionsDictionary = new ConnectionsDictionary();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        connectionsDictionary.AddPlayerConnection("0", gameMock.Object);
        connectionsDictionary.RemovePlayerConnection(gameMock.Object.GameId, "0");
        var actual = connectionsDictionary.PlayersConnectedCount(gameMock.Object.GameId);
        var expected = 0;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetGameFromConnectionId__Returns_Correct_Game()
    {
        var connectionsDictionary = new ConnectionsDictionary();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        connectionsDictionary.AddPlayerConnection("0", gameMock.Object);
        var actual = connectionsDictionary.GetGameFromConnectionId("0");
        var expected = gameMock.Object;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PlayersConnectedCount_Returns_Correct_Player_Count()
    {
        var connectionsDictionary = new ConnectionsDictionary();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        connectionsDictionary.AddPlayerConnection("0", gameMock.Object);
        connectionsDictionary.AddPlayerConnection("1", gameMock.Object);

        var actual = connectionsDictionary.PlayersConnectedCount(gameMock.Object.GameId);
        const int expected = 2;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RemoveAllGameConnections_Removes_Game_From_Dictionary()
    {
        var connectionsDictionary = new ConnectionsDictionary();
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.GameId).Returns("0");

        var utilsMock = new Mock<IUtils>();
        utilsMock.Setup(e => e.GenerateGameId()).Returns("0");

        connectionsDictionary.AddPlayerConnection("0", gameMock.Object);
        connectionsDictionary.AddPlayerConnection("1", gameMock.Object);
        connectionsDictionary.RemoveAllGameConnections(gameMock.Object.GameId);

        var actual = connectionsDictionary.PlayersConnectedCount(gameMock.Object.GameId);
        const int expected = 0;
        Assert.Equal(expected, actual);
    }
}