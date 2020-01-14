using FluentValidation;
using System;

namespace LivrariaRomana.Domain.Entities
{
    public class Book : Entity, IEntity
    {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string PublishingCompany { get; set; }
        public string ISBN { get; set; }
        public DateTime PublicationYear { get; set; }
        public int Amount { get; set; }

        public Book()
        {
            Validate(this, new BookValidator());
        }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;

            Validate(this, new BookValidator());
        }

        public Book(string title, string author, string originalTitle,  string publisingCompany, string isbn, DateTime publicationYear, int amount, int id = 0)
        {
            Id = id;
            Title = title;
            OriginalTitle = originalTitle;
            Author = author;
            PublishingCompany = publisingCompany;
            PublicationYear = publicationYear;
            ISBN = isbn;
            Amount = amount;

            Validate(this, new BookValidator());
        }

        public class BookValidator : AbstractValidator<Book>
        {
            public BookValidator()
            {
                RuleFor(a => a.Title).NotNull().NotEmpty().WithMessage("Título é obrigatório.");
                RuleFor(a => a.Author).NotNull().NotEmpty().WithMessage("Autor é obrigatório.");
                RuleFor(a => a.Amount).NotNull().GreaterThanOrEqualTo(0).WithMessage("A quantidade de livros deve ser maior ou igual a 0.");
                RuleFor(a => a.ISBN).Matches(@"(\d{10,13}).*?_(\d{3})|(\d{3}).*?_(\d{10,13})|(\d{10,13})(?=[^\d])");                    
            }
        }
    }
}
