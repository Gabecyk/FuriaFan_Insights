using MongoDB.Driver;
using FuriaAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

namespace FuriaAPI.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Fan> _fansCollection;
        private readonly IConfiguration _configuration;

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            string connectionString = null;
            string databaseName = null;

            // Tenta ler da estrutura JSON em appsettings.json (para local)
            var mongoDbConfig = _configuration.GetSection("MONGODB_CONNECTION_STRING");
            if (mongoDbConfig.Exists())
            {
                connectionString = mongoDbConfig.GetValue<string>("ConnectionString");
                databaseName = mongoDbConfig.GetValue<string>("DatabaseName");
            }

            // Se não encontrou na estrutura JSON, tenta ler de variáveis de ambiente separadas (para Render)
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
                databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão do MongoDB não foi configurada (nem em 'MONGODB_CONNECTION_STRING' no appsettings nem na variável de ambiente 'MONGODB_CONNECTION_STRING').");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("O nome do banco de dados do MongoDB não foi configurado (nem em 'MONGODB_CONNECTION_STRING' no appsettings nem na variável de ambiente 'MONGODB_DATABASE_NAME').");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _fansCollection = database.GetCollection<Fan>("fans");
        }

        public async Task CreateAsync(Fan newFan)
        {
            await _fansCollection.InsertOneAsync(newFan);
        }
    }
}