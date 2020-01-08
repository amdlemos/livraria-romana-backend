using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard
{
    public interface IDomainRepository<TEntity> : IRepositoryAsync<TEntity> where TEntity: class, IIdentityEntity
    {
    }
}
