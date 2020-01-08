using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain
{
    public interface IBookRepository : IDomainRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllIncludingBooksAsync();
        Task<Book> GetByIdIncludingBooksAsync(int id);
    }
}
