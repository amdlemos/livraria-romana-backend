using LivrariaRomana.Domain.Validators;

namespace LivrariaRomana.Domain.Entities
{
    public class User : Entity, IEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }       
        public string Role { get; set; }       

        public User()
        {
            Validate(this, new UserValidator());
        }
        public User(string username, string password, string email, string role = "", int id = 0)
        {
            Username = username;
            Password = password;
            Email = email;
            Id = id;
            Role = role;

            Validate(this, new UserValidator());
        }
    }
}
