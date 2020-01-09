using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using LivrariaRomana.Infrastructure.Repositories.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Repositories.Domain
{
    public class BookRepository : RepositoryAsync<Book>, IDomainRepository<Book>, IBookRepository
    {
        public BookRepository(DatabaseContext dbContext) : base(dbContext)
        {

        }       
    }
}

