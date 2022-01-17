namespace Backend.Models;

public class DbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string GamesCollectionName { get; set; } = null!;
}