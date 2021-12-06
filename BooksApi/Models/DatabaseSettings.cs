using MongoDB.Driver;

namespace BooksApi.Models
{
    public class MongoDBSettings : DatabaseSettings
    {
        public string CollectionName { get; set; }
        
        public MongoClientSettings getMongoClientSettings() =>
            MongoClientSettings.FromConnectionString($"mongodb+srv://{User}:{Password}@{Cluster}{Domain}/{DatabaseName}{ConnectionParameters}");
    }

    public class CassandraDBSettings : DatabaseSettings
    {
        public void getCassandraClientSettings() { }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string Cluster { get; set; }
        public string Domain { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionParameters { get; set; }
    }

    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string Cluster { get; set; }
        string Domain { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string ConnectionParameters { get; set; }
    }
}
