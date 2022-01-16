namespace Backend.Models;

public class GameRepository : IRepository<Game>
{
    public Task Add(Game game)
    {
        throw new NotImplementedException();
    }

    public Task Update(Game entity)
    {
        throw new NotImplementedException();
    }

    public Task<Game> Get(string gameId)
    {
        throw new NotImplementedException();
    }
}