using BooksApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BooksApi.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.MongoConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            var collectionNames = database.ListCollectionNames().ToList();
            if (!collectionNames.Contains(settings.BooksCollectionName))
                database.CreateCollection(settings.BooksCollectionName);

            _books = database.GetCollection<Book>(settings.BooksCollectionName);

            Seed();
        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book.Id == bookIn.Id);

        public void Remove(string id) => 
            _books.DeleteOne(book => book.Id == id);

        private void Seed()
        {
            // No books
            if (!Get().Any())
            {
                var books = new List<Book>()
                {
                    new Book
                    {
                        BookName = "Design Patterns",
                        Price = 54.93M,
                        Category = "Computers",
                        Author = "Ralph Johnson"
                    },
                    new Book
                    {
                        BookName = "Clean Code",
                        Price = 43.15M,
                        Category = "Computers",
                        Author = "Robert C. Martin"
                    },
                };

                foreach (var book in books)
                    Create(book);
            }
        }
    }
}
