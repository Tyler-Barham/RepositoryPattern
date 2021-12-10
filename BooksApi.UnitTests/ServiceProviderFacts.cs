using System;
//using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using BooksApi.UnitTests.Utilities;
using BooksApi.Controllers;
using BooksApi.Models;
using BooksApi.Repositories;
using BooksApi.Services;

namespace BooksApi.UnitTests
{
    public class ServiceProviderFacts
    {
        [Theory]
        [InlineData(typeof(BookService))]
        [InlineData(typeof(IRepository<Book>))] // Enforces that there is a repo implementing this interface
        [InlineData(typeof(IOpenApiDocumentGenerator))] // Enforces swagger documentation
        public void HasRequiredServices(Type type)
        {
            // Arrange
            var serviceProvider = Initializers.GetServiceProvider();

            // Act
            var expectedService = serviceProvider.GetRequiredService(type);
                
            // Assert
            Assert.NotNull(expectedService);
            Assert.IsAssignableFrom(type, expectedService); // Cannot use IsType() due to checking for
                                                            // interfaces rather than concrete types
        }
    }
}
