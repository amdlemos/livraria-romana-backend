using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain
{
    public interface IUserService : IDomainRepository<User>
    {
        Task<User> GetByUsernamePassword(string username, string password);
    }
}
