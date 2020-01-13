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

        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;            
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
