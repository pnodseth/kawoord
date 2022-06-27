using Backend.BotPlayerService.Models;
using Xunit;

namespace BotPlayerServiceTests;

public class BotPlayerGenerator_Tests
{
    [Fact]
    public void GeneratePlayer_Returns_BotPlayer()
    {
        var handler = new BotPlayerGenerator();
        var actual = handler.GeneratePlayer();
        Assert.True(actual.IsBot);
    }

    [Fact]
    public void GenerateBotName_BotPlayer_Has_Name()
    {
        var handler = new BotPlayerGenerator();
        var actual = handler.GeneratePlayer();
        Assert.NotNull(actual.Name);
    }

    [Fact]
    public void GenerateBotName_BotPlayer_Has_Id_EndsWith_BOT()
    {
        var handler = new BotPlayerGenerator();
        var player = handler.GeneratePlayer();
        var actual = player.Id.Split("--")[1];
        const string expected = "BOT";
        Assert.Equal(actual, expected);
    }
}