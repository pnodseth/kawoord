using Backend.BotPlayerService.Data;
using Backend.BotPlayerService.Models;
using Moq;
using Xunit;

namespace BotPlayerServiceTests;

public class BotPlayerGeneratorTests
{
    [Fact]
    public void GeneratePlayer_Returns_BotPlayer()
    {
        var botNamesMock = new Mock<IBotNames>();
        var handler = new BotPlayerGenerator(botNamesMock.Object);
        var actual = handler.GeneratePlayer();
        Assert.True(actual.IsBot);
    }

    [Fact]
    public void GenerateBotName_BotPlayer_Has_Name()
    {
        var botNamesMock = new Mock<IBotNames>();
        botNamesMock.Setup(e => e.GetRandomName()).Returns("bot");
        var handler = new BotPlayerGenerator(botNamesMock.Object);
        var actual = handler.GeneratePlayer();
        Assert.NotNull(actual.Name);
    }

    [Fact]
    public void GenerateBotName_BotPlayer_Has_Id_EndsWith_BOT()
    {
        var botNamesMock = new Mock<IBotNames>();
        var handler = new BotPlayerGenerator(botNamesMock.Object);
        var player = handler.GeneratePlayer();
        var actual = player.Id.Split("--")[1];
        const string expected = "BOT";
        Assert.Equal(actual, expected);
    }
}