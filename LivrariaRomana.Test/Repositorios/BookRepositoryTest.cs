using Xunit;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Infrastructure.DBConfiguration;
using System.Threading.Tasks;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Test.DataBuilder;
using System.Linq;
using System;

namespace LivrariaRomana.Test.Repositorios
{
    public class BookRepositoryTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;

        private readonly BookBuilder _bookBuilder;
        

        public BookRepositoryTest()
        {
            _dbContext = new DatabaseContext();
            _bookRepository = new BookRepository(_dbContext);
            _bookBuilder = new BookBuilder();
        }        

        [Fact]
        public void AddBookAsyncTest()
        {            
            var result = _bookRepository.AddAsync(_bookBuilder.CreateBook());

            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateBook());
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
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateBook());
            var result = await _bookRepository.RemoveAsync(entity.Id);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveAsyncObjTest()
        {
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateBook());
            var result = await _bookRepository.RemoveAsync(entity);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAscyncTest()
        {
            var allBooks = await _bookRepository.GetAllAsync();
            var entity = allBooks.FirstOrDefault();
            var newTitle = $"Title Editado: + { DateTime.Now }";
            entity.Title = newTitle;

            var result = await _bookRepository.UpdateAsync(entity);
            Assert.Equal(newTitle, entity.Title);
            Assert.Equal(1, result);
        }
    }
}
