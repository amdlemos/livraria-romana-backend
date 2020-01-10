﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Services.Standard
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