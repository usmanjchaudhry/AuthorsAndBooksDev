using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


using AuthorsAndBooksAPI.Data;
using AuthorsAndBooksAPI.Data.Models;

namespace AuthorsAndBooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<ApiResult<BookDTO>>> GetBooks(int pageIndex = 0, 
    int pageSize = 10, string? sortColumn = null,
        string? sortOrder = null, string? filterColumn = null,
        string? filterQuery = null)
        {
            return await ApiResult<BookDTO>.CreateAsync(
            _context.Books.AsNoTracking()
             .Select(c => new BookDTO()
             {
                 Id = c.Id,
                 Title = c.Title,
                 Genre = c.Genre,
                 MAINCHARACTER = c.MAINCHARACTER,
                 AuthorId = c.Author!.Id,
                 AuthorName = c.Author!.Name
             }),
            pageIndex,
            pageSize,
            sortColumn,
            sortOrder,
              filterColumn,
                    filterQuery);

        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]

        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RegisteredUser")]

        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        [HttpPost]
        [Route("IsDupeCity")]
        public bool IsDupeCity(Book book)
        {
            return _context.Books.Any(
                e => e.Title == book.Title
                && e.Genre == book.Genre
                && e.MAINCHARACTER == book.MAINCHARACTER
                && e.AuthorId == book.AuthorId
                && e.Id  != book.Id
            );
        }
    }
}