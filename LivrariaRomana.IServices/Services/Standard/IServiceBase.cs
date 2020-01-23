using System.Collections.Generic;
using System.Threading.Tasks;

namespace LivrariaRomana.IServices
{
    public interface IServiceBase<TEntity> where TEntity : class
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
