using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books_Management_API.DTOs
{
    public class CreateBookDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
    }

    public class ReadBookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public int BookViews { get; set; }
        public double PopularityScore { get; set; }
    }

    public class DeleteBookDTO
    {
        public Guid Id { get; set; }
    }
}