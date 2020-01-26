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
using Konscious.Security.Cryptography;

namespace LivrariaRomana.Services.Tests
{
    public class UserServiceTest : IDisposable
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;        
        private readonly UserBuilder _userBuilder;

        public UserServiceTest()
        {
            // Prepare Test 
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userService = new UserService(_userRepository);            
            _userBuilder = new UserBuilder();
            
            // Start Transaction
            _dbContext.Database.BeginTransaction();
        }

        [Fact]
        public async Task AddUserAsyncTest()
        {
            // Act
            var result = await _userService.AddAsync(_userBuilder.CreateUser());
            // Assert
            result.Id.Should().BePositive();
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act            
            var result = await _userService.GetByIdAsync(entity.Id);
            // Assert
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            // Arrange
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act
            var result = await _userService.GetAllAsync();
            // Assert
            result.Count().Should().BePositive();
        }

        [Fact]
        public async Task RemoveAsync()
        {
            // Arrange            
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            // Act
            var result = await _userService.RemoveAsync(entity.Id);
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
            var result = await _userService.UpdateAsync(user);

            // Assert
            Assert.Equal(1, result);
            var userEdited = await _userRepository.GetByIdAsync(user.Id);
            Assert.Equal(newUsername, userEdited.Username);
        }

        [Fact]
        public async Task AuthenticateAscyncTest()
        {
            // Arrange
            var user = await _userService.AddAsync(_userBuilder.CreateUser());

            // Act
            var userDTO = await _userService.Authenticate(user.Username, user.Password);

            // Assert
            userDTO.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckUserExistByUsernameTest()
        {
            // Arrange
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());

            // Act
            var exist = await _userService.CheckUserExistByUsername("user");

            // Assert
            exist.Should().BeTrue();
        }

        [Fact]
        public async Task CheckUserExistByEmailTest()
        {
            // Arrange
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());

            // Act
            var exist = await _userService.CheckUserExistByEmail("user@builder.com");

            // Assert
            exist.Should().BeTrue();

        }

        [Fact]
        public async Task CheckHashGenerationTest()
        {
            //byte[] password;
            //var hasher = new Argon2id(password);
        }

        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
