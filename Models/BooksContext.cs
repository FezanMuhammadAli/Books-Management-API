using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Books_Management_API.Models
{
    public class BooksContext:DbContext
    {
        public BooksContext(DbContextOptions<BooksContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}