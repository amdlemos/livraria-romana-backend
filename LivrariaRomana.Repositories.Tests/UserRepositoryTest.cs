using System.Threading.Tasks;
using System;
using System.Linq;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.Test.Helper;
using Xunit;
using FluentAssertions;

namespace LivrariaRomana.Repositories.Tests
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly DatabaseContext _dbContext;        
        private readonly IUserRepository _userRepository;
        private readonly UserBuilder _userBuilder;

        public UserRepositoryTest()
        {
            // Prepare Test
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userBuilder = new UserBuilder();

            // Start Transaction
            _dbContext.Database.BeginTransaction();
        }

        [Fact]
        public async Task AddUserAsyncTest()
        {
            // Act
            var result = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Assert
            result.Id.Should().BePositive();
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act            
            var result = await _userRepository.GetByIdAsync(entity.Id);
            // Assert
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            // Arrange
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act
            var result = await _userRepository.GetAllAsync();
            // Assert
            result.Count().Should().BePositive();
        }

        [Fact]
        public async Task RemoveAsync()
        {
            // Arrange            
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act
            var result = await _userRepository.RemoveAsync(entity.Id);
            // Assert
            Assert.True(result);
        }      

        [Fact]
        public async Task UpdateAscyncTest()
        {
            // Arrange
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());            
            var newUsername = $"Nome Editado: + { DateTime.Now }";
            user.Username = newUsername;

            // Act
            var result = await _userRepository.UpdateAsync(user);

            // Assert
            Assert.Equal(1, result);
            var userEdited = await _userRepository.GetByIdAsync(user.Id);
            Assert.Equal(newUsername, userEdited.Username);
        }

        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
