using System;
using Xunit;
using BooksApi.UnitTests.Utilities;

namespace BooksApi.UnitTests.Services
{
    public class BookServiceFacts
    {
        public class GetMethod
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            public void ReturnAllEntities(int numEntities)
            {
                // Arrange
                var service = Initializers.GetBookService(numEntities);

                // Act
                var results = service.Get();

                // Assert
                Assert.Equal(numEntities, results.Count);
            }

            [Fact]
            public void ReturnSpecificEntity()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var expectedBook = Initializers.GetGenericBook("A");
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
                var service = Initializers.GetBookService(numEntities);

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
                var service = Initializers.GetBookService();
                var expectedBook = Initializers.GetGenericBook("A");

                // Act
                var result = service.Create(expectedBook);

                // Assert
                Assert.Equal(expectedBook, result);
            }

            [Fact]
            public void CreateDuplicateEntityFails()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var existingBook = service.Create(Initializers.GetGenericBook("A"));

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
                var service = Initializers.GetBookService();
                var initialBook = service.Create(Initializers.GetGenericBook("A"));
                var expectedId = initialBook.Id;
                var updatedBook = Initializers.GetGenericBook("B");
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
                var service = Initializers.GetBookService();
                var newBook = Initializers.GetGenericBook("A");

                // Act
                var result = service.Update(newBook.Id.ToString(), newBook);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void UpdateExistingEntityWithIncorrectIdParam()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var initialBook = service.Create(Initializers.GetGenericBook("A"));
                var expectedId = initialBook.Id;
                var updatedBook = Initializers.GetGenericBook("B");
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
                var service = Initializers.GetBookService();
                var initialBook = service.Create(Initializers.GetGenericBook("A"));
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
                var service = Initializers.GetBookService();
                var existingBook = service.Create(Initializers.GetGenericBook("A"));

                // Act
                var success = service.Remove(existingBook.Id.ToString());

                // Assert
                Assert.True(success);
                Assert.Null(service.Get(existingBook.Id.ToString()));
            }

            [Fact]
            public void RemoveNonExistingEntityByIdFails()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var nonexistingBook = Initializers.GetGenericBook("A");

                // Act
                var success = service.Remove(nonexistingBook.Id.ToString());

                // Assert
                Assert.False(success);
            }

            [Fact]
            public void RemoveExistingEntityByEntity()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var existingBook = service.Create(Initializers.GetGenericBook("A"));

                // Act
                var success = service.Remove(existingBook);

                // Assert
                Assert.True(success);
                Assert.Null(service.Get(existingBook.Id.ToString()));
            }

            [Fact]
            public void RemoveNonExistingEntityByEntityFails()
            {
                // Arrange
                var service = Initializers.GetBookService();
                var nonexistingBook = Initializers.GetGenericBook("A");

                // Act
                var success = service.Remove(nonexistingBook);

                // Assert
                Assert.False(success);
            }
        }
    }
}
