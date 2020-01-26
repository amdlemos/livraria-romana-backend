using LivrariaRomana.Domain.Validators;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivrariaRomana.Domain.Entities
{
    public class Book : Entity, IEntity
    {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string PublishingCompany { get; set; }
        public string ISBN { get; set; }
        public string PublicationYear { get; set; }
        public int Amount { get; set; }      

        public Book()
        {
            this.Validate();
        }

        public Book(string title, string author, string originalTitle, string publisingCompany, string isbn, string publicationYear = null, int id = 0)
        {
            Id = id;
            Title = title;
            OriginalTitle = originalTitle;
            Author = author;
            PublishingCompany = publisingCompany;
            PublicationYear = publicationYear;
            ISBN = isbn;
            Amount = 0;

            this.Validate();
        }

        public void Validate()
        {
            Validate(this, new BookValidator());
        }
    }
}
