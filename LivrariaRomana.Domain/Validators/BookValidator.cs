using FluentValidation;
using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(a => a.Title).NotNull().NotEmpty().WithMessage("Título é obrigatório.");
            RuleFor(a => a.Author).NotNull().NotEmpty().WithMessage("Autor é obrigatório.");
            RuleFor(a => a.Amount).NotNull().GreaterThanOrEqualTo(0).WithMessage("A quantidade de livros deve ser maior ou igual a 0.");
            RuleFor(a => a.PublicationYear)
                .Matches(@"^\d{4}$").WithMessage("Data de publicação inválida")
                .When(m => !string.IsNullOrEmpty(m.PublicationYear));

            RuleFor(a => a.ISBN)
                .Matches(@"ISBN(-1(?:(0)|3))?:?\x20(\s)*[0-9]+[- ][0-9]+[- ][0-9]+[- ][0-9]*[- ]*[xX0-9]")
                .WithMessage("ISBN inválido.")
                .When(a => !string.IsNullOrEmpty(a.ISBN));
        }
    }
}
