using FluentAssertions;
using LivrariaRomana.API.Test;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Services;
using LivrariaRomana.TestingAssistent.DataBuilder;
using LivrariaRomana.TestingAssistent.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Authorizations.Tests
{
    public class UserAuthorizationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly UserBuilder _userBuilder;
        private readonly TestServer _testServer;
        private readonly Authentication _authentication;
        private HttpClient Client;

        public UserAuthorizationTest()
        {
            _userBuilder = new UserBuilder();
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userService = new UserService(_userRepository);
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _authentication = new Authentication();
            
            Client = _testServer.CreateClient();
        }

        [Fact]
        public async Task User_GetAll_Without_Authentication_Return_Unauthorized()
        {
            var response = await Client.GetAsync("api/user");
            
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_GetById_Without_Authentication_Return_Unauthorized()
        {
            var response = await Client.GetAsync("api/user/1");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Update_Without_Authentication_Return_Unauthorized()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PutAsync("api/user/1/", contentString);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Update_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.PutAsync($"api/user/{ book.Id }/", contentString);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task User_Post_Without_Authentication_Return_Unauthorized()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PostAsync("api/user/", contentString);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Post_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.PostAsync($"api/user/", contentString);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task User_Remove_Without_Authentication_Return_Unauthorized()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.DeleteAsync("api/user/0");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Remove_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _userBuilder.CreateUser();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.DeleteAsync($"api/user/0");

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

    }
}
