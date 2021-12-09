using System;
using Xunit;
using BooksApi.Models;
using BooksApi.Services;
using BooksApi.Repository;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]

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

            for (int i = 0; i < numEntities; i++)
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
                Price = (decimal)random.NextDouble() * 100 // Random value between 0-100
            };
        }

        public class GetMethod
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            public void ReturnAllEntities(int numEntities)
            {
                // Arrange
                var service = SetupRepo(numEntities);

                // Act
                var results = service.Get();

                // Assert
                Assert.Equal(numEntities, results.Count);
            }

            [Fact]
            public void ReturnSpecificEntity()
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
            public void ReturnNullForEmptyGuid(int numEntities)
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
            [Fact]
            public void CreateNewEntity()
            {
                // Arrange
                var service = SetupRepo(0);
                var expectedBook = GetGenericBook("A");

                // Act
                var result = service.Create(expectedBook);

                // Assert
                Assert.Equal(expectedBook, result);
            }

            [Fact]
            public void CreateDuplicateEntityFails()
            {
                // Arrange
                var service = SetupRepo(0);
                var existingBook = service.Create(GetGenericBook("A"));

                // Act
                var result = service.Create(existingBook);

                // Assert
                Assert.Null(result);
            }
        }

        public class UpdateMethod
        {
            [Fact]
            public void UpdateExistingEntity()
            {
                // Arrange
                var service = SetupRepo(0);
                var initialBook = service.Create(GetGenericBook("A"));
                var expectedId = initialBook.Id;
                var updatedBook = GetGenericBook("B");
                updatedBook.Id = expectedId;

                // Act
                var result = service.Update(expectedId.ToString(), updatedBook);

                // Assert
                Assert.Equal(updatedBook, result);
            }

            [Fact]
            public void UpdateNonExistingEntityFails()
            {
                // Arrange
                var service = SetupRepo(0);
                var newBook = GetGenericBook("A");

                // Act
                var result = service.Update(newBook.Id.ToString(), newBook);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void UpdateExistingEntityWithIncorrectIdParam()
            {
                // Arrange
                var service = SetupRepo(0);
                var initialBook = service.Create(GetGenericBook("A"));
                var expectedId = initialBook.Id;
                var updatedBook = GetGenericBook("B");
                updatedBook.Id = expectedId;

                // Act
                var result = service.Update(Guid.Empty.ToString(), updatedBook);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void UpdateExistingEntityWithIncorrectEntityId()
            {
                // Arrange
                var service = SetupRepo(0);
                var initialBook = service.Create(GetGenericBook("A"));
                var expectedId = initialBook.Id;
                initialBook.Id = Guid.NewGuid();

                // Act
                var result = service.Update(expectedId.ToString(), initialBook);

                // Assert
                Assert.Null(result);
            }
        }

        public class RemoveMethod
        {
            [Fact]
            public void RemoveExistingEntityById()
            {
                // Arrange
                var service = SetupRepo(0);
                var existingBook = service.Create(GetGenericBook("A"));

                // Act
                var success = service.Remove(existingBook.Id.ToString());

                // Assert
                Assert.True(success);
            }

            [Fact]
            public void RemoveNonExistingEntityByIdFails()
            {
                // Arrange
                var service = SetupRepo(0);
                var nonexistingBook = GetGenericBook("A");

                // Act
                var success = service.Remove(nonexistingBook.Id.ToString());

                // Assert
                Assert.False(success);
            }

            [Fact]
            public void RemoveExistingEntityByEntity()
            {
                // Arrange
                var service = SetupRepo(0);
                var existingBook = service.Create(GetGenericBook("A"));

                // Act
                var success = service.Remove(existingBook);

                // Assert
                Assert.True(success);
            }

            [Fact]
            public void RemoveNonExistingEntityByEntityFails()
            {
                // Arrange
                var service = SetupRepo(0);
                var nonexistingBook = GetGenericBook("A");

                // Act
                var success = service.Remove(nonexistingBook);

                // Assert
                Assert.False(success);
            }
        }
    }
}
