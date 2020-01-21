using LivrariaRomana.Domain.Validators;

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
    }
}
