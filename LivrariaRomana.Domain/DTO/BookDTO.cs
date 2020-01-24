using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.DTO
{
    public class BookDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string originalTitle { get; set; }
        public string author { get; set; }
        public string publishingCompany { get; set; }
        public string isbn { get; set; }
        public string publicationYear { get; set; }
        public int amount { get; set; }
    }
}
