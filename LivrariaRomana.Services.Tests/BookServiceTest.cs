using System;
using System.Linq;
using System.Threading.Tasks;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Test.Helper;
using Xunit;
using FluentAssertions;
using LivrariaRomana.Domain.DTO;

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
            var entity = await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.GetByIdAsync(entity.Id);
            // Assert
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            // Arrange
            await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.GetAllAsync();
            // Assert
            result.Count().Should().BePositive();
        }

        [Fact]
        public async Task RemoveAsync()
        {
            // Arrange
            var entity = await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            // Act
            var result = await _bookService.RemoveAsync(entity.Id);
            // Asset
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsyncTest()
        {
            // Arrange
            var book = await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            var newTitle = $"Title Editado: + { DateTime.Now }";
            book.Title = newTitle;

            // Acct
            var result = await _bookService.UpdateAsync(book);

            // Assert
            Assert.Equal(1, result);
            var bookEdited = await _bookService.GetByIdAsync(book.Id);
            Assert.Equal(newTitle, bookEdited.Title);

        }

        [Fact]
        public async Task UpdateBookAmountAsync()
        {
            // arrange
            // Arrange
            var book = await _bookService.AddAsync(_bookBuilder.CreateValidBook());
            var bookAmount = new BookUpdateAmountDTO();
            bookAmount.id = book.Id;
            bookAmount.addToAmount = 15;
            bookAmount.removeToAmount = 5;
            
            // act
            var updated = await _bookService.UpdateAmountInStock(bookAmount);


            // assert
            Assert.Equal(1, updated);
            var bookUpdated = await _bookService.GetByIdAsync(book.Id);
            bookUpdated.Amount.Should().Be(10);
        }



        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
