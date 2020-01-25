using System;
using System.Linq;
using System.Threading.Tasks;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.TestingAssistent.DataBuilder;
using LivrariaRomana.TestingAssistent.DBConfiguration;
using Xunit;
using FluentAssertions;

namespace LivrariaRomana.Services.Tests
{
    public class BookServiceTest : IDisposable
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly BookBuilder _bookBuilder;

        public BookServiceTest()
        {
            // Prepare Test
            _dbContext = new Connection().DatabaseConfiguration();
            _bookRepository = new BookRepository(_dbContext);
            _bookService = new BookService(_bookRepository);
            _bookBuilder = new BookBuilder();
            
            // Start Transaction
            _dbContext.Database.BeginTransaction();
        }

        [Fact]
        public async Task AddBookAsyncTest()
        {
            // Act
            var result = await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            // Assert
            result.Id.Should().BePositive();
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.GetByIdAsync(entity.Id);
            // Assert
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            // Arrange
            await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.GetAllAsync();
            // Assert
            result.Count().Should().BePositive();
        }

        [Fact]
        public async Task RemoveAsync()
        {
            // Arrange
            var entity = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.RemoveAsync(entity.Id);
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
            var result = await _bookService.UpdateAsync(book);

            // Assert
            Assert.Equal(1, result);
            var bookEdited = await _bookService.GetByIdAsync(book.Id);
            Assert.Equal(newTitle, bookEdited.Title);

        }



        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
