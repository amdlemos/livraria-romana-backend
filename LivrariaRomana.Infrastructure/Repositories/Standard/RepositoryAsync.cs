using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.EFCore;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Repositories.Standard
{
    public class RepositoryAsync<TEntity> : SpecifcMethods<TEntity>, IRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {
        public Task<TEntity> AddAsync(TEntity obj)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddRangAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveAsync(TEntity obj)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(TEntity obj)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<TEntity> GenerateQuery(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params string[] includeProperties)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TEntity> GetYieldManipulated(IEnumerable<TEntity> entities, Func<TEntity, TEntity> DoAction)
        {
            throw new NotImplementedException();
        }
    }
}
