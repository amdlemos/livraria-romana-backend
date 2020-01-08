using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain
{
    public interface IUserRepository : IDomainRepository<User>
    {
        Task<IEnumerable<User>> GetAllIncludingUserAsync();
        Task<User> GetByIdIncludingUserAsync(int id);
    }
}
