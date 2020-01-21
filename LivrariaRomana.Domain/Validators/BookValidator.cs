using FluentValidation;
using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator(string isbn)
        {
            RuleFor(a => a.Title).NotNull().NotEmpty().WithMessage("Título é obrigatório.");
            RuleFor(a => a.Author).NotNull().NotEmpty().WithMessage("Autor é obrigatório.");
            RuleFor(a => a.Amount).NotNull().GreaterThanOrEqualTo(0).WithMessage("A quantidade de livros deve ser maior ou igual a 0.");
            if (!string.IsNullOrEmpty(isbn))
                RuleFor(a => a.ISBN).Matches(@"ISBN(-1(?:(0)|3))?:?\x20(\s)*[0-9]+[- ][0-9]+[- ][0-9]+[- ][0-9]*[- ]*[xX0-9]");
        }
    }
}
