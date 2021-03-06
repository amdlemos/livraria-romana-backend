﻿using LivrariaRomana.Domain.Entities;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LivrariaRomana.Services
{
    public class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : class, IEntity
    {
        protected readonly IRepositoryAsync<TEntity> _repository;

        public ServiceBase(IRepositoryAsync<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            return await _repository.AddAsync(obj);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<bool> RemoveAsync(object id)
        {
            return await _repository.RemoveAsync(id);
        }

        public virtual async Task<int> RemoveAsync(TEntity obj)
        {
            return await _repository.RemoveAsync(obj);
        }

        public virtual async Task<int> UpdateAsync(TEntity obj)
        {
            return await _repository.UpdateAsync(obj);
        }
    }
}
