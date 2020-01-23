using LivrariaRomana.Domain.Entities;
using System.Threading.Tasks;

namespace LivrariaRomana.IRepositories
{
    public interface IUserRepository : IDomainRepository<User>
    {
        Task<User> GetByUsernamePassword(string username, string password);
    }
}
