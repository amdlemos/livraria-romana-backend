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
        public UserRepository(DataBaseContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<User>> GetAllIncludingUserAsync()
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: null,
                                                                     orderBy: null,
                                                                     includeProperties: nameof(User.Username)));
            return query.AsEnumerable();
        }

        public async Task<User> GetByIdIncludingUserAsync(int id)
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: (user => user.Id == id),
                                                                     orderBy: null,
                                                                     includeProperties: nameof(User.Username)));
            return query.SingleOrDefault();
        }
    }
}
