using FluentValidation;
using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(a => a.Username).NotEmpty().WithMessage("Username é obrigatório.");
            RuleFor(a => a.Password).NotEmpty().WithMessage("Password é obrigatório.");
            RuleFor(a => a.Email).NotNull().NotEmpty().WithMessage("Email é obrigatório.");
            RuleFor(a => a.Email).EmailAddress().WithMessage("Email inválido.");
        }
    }
}
