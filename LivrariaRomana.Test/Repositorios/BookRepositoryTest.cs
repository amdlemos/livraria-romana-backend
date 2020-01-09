using Xunit;
using System.Linq.Expressions;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Infrastructure.DBConfiguration;
using System.Collections.Generic;
using System.Threading.Tasks;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;

namespace LivrariaRomana.Test.Repositorios
{
    public class BookRepositoryTest
    {
        private readonly IBookRepository _bookRepository;
        private readonly DatabaseContext _dbContext;

        public BookRepositoryTest()
        {
            _dbContext = new DatabaseContext();
            _bookRepository = new BookRepository(_dbContext);
        }        

        [Fact]
        public void AddBookTest()
        {
            Book book = new Book()
            {
                Id = 0,
                Author = "Primeiro test",
                Title = "Primeiro titulo",
                ISBN = null,
                OriginalTitle = null,
                PublicationYear = null,
                PublishingCompany = null
            };
            var result = _bookRepository.AddAsync(book);            
            
            Assert.NotNull(result);
        }

        [Fact]
        public void GetByIdAsync()
        {
            var result = _bookRepository.GetByIdAsync(1);

            Assert.NotNull(result);
        }
    }
}
