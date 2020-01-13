using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard
{
    public interface IRepositoryAsync<TEntity> : IDisposable where TEntity : class, IEntity
    {
        // ADD
        Task<TEntity> AddAsync(TEntity obj);       

        // GET
        Task<TEntity> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        // UPDATE
        Task<int> UpdateAsync(TEntity obj);        

        // REMOVE
        Task<bool> RemoveAsync(object id);
        Task<int> RemoveAsync(TEntity obj);
        

    }
}
