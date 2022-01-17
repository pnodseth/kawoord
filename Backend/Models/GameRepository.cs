using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend.Models;

public class GameRepository : IRepository<Game>
{
    private IMongoCollection<Game> _gamesCollection;
    public GameRepository(IOptions<DbSettings> dbSettings)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _gamesCollection = mongoDb.GetCollection<Game>(dbSettings.Value.GamesCollectionName);
    }
    
    public async Task Add(Game game)
    {
        await _gamesCollection.InsertOneAsync(game);
    }

    public async Task Update(Game entity)
    {
        await _gamesCollection.ReplaceOneAsync(x => x.GameId == entity.GameId, entity);
    }

    public async Task<Game> Get(string gameId)
    {
       var result =  await _gamesCollection.FindAsync(x => x.GameId == gameId);
       return await result.FirstOrDefaultAsync();


    }
}