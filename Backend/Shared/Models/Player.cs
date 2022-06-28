namespace Backend.Shared.Models;

public interface IPlayer
{
    string Id { get; set; }
    string Name { get; set; }
    string? ConnectionId { get; set; }
    bool IsBot { get; set; }
}

public class Player : IPlayer
{
    public Player(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string? ConnectionId { get; set; }
    public bool IsBot { get; set; } = false;
}