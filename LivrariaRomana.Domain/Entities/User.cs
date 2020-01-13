using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace LivrariaRomana.Domain.Entities
{
    public class User : Entity, IEntity
    { 
       
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; private set; }
        public void AddToken(string token)
        {
            Token = token;
        }

        public User()
        {
            Validate(this, new UserValidator());
        }
        public User(string username, string password, string email, int id = 0)
        {
            Username = username;
            Password = password;
            Email = email;
            Id = id;

            Validate(this, new UserValidator());
        }        

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
}
