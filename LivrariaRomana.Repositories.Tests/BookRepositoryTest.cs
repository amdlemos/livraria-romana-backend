using FluentAssertions;
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
            _dbContext.Database.BeginTransaction();
        }

        [Fact]
        public async Task AddBookAsyncTest()
        {
            // Act
            var result = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Assert
            result.Id.Should().BePositive();            
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookRepository.GetByIdAsync(entity.Id);
            // Assert
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            // Arrange
            await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookRepository.GetAllAsync();
            // Assert
            result.Count().Should().BePositive();
        }

        [Fact]
        public async Task RemoveAsync()
        {
            // Arrange
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());            
            // Act
            var result = await _bookRepository.RemoveAsync(entity.Id);            
            // Asset
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAscyncTest()
        {
            // Arrange
            var book = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            var newTitle = $"Title Editado: + { DateTime.Now }";
            book.Title = newTitle;

            // Acct
            var result = await _bookRepository.UpdateAsync(book);

            // Assert
            Assert.Equal(1, result);
            var bookEdited = await _bookRepository.GetByIdAsync(book.Id);
            Assert.Equal(newTitle, bookEdited.Title);
            
        }

        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
