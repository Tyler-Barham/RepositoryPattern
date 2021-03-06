using BooksApi.Models;
using BooksApi.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BooksApi.Services
{
    public class BookService
    {
        private readonly IRepository<Book> _repo;

        public BookService(IRepository<Book> repo)
        {
            _repo = repo;

            Seed();
        }

        public List<Book> Get() =>
            _repo.Get(book => true);

        public Book Get(string id) =>
            _repo.GetById(new Guid(id));

        public Book Create(Book book) =>
            _repo.Add(book);

        public Book Update(string id, Book bookIn) =>
            _repo.Update(new Guid(id), bookIn);

        public bool Remove(Book bookIn) =>
            _repo.Delete(bookIn);

        public bool Remove(string id) =>
            _repo.Delete(new Guid(id));

        private void Seed()
        {
            // No books
            if (_repo.Count() == 0)
            {
                var books = new List<Book>()
                {
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        BookName = "Design Patterns",
                        Price = 54.93M,
                        Category = "Computers",
                        Author = "Ralph Johnson"
                    },
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        BookName = "Clean Code",
                        Price = 43.15M,
                        Category = "Computers",
                        Author = "Robert C. Martin"
                    },
                };

                _repo.Add(books);
            }
        }
    }
}
