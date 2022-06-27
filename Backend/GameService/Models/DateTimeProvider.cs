namespace Backend.GameService.Models;

public interface IDateTimeProvider
{
    DateTime GetNowUtc();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNowUtc()
    {
        return DateTime.UtcNow;
    }
}