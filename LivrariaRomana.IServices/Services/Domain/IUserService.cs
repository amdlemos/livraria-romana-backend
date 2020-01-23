using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using System.Threading.Tasks;

namespace LivrariaRomana.IServices
{
    public interface IUserService : IServiceBase<User>
    {
        Task<UserDTO> Authenticate(string username, string password);
    }
}
