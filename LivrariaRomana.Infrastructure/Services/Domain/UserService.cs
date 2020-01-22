using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Infrastructure.Services.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using LivrariaRomana.Domain.DTO;

namespace LivrariaRomana.Infrastructure.Services.Domain
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _userRepository = repository;
        }

        public virtual async Task<UserDTO> Authenticate(string username, string password)
        {
            var user = new User();
            
            if (FirstAccess())
            {
                user = new User(username, password, "adicionar@email.com", "admin");
                await _repository.AddAsync(user);
            }
            
            user = await _userRepository.GetByUsernamePassword(username, password);

            if (user == null)
                return null;

            var userDTO = new UserDTO();
            userDTO.username = user.Username;
            userDTO.token = GenerateToken(user);
            userDTO.role = user.Role;
            //user.AddToken(GenerateToken(user));
            //user.Password = "";

            return userDTO;
        }

        private bool FirstAccess()
        {
            return _repository.GetAllAsync().Result.Count() == 0;
        }

        private static string GenerateToken(User user)
        {
            // Gera token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("bookStore", user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
