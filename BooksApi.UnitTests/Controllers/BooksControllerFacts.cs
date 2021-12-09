using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BooksApi.UnitTests.Utilities;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BooksApi.UnitTests.Controllers
{
    public class BooksControllerFacts
    {
        public class HttpGet
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(2)]
            public void GetAllEntities_Succeeds(int numEntities)
            {
                // Arrange
                var controller = Initializers.GetBookController(numEntities);

                // Act
                var results = controller.Get();

                // Assert
                Assert.Equal(numEntities, results.Value.Count);
            }

            [Fact]
            public void GetSpecificEntity_Succeeds()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var expectedBook = Initializers.GetGenericBook("A");
                controller.Create(expectedBook);

                // Act
                var result = controller.Get(expectedBook.Id.ToString());

                // Assert
                Assert.Equal(expectedBook, result.Value);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            public void GetEmptyGuid_ReturnsNotFound(int numEntities)
            {
                // Arrange
                var controller = Initializers.GetBookController(numEntities);

                // Act
                var result = controller.Get(Guid.Empty.ToString());

                // Assert
                Assert.Null(result.Value);
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        public class HttpPost
        {
            [Fact]
            public void CreateNewEntity_Succeeds()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var expectedBook = Initializers.GetGenericBook("A");

                // Act
                var rawResult = controller.Create(expectedBook);

                // Assert
                Assert.IsType<CreatedAtRouteResult>(rawResult.Result);
                CreatedAtRouteResult castedResult = (CreatedAtRouteResult)rawResult.Result;
                Assert.Equal(expectedBook, castedResult.Value);
            }

            [Fact]
            public void CreateDuplicateEntity_ReturnsProblem()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var existingBook = Initializers.GetGenericBook("A");
                controller.Create(existingBook);

                // Act
                var result = controller.Create(existingBook);

                // Assert
                Assert.IsType<ObjectResult>(result.Result);
                Assert.Equal(500, ((ObjectResult)result.Result).StatusCode);
            }
        }

        public class HttpPut
        {
            [Fact]
            public void UpdateExistingEntity_Succeeds()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var initialBook = Initializers.GetGenericBook("A");
                controller.Create(initialBook);
                var expectedId = initialBook.Id;
                var updatedBook = Initializers.GetGenericBook("B");
                updatedBook.Id = expectedId;

                // Act
                var result = controller.Update(expectedId.ToString(), updatedBook);

                // Assert
                Assert.IsType<OkResult>(result);
                var returnedBook = controller.Get(expectedId.ToString()).Value;
                Assert.Equal(updatedBook, returnedBook);
            }

            [Fact]
            public void UpdateNonExistingEntityFails_ReturnsNotFound()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var newBook = Initializers.GetGenericBook("A");

                // Act
                var result = controller.Update(newBook.Id.ToString(), newBook);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }

            [Fact]
            public void UpdateExistingEntityWithIncorrectIdParam_ReturnsBadRequest()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var initialBook = Initializers.GetGenericBook("A");
                controller.Create(initialBook);
                var expectedId = initialBook.Id;
                var updatedBook = Initializers.GetGenericBook("B");
                updatedBook.Id = expectedId;

                // Act
                var result = controller.Update(Guid.Empty.ToString(), updatedBook);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }

            [Fact]
            public void UpdateExistingEntityWithIncorrectEntityId_ReturnsBadRequest()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var initialBook = Initializers.GetGenericBook("A");
                controller.Create(initialBook);
                var expectedId = initialBook.Id;
                initialBook.Id = Guid.NewGuid();

                // Act
                var result = controller.Update(expectedId.ToString(), initialBook);

                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        public class HttpDelete
        {
            [Fact]
            public void RemoveExistingEntity_SucceedsThenNotFound()
            {
                // Arrange
                var controller = Initializers.GetBookController();
                var existingBook = Initializers.GetGenericBook("A");
                controller.Create(existingBook);

                // Act
                var result = controller.Delete(existingBook.Id.ToString());

                // Assert
                Assert.IsType<OkResult>(result);
                var newResult = controller.Get(existingBook.Id.ToString());
                Assert.IsType<NotFoundResult>(newResult.Result);
            }

            [Fact]
            public void RemoveNonExistingEntity_ReturnsNotFound()
            {
                // Arrange
                var controller = Initializers.GetBookController();

                // Act
                var result = controller.Delete(Guid.Empty.ToString());

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }
    }
}
