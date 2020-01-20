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
            Validate(this, new BookValidator(null));
        }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;

            Validate(this, new BookValidator(null));
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

            Validate(this, new BookValidator(isbn));
        }

        public class BookValidator : AbstractValidator<Book>
        {
            public BookValidator(string isbn)
            {
                RuleFor(a => a.Title).NotNull().NotEmpty().WithMessage("Título é obrigatório.");
                RuleFor(a => a.Author).NotNull().NotEmpty().WithMessage("Autor é obrigatório.");
                RuleFor(a => a.Amount).NotNull().GreaterThanOrEqualTo(0).WithMessage("A quantidade de livros deve ser maior ou igual a 0.");  
                if(isbn != null)
                    RuleFor(a => a.ISBN).Matches(@"ISBN(-1(?:(0)|3))?:?\x20(\s)*[0-9]+[- ][0-9]+[- ][0-9]+[- ][0-9]*[- ]*[xX0-9]");                    
            }
        }
    }
}
