using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using System.Threading.Tasks;

namespace LivrariaRomana.IServices
{
    public interface IUserService : IServiceBase<User>
    {
        Task<User> Authenticate(string username, string password);
        string GenerateToken(User user);
        Task<bool> CheckUserExistByUsername(string username);
        Task<bool> CheckUserExistByEmail(string email);
    }
}
