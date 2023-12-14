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
    public class AuthorsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<ApiResult<AuthorDTO>>> GetAuthors(
            int pageIndex = 0,
    int pageSize = 10, 
    string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
        {
            return await ApiResult<AuthorDTO>.CreateAsync(
            _context.Authors.AsNoTracking()
            .Select(c => new AuthorDTO()
            {
                Id = c.Id,
                Name = c.Name,
                COUNTRYOFORIGIN = c.COUNTRYOFORIGIN,
                Gender = c.Gender,
                TotBooks = c.Books!.Count
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
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]

        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
        [HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(
    int authorId,
    string fieldName,
    string fieldValue)
        {
            switch (fieldName)
            {
                case "name":
                    return _context.Authors.Any(
                        c => c.Name == fieldValue && c.Id != authorId);
                case "countryoforigin":
                    return _context.Authors.Any(
                        c => c.COUNTRYOFORIGIN == fieldValue && c.Id != authorId);
                case "gender":
                    return _context.Authors.Any(
                        c => c.Gender == fieldValue && c.Id != authorId);
                default:
                    return false;
            }
        }
    }
}