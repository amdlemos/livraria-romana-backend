using FluentAssertions;
using LivrariaRomana.API;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Services;
using LivrariaRomana.Test.DataBuilder;
using LivrariaRomana.Test.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.Test.Integrations
{
    public class UserIntegrationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly UserBuilder _userBuilder;
        private readonly TestServer _testServer;
        private readonly Authentication _authentication;
        private readonly HttpClient Client;

        public UserIntegrationTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userService = new UserService(_userRepository);
            _userBuilder = new UserBuilder();
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _authentication = new Authentication();

            // Autentica um usuário admin para poder realizar os testes.
            var userDTO = _authentication.LoginAsAdmin(_userService);

            Client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
        }

        [Fact]
        public async Task User_GetAllAsync_Return_OK()
        {
            var response = await Client.GetAsync("api/user");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_GetByIdAsync_Return_OK()
        {
            var response = await Client.GetAsync("api/user/1");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_GetByIdAsync_With_Invalid_Parameter_Return_BadRequest()
        {
            var response = await Client.GetAsync("api/user/dfd");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_GetByIdAsync_Return_BadRequest()
        {
            var response = await Client.GetAsync("api/user/9999999");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_BadRequest()
        {
            var user = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(user);

            var response = await Client.PutAsync("api/user/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_InternalServerError()
        {
            var user = _userBuilder.CreateUserWithNonexistentId();

            StringContent contentString = JsonSerialize.GenerateStringContent(user);
            
            var response = await Client.PutAsync("api/user/9999999/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_OK()
        {
            var user = _userBuilder.CreateUserWithId();

            StringContent contentString = JsonSerialize.GenerateStringContent(user);

            var response = await Client.PutAsync("api/user/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }     

        [Fact]
        public async Task User_AddAsync_Return_OK()
        {
            var user = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(user);

            var response = await Client.PostAsync("api/user/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task User_AddAsync_Return_BadRequest()
        {
            var user = new User();

            StringContent contentString = JsonSerialize.GenerateStringContent(user);

            var response = await Client.PostAsync("api/user/", contentString);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_AddAsync_With_Null_Parameters_Return_UnsupportedMediaType()
        {
            var response = await Client.PostAsync("api/user/", null);

            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_Ok()
        {
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());
            var allUser = await _userRepository.GetAllAsync();
            var userToDelete = allUser.LastOrDefault();
            var response = await Client.DeleteAsync($"api/user/{ userToDelete.Id }");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_NotFound()
        {
            var response = await Client.DeleteAsync($"api/user/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
