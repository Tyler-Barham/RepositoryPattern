using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BooksApi.Controllers;
using BooksApi.Models;
using BooksApi.Repositories;
using BooksApi.Services;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BooksApi.UnitTests.Utilities
{
    public class Initializers
    {
        private enum RepoType
        {
            RAM,
            Mongo,
            Cassandra
        }

        public static ServiceProvider GetServiceProvider()
            => (ServiceProvider)Program.BuildWebHost(new string[] { }).Services;

        public static BookService GetBookService(int numEntities = 0)
        {

            IRepository<Book> repo = new RAMRepository<Book>();

            // Tests cannot be run in parallel if using mongo or cassandra.
            RepoType repoType = RepoType.RAM;

            if (repoType == RepoType.Mongo)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
                var mongoSettings = config.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();
                repo = new MongoRepository<Book>(mongoSettings);
            }
            else if (repoType == RepoType.Cassandra)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
                var cassandraSettings = config.GetSection(nameof(CassandraDBSettings)).Get<CassandraDBSettings>();
                repo = new CassandraRepository<Book>(cassandraSettings);
            }

            var service = new BookService(repo);

            // Ensure we have an empty repo once the service has be inited
            repo.DeleteAll();

            for (int i = 0; i < numEntities; i++)
                repo.Add(GetGenericBook(i.ToString()));

            // Make sure our setup succeeded
            Assert.Equal(numEntities, repo.Count());
            return service;
        }

        public static Book GetGenericBook(String suffix)
        {
            var random = new Random();
            return new Book
            {
                Id = Guid.NewGuid(),
                Author = $"Author {suffix}",
                BookName = $"Book {suffix}",
                Category = $"Category {random.Next(0, 2)}",
                Price = ((decimal)Math.Round(random.NextDouble() * 100, 2)) // Random value between 0-100
            };
        }

        public static BookController GetBookController(int numEntities = 0)
            => new BookController(GetBookService(numEntities));
    }
}
