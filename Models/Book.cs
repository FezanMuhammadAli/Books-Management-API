using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books_Management_API.Models
{
    public class Book
    {
        public Guid Id { get; set; }=Guid.NewGuid();
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public int BookViews { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
    }
}