using LivrariaRomana.Domain.Entities;
using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LivrariaRomana.IServices;
using LivrariaRomana.IRepositories;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace LivrariaRomana.Services
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _userRepository = repository;
        }

        public virtual async Task<User> Authenticate(string username, string password)
        {
            User user;

            //var key = EncryptPassword.GetHashKey();
            //var passwordEncrypited = EncryptPassword.Encrypt(key, password);
            
            if (FirstAccess())
            {
                user = new User(username, password, "adicionar@email.com", "admin");
                await _repository.AddAsync(user);
            }

            user = await _userRepository.GetByUsernamePassword(username, password);
                       
            if (user == null)
                return null;

            user.Password = "";

            //var userDTO = new UserDTO();          
            //userDTO.token = GenerateToken(user);
          

            return user;
        }

        public async Task<bool> CheckUserExistByEmail(string email)
        {
            var users= await _userRepository.GetAllAsync();
            return users.Where(x => x.Email == email).Count() > 0;
        }

        public async Task<bool> CheckUserExistByUsername(string username)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(x => x.Username == username).Count() > 0;
        }

        public string GenerateToken(User user)
        {            
            // Cria chave
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
           
            // Cria token descriptor
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

            // Gera e retorna token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool FirstAccess()
        {
            return _repository.GetAllAsync().Result.Count() == 0;
        }      
    }
}
