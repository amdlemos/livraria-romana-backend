using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Linq;
using LivrariaRomana.TestingAssistent.DataBuilder;
using LivrariaRomana.TestingAssistent.DBConfiguration;

namespace LivrariaRomana.Repositories.Tests
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly DatabaseContext _dbContext;        
        private readonly IUserRepository _userRepository;

        private readonly UserBuilder _userBuilder;


        public UserRepositoryTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userBuilder = new UserBuilder();
            _dbContext.Database.BeginTransactionAsync();
        }

        [Fact]
        public async Task AddUserAsyncTest()
        {
            var result = await _userRepository.AddAsync(_userBuilder.CreateUser());

            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            var result = await _userRepository.GetByIdAsync(entity.Id);

            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            var result = await _userRepository.GetAllAsync();

            Assert.NotNull(result);

        }

        [Fact]
        public async Task RemoveAsync()
        {
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            var result = await _userRepository.RemoveAsync(entity.Id);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveAsyncObjTest()
        {
            var entity = await _userRepository.AddAsync(_userBuilder.CreateUser());
            var result = await _userRepository.RemoveAsync(entity);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAscyncTest()
        {
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());
            
            var newUsername = $"Nome Editado: + { DateTime.Now }";
            user.Username = newUsername;

            var result = await _userRepository.UpdateAsync(user);

            var userEdited = await _userRepository.GetByIdAsync(user.Id);            
            
            Assert.Equal(newUsername, userEdited.Username);
            Assert.Equal(1, result);
        }

        public void Dispose()
        {
            _dbContext.Database.RollbackTransaction();
        }
    }
}
