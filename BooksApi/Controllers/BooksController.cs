using BooksApi.Models;
using BooksApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public ActionResult<List<Book>> Get() =>
            _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Book> Get(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
                return NotFound();
            else
                return book;
        }

        [HttpPost]
        public ActionResult<Book> Create(Book bookIn)
        {
            var book = _bookService.Create(bookIn);

            if (book == null)
                return Problem();
            else
                return CreatedAtRoute("GetBook", new { id = book.Id.ToString() }, book);
        }

        [HttpPut("{id:length(36)}")]
        public IActionResult Update(string id, Book bookIn)
        {
            if (!id.Equals(bookIn.Id.ToString()))
                return BadRequest();

            var book = _bookService.Update(id, bookIn);

            if (book == null)
                return NotFound();
            else
                return Ok();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var deleted = _bookService.Remove(id);

            if (!deleted)
                return NotFound();
            else
                return Ok();
        }
    }
}
