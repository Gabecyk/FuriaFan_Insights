using MongoDB.Driver;
using FuriaAPI.Models;

namespace FuriaAPI.Services
{

    public class MongoDbService
    {
        private readonly IMongoCollection<Fan> _fansCollection;
        private readonly IConfiguration _configuration; 

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration; 

            var connectionString = _configuration["MONGODB_CONNECTION_STRING"]; 
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("furia_db");
            _fansCollection = database.GetCollection<Fan>("fans");
        }

        public async Task CreateAsync(Fan newFan)
        {
            await _fansCollection.InsertOneAsync(newFan);
        }
    }
}