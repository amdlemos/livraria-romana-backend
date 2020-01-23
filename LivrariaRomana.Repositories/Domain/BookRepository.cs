using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;

namespace LivrariaRomana.Repositories
{
    public class BookRepository : RepositoryAsync<Book>, IDomainRepository<Book>, IBookRepository
    {
        public BookRepository(DatabaseContext dbContext) : base(dbContext)
        {

        }       
    }
}

