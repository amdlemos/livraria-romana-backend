using LivrariaRomana.Domain.Validators;

namespace LivrariaRomana.Domain.Entities
{
    public class User : Entity, IEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }       
        public string Role { get; set; }       

        public User()
        {
            this.Validate();
        }
        public User(string username, string password, string email, string role = "", int id = 0)
        {
            Username = username;
            Password = password;
            Email = email;
            Id = id;
            Role = role;

            this.Validate();
        }

        public void Validate()
        {
            Validate(this, new UserValidator());
        }
    }
}
