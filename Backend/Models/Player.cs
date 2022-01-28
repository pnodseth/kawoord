namespace Backend.Models;

public class Player
{
    public Player(string name, string id)
    {
        Name = name;
        Id = id;

    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string? ConnectionId { get; set; }
}