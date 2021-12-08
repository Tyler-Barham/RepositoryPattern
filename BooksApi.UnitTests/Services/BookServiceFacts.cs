using System;
using Xunit;
using BooksApi.Models;
using BooksApi.Services;
using BooksApi.Repository;

namespace BooksApi.UnitTests
{
    public class BookServiceFacts
    {
        private static BookService SetupRepo(int numEntities = 1)
        {
            var repo = new RAMRepository<Book>();
            var service = new BookService(repo);

            // Ensure we have an empty repo once the service has be inited
            repo.DeleteAll();

            for(int i = 0; i < numEntities; i++)
                repo.Add(GetGenericBook(i.ToString()));

            // Make sure our setup succeeded
            Assert.Equal(numEntities, repo.Count());
            return service;
        }

        private static Book GetGenericBook(String suffix)
        {
            var random = new Random();
            return new Book
            {
                Id = Guid.NewGuid(),
                Author = $"Author {suffix}",
                BookName = $"Book {suffix}",
                Category = $"Category {random.Next(0, 2)}",
                Price = (decimal)random.NextDouble()*100 // Random value between 0-100
            };
        }
        public class GetMethod
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            public void ReturnAll(int numEntities)
            {
                // Arrange
                var service = SetupRepo(numEntities);

                // Act
                var results = service.Get();

                // Assert
                Assert.Equal(numEntities, results.Count);
            }

            [Fact]
            public void ReturnSpecific()
            {
                // Arrange
                var service = SetupRepo(0);
                var expectedBook = GetGenericBook("A");
                service.Create(expectedBook);

                // Act
                var result = service.Get(expectedBook.Id.ToString());

                // Assert
                Assert.Equal(expectedBook, result);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            public void ReturnNull(int numEntities)
            {
                // Arrange
                var service = SetupRepo(numEntities);

                // Act
                var result = service.Get(Guid.Empty.ToString());

                // Assert
                Assert.Null(result);
            }
        }

        public class CreateMethod
        {
            [Fact (Skip = "Not Implemented")]
            public void CreateNew()
            {
                // Arrange
                // Act
                // Assert
                throw new NotImplementedException();
            }
        }
    }
}
