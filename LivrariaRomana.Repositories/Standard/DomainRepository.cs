using LivrariaRomana.Domain.Entities;
using LivrariaRomana.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LivrariaRomana.Repositories
{
    public class DomainRepository<TEntity> : RepositoryAsync<TEntity>,
                                         IDomainRepository<TEntity> where TEntity : class, IEntity
    {
        protected DomainRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
