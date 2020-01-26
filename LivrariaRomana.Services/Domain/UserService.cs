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

        public override async Task<User> AddAsync(User obj)
        {
            // Verifica se o usuário já existe
            var existingEmail = await CheckUserExistByEmail(obj.Email);
            var existingUsername = await CheckUserExistByEmail(obj.Username);

            
            if (!existingEmail && !existingUsername)
            {
                // Criptografa o password
                //var key = EncryptPassword.GetHashKey();
                //var passwordEncrypited = EncryptPassword.Encrypt(key, obj.Password);
                //obj.Password = passwordEncrypited;

                // Cria salt e salt e salva no objeto
                var salt = EncryptPassword.CreateSaltArgon2();                
                obj.Hash = Convert.ToBase64String(EncryptPassword.HashPasswordArgon2(obj.Password, salt));
                obj.Salt = Convert.ToBase64String(salt);

                // Adicona o usuário 
                obj.Password = "p@ssword";
                var user = await base.AddAsync(obj);
                
                // Escondo hash e salt
                user.Hash = "";
                user.Salt = "";

                
                return user;
            }

            return null;
        }

        /// <summary>
        /// Autentica o usuário no sistema.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Retorna um usuário caso ele exista, do contrário retorna null.</returns>
        public virtual async Task<User> Authenticate(string username, string password)
        {            
            User user;

            // Verifica se já existe usuário no sistema
            if (FirstAccess())
            {
                // Cria usuário
                user = new User(username, password, "adicionar@email.com", "admin");
                await this.AddAsync(user);
            }
            else
            {
                // Busca usuário
                var allUsers = await _userRepository.GetAllAsync();
                user = allUsers.Where(x => x.Username == username).FirstOrDefault();

                // Valida
                if (user != null)
                {        
                    // gera hash
                    var salt = Convert.FromBase64String(user.Salt);
                    var hash = EncryptPassword.HashPasswordArgon2(password, salt);

                    // Valida hash                    
                    if (EncryptPassword.VerifyHash(password, salt, hash))
                        user = await _userRepository.GetByIdAsync(user.Id);
                    else
                        user = null;
                }                
            }
                       
            // Esconde o password
            if (user != null)
                user.Password = null;

            // Retorna o usuário
            return user;
        }

        /// <summary>
        /// Verifica se o email já está em uso.
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckUserExistByEmail(string email)
        {
            var users= await _userRepository.GetAllAsync();
            return users.Where(x => x.Email == email).Count() > 0;
        }

        /// <summary>
        /// Verifica se o username já está em uso
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckUserExistByUsername(string username)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(x => x.Username == username).Count() > 0;
        }
        
        /// <summary>
        /// Verifica se usuário existe.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckUserExistById(int id)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(x => x.Id == id).Count() > 0;
        }

        /// <summary>
        /// Gera token.
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>String contendo o token</returns>
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

        /// <summary>
        /// Verifica se é o primeiro acesso.
        /// </summary>
        /// <returns>Boolean</returns>
        private bool FirstAccess()
        {
            return _repository.GetAllAsync().Result.Count() == 0;
        }      
    }
}
