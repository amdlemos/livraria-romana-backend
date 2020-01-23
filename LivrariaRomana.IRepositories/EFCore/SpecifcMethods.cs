using LivrariaRomana.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LivrariaRomana.IRepositories
{
    public abstract class SpecifcMethods<TEntity> where TEntity : class, IEntity
    {
        #region ProtectedMethods
        protected abstract IQueryable<TEntity> GenerateQuery(Expression<Func<TEntity, bool>> filter = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                params string[] includeProperties);

        protected abstract IQueryable<TEntity> GenerateQueryableWhereExpression(IQueryable<TEntity> query,
                                                Expression<Func<TEntity, bool>> filter);

        protected abstract IQueryable<TEntity> GenerateIncludeProperties(IQueryable<TEntity> query, params string[] includeProperties);

        protected abstract IEnumerable<TEntity> GetYieldManipulated(IEnumerable<TEntity> entities, Func<TEntity, TEntity> DoAction);
        #endregion ProtectedMethods
    }
}
