using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Repositories.Standard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Repositories.Domain
{
    public class UserRepository : DomainRepository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext dbContext) : base(dbContext)
        {

        }
    }
}
