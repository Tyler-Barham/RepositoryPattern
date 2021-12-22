﻿using BooksApi.Models;
using BooksApi.Repository;
using MongoDB.Driver;
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

            _Seed();
        }

        public List<Book> Get() =>
            _repo.Get(book => true);

        public Book Get(string id) =>
            _repo.GetById(id);

        public Book Create(Book book) =>
            _repo.Add(book);

        public Book Update(string id, Book bookIn) =>
            _repo.Update(id, bookIn);

        public bool Remove(Book bookIn) =>
            _repo.Delete(bookIn);

        public bool Remove(string id) =>
            _repo.Delete(id);

        private void _Seed()
        {
            // No books
            if (_repo.Count() == 0)
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
                    _repo.Add(book);
            }
        }
    }
}
