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

        private double CalculatePopularityScore(Book book)
        {
            int currentYear = DateTime.UtcNow.Year;
            int yearsSincePublished = currentYear - book.PublicationYear;
            return (book.BookViews * 0.5) + (yearsSincePublished * 2);
        }

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
                PublicationYear = book.PublicationYear,
                BookViews = book.BookViews,
                PopularityScore=CalculatePopularityScore(book)
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

            book.BookViews += 1;
            await _context.SaveChangesAsync();

            var readBookDTO = new ReadBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear,
                BookViews = book.BookViews,
                PopularityScore = CalculatePopularityScore(book)
            };

            return Ok(readBookDTO);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ReadBookDTO>>> PostBooks(List<CreateBookDTO> createBookDTOs)
        {
            if (createBookDTOs == null || createBookDTOs.Count == 0)
            {
                return BadRequest("Book list cannot be empty.");
            }

            var duplicateTitles = createBookDTOs
            .GroupBy(dto => dto.Title)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

            if (duplicateTitles.Any())
            {
                return Conflict(new { message = $"Duplicate titles found in the provided list: {string.Join(", ", duplicateTitles)}" });
            }

            foreach (var createBookDTO in createBookDTOs)
            {
                bool bookExists = await _context.Books.AnyAsync(b => b.Title == createBookDTO.Title);
                if (bookExists)
                {
                    return Conflict(new { message = $"A book with the title '{createBookDTO.Title}' already exists." });
                }
            }

            var books = createBookDTOs.Select(dto => new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                PublicationYear = dto.PublicationYear
            }).ToList();

            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            var readBookDTOs = books.Select(book => new ReadBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationYear = book.PublicationYear,
                PopularityScore = CalculatePopularityScore(book)
            }).ToList();

            return CreatedAtAction(nameof(GetBooks), readBookDTOs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(Guid id, CreateBookDTO createBookDTO)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            bool bookExists = await _context.Books.AnyAsync(b => b.Title == createBookDTO.Title && b.Id != id);
            if (bookExists)
            {
                return Conflict(new { message = $"A book with the title '{createBookDTO.Title}' already exists." });
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