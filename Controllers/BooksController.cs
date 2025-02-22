using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books_Management_API.DTOs;
using Books_Management_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books_Management_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BooksContext _context;

        public BooksController(BooksContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadBookDTO>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            var readBookDTOs = books.Select(book => new ReadBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear
            }).ToList();

            return Ok(readBookDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadBookDTO>> GetBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var readBookDTO = new ReadBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear
            };

            return Ok(readBookDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ReadBookDTO>> PostBook(CreateBookDTO createBookDTO)
        {
            var book = new Book
            {
                Title = createBookDTO.Title,
                Author = createBookDTO.Author,
                PublicationYear = createBookDTO.PublicationYear
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var readBookDTO = new ReadBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear
            };

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, readBookDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(Guid id, CreateBookDTO createBookDTO)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = createBookDTO.Title;
            book.Author = createBookDTO.Author;
            book.PublicationYear = createBookDTO.PublicationYear;

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
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

        private bool BookExists(Guid id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}