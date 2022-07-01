using System.Linq;
using Backend.GameService.Models;
using Moq;
using Xunit;

namespace GameServiceTests;

public class ScoreCalculatorTests
{
    [Theory]
    [InlineData("aaaaa", 5)]
    [InlineData("abaaa", 4)]
    [InlineData("abcaa", 3)]
    [InlineData("abcda", 2)]
    [InlineData("abcde", 1)]
    [InlineData("bbcde", 0)]
    public void CalculateLetterEvaluations_Should_Return_Correct_Amount_Of_Correct_Letters(string submittedWord,
        int expected)
    {
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.Solution).Returns("aaaaa");
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.SetupGet(e => e.CurrentRoundNumber).Returns(1);

        var scoreCalculator = new ScoreCalculator();

        var letterEvaluations = scoreCalculator.CalculateLetterEvaluations(gameMock.Object, submittedWord);
        var actual = letterEvaluations.Count(e => e.LetterValueType.Value == "Correct");

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("abcde", 0)]
    [InlineData("bacde", 2)]
    [InlineData("badce", 4)]
    public void CalculateLetterEvaluations_Should_Return_Correct_Amount_Of_WrongPosition_Letters(string submittedWord,
        int expected)
    {
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.Solution).Returns("abcde");
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.SetupGet(e => e.CurrentRoundNumber).Returns(1);

        var scoreCalculator = new ScoreCalculator();

        var letterEvaluations = scoreCalculator.CalculateLetterEvaluations(gameMock.Object, submittedWord);
        var actual = letterEvaluations.Count(e => e.LetterValueType.Value == "WrongPlacement");

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("affff", 4)]
    [InlineData("abfff", 3)]
    [InlineData("abcff", 2)]
    [InlineData("abcdf", 1)]
    public void CalculateLetterEvaluations_Should_Return_Correct_Amount_Of_Wrong_Letters(string submittedWord,
        int expected)
    {
        var gameMock = new Mock<IGame>();
        gameMock.SetupAllProperties();
        gameMock.SetupGet(e => e.Solution).Returns("abcde");
        gameMock.SetupGet(e => e.Config).Returns(new GameConfig {WordLength = 5});
        gameMock.SetupGet(e => e.CurrentRoundNumber).Returns(1);

        var scoreCalculator = new ScoreCalculator();

        var letterEvaluations = scoreCalculator.CalculateLetterEvaluations(gameMock.Object, submittedWord);
        var actual = letterEvaluations.Count(e => e.LetterValueType.Value == "Wrong");

        Assert.Equal(expected, actual);
    }
}