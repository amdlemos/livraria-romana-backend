using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using LivrariaRomana.Infrastructure.Repositories.Standard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Repositories.Domain
{
    public class BookRepository : RepositoryAsync<Book>, IDomainRepository<Book>, IBookRepository
    {
        public BookRepository(DataBaseContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Book>> GetAllIncludingBooksAsync()
        {
            IQueryable<Book> query = await Task.FromResult(GenerateQuery(filter: null,
                                                                         orderBy: null,
                                                                         includeProperties: nameof(Book.Title)));
            return query.AsEnumerable();
        }

        public async Task<Book> GetByIdIncludingBooksAsync(int id)
        {
            IQueryable<Book> query = await Task.FromResult(GenerateQuery(filter: (book => book.Id == id),
                                                                         orderBy: null,
                                                                         includeProperties: nameof(Book.Title)));
            return query.SingleOrDefault();
        }
    }
}

