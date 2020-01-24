using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.TestingAssistent.DataBuilder;
using LivrariaRomana.TestingAssistent.DBConfiguration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.Repositories.Tests
{
    public class BookRepositoryTest : IDisposable
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;

        private readonly BookBuilder _bookBuilder;


        public BookRepositoryTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();
            _bookRepository = new BookRepository(_dbContext);
            _bookBuilder = new BookBuilder();
            _dbContext.Database.BeginTransactionAsync();
        }

        [Fact]
        public async Task AddBookAsyncTest()
        {
            var result = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());

            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            var result = await _bookRepository.GetByIdAsync(entity.Id);

            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            var result = await _bookRepository.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveAsync()
        {
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            var result = await _bookRepository.RemoveAsync(entity.Id);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveAsyncObjTest()
        {
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            var result = await _bookRepository.RemoveAsync(entity);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAscyncTest()
        {
            var book = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());


            var newTitle = $"Title Editado: + { DateTime.Now }";
            book.Title = newTitle;

            var result = await _bookRepository.UpdateAsync(book);

            var bookEdited = await _bookRepository.GetByIdAsync(book.Id);

            Assert.Equal(newTitle, bookEdited.Title);
            Assert.Equal(1, result);
        }

        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
