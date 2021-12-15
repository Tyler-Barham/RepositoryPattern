using MongoDB.Driver;

namespace BooksApi.Repositories
{
    public class MongoDBSettings : DatabaseSettings
    {
        public string CollectionName { get; set; }
        public string Cluster { get; set; }
        public string Domain { get; set; }
        public string ConnectionParameters { get; set; }

        public MongoClientSettings GetMongoClientSettings() =>
            MongoClientSettings.FromConnectionString($"mongodb+srv://{User}:{Password}@{Cluster}{Domain}/{DatabaseName}{ConnectionParameters}");
    }

    public class CassandraDBSettings : DatabaseSettings
    {
        public string Keyspace { get; set; }
        public string TableName { get; set; }
        public string TableSchema { get; set; }
        public string ConnectionZip { get; set; }

        public void GetCassandraClientSettings() { }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string User { get; set; }
        string Password { get; set; }
    }
}
