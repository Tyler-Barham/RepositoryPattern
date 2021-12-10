using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using BooksApi.Controllers;
using BooksApi.Models;
using BooksApi.Repositories;
using BooksApi.Services;

namespace BooksApi.UnitTests.Utilities
{
    public class Initializers
    {
        public static ServiceProvider GetServiceProvider()
            => (ServiceProvider)Program.BuildWebHost(new string[] { }).Services;

        public static BookService GetBookService(int numEntities = 0)
        {
            var repo = new RAMRepository<Book>();
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
                Price = (decimal)random.NextDouble() * 100 // Random value between 0-100
            };
        }

        public static BookController GetBookController(int numEntities = 0)
            => new BookController(GetBookService(numEntities));
    }
}
