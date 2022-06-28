namespace Backend.GameService.Models;

public interface IRandomProvider
{
    int RandomFromMinMax(int min, int max);
}

public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int RandomFromMinMax(int min, int max)
    {
        return _random.Next(min, max);
    }

    public int RandomFromMax(int max)
    {
        return _random.Next(max);
    }
}