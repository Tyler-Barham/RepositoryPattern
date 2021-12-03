using MongoDB.Driver;

namespace BooksApi.Models
{
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string BooksCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public MongoClientSettings MongoConnectionString { get { return MongoClientSettings.FromConnectionString(ConnectionString); } }
        public string DatabaseName { get; set; }
    }

    public interface IBookstoreDatabaseSettings
    {
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        MongoClientSettings MongoConnectionString { get; }
        string DatabaseName { get; set; }
    }
}
