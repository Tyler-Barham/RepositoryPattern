using MongoDB.Driver;

namespace BooksApi.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public MongoClientSettings MongoConnectionString { get { return MongoClientSettings.FromConnectionString(ConnectionString); } }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        MongoClientSettings MongoConnectionString { get; }
        string DatabaseName { get; set; }
    }
}
