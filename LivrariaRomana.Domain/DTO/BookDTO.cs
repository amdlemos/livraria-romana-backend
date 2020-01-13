using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.DTO
{
    public class BookDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string publishingCompany { get; set; }
        public string publicationYear { get; set; }
        public string isbn { get; set; }
        public int amount { get; set; }
    }
}
