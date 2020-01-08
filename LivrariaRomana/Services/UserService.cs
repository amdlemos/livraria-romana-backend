using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace LivrariaRomana.Services
{
    public class UserService
    {
        private readonly DataBaseContext _dbContext;

        public UserService(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool VerificaEmail(string email)
        {
            return _dbContext.Usuarios.Any(x => x.Email == email);
        }

        public User Authenticate(string username, string password)
        {
            var user = _dbContext.Usuarios.Where(x => x.Username == username && x.Password == password).FirstOrDefault();

            if (user == null)
                return null;
            
            // Gera token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    //new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";

            return user;
        }
    }
}
