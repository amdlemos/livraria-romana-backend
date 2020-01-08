using Xunit;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Infrastructure.DBConfiguration;

namespace LivrariaRomana.Test.Repositorios
{
    public class BookRepositoryTest
    {
        private readonly BookRepository _bookRepository;
        private readonly DataBaseContext _dbContext;

        public BookRepositoryTest()
        {
            _dbContext = new DataBaseContext();
            _bookRepository = new BookRepository(_dbContext);
        }        

        [Fact]
        public void LoadBook()
        {
            var result = _bookRepository.GetAllIncludingBooksAsync();

            Assert.NotNull(result);
        }
    }
}
