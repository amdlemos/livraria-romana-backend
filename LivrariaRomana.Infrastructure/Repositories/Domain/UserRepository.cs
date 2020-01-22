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

        public virtual async Task<User> GetByUsernamePassword(string username, string password)
        {            
            var allUsers = await this.GetAllAsync();
            var user = allUsers.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            return user;
        }
    }
}
