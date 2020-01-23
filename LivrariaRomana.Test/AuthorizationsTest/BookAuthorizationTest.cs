using FluentAssertions;
using LivrariaRomana.API;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Services;
using LivrariaRomana.Test.DataBuilder;
using LivrariaRomana.Test.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.Test.AuthorizationsTest
{
    public class BookAuthorizationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly BookBuilder _bookBuilder;
        private readonly TestServer _testServer;
        private readonly Authentication _authentication;
        private HttpClient Client;

        public BookAuthorizationTest()
        {
            _bookBuilder = new BookBuilder();
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userService = new UserService(_userRepository);
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _authentication = new Authentication();

            Client = _testServer.CreateClient();
        }

        [Fact]
        public async Task Book_GetAll_Without_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var response = await Client.GetAsync("api/book");

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_GetById_Without_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var response = await Client.GetAsync("api/book/1");

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Update_Without_Authentication_Return_Unauthorized()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PutAsync("api/book/3/", contentString);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Update_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.PutAsync($"api/book/{ book.Id }/", contentString);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Post_Without_Authentication_Return_Unauthorized()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PostAsync("api/book/", contentString);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Post_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.PostAsync($"api/book/", contentString);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Remove_Without_Authentication_Return_Unauthorized()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.DeleteAsync("api/book/0");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Remove_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            var userDTO = _authentication.LoginAsAdmin(_userService);
            var client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
            var response = await client.DeleteAsync($"api/book/");

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

    }
}
