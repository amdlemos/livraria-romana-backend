using FluentAssertions;
using LivrariaRomana.API;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Test.DataBuilder;
using LivrariaRomana.Test.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Xunit;

namespace LivrariaRomana.Test.Integrations
{
    public class UserIntegrationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly UserBuilder _userBuilder;
        private readonly TestServer _server;
        public HttpClient Client;

        public UserIntegrationTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();
            _userRepository = new UserRepository(_dbContext);
            _userBuilder = new UserBuilder();
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = _server.CreateClient();
        }

        [Fact]
        public async Task User_GetAllAsync_ReturnsOkResponse()
        {
            var response = await Client.GetAsync("api/user");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_GetByIdAsync_ReturnsOkResponse()
        {
            var response = await Client.GetAsync("api/user/1");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_GetByIdAsync_ReturnBadRequest()
        {
            var response = await Client.GetAsync("api/user/dfd");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_GetByIdAsync_Returns500()
        {
            var response = await Client.GetAsync("api/user/9999999");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_BadRequest()
        {
            var user = _userBuilder.CreateUser();
            var jsonSerialized = JsonSerialize.Serialize(user);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PutAsync("api/user/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_OkResponse()
        {
            var user = _userBuilder.CreateUserWithId();
            var jsonSerialized = JsonSerialize.Serialize(user);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PutAsync("api/user/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_UpdateAsync_Return_NotFound()
        {
            var user = _userBuilder.CreateUserWithNonexistentId();
            var jsonSerialized = JsonSerialize.Serialize(user);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PutAsync("api/user/9999999/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task User_AddAsync_Return_OkResponse()
        {
            var user = _userBuilder.CreateUser();
            var jsonSerialized = JsonSerialize.Serialize(user);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync("api/user/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task user_AddAsync_Return_500()
        {
            var user = new User();
            var jsonSerialized = JsonSerialize.Serialize(user);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync("api/user/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task user_RemoveAsync_Return_Ok()
        {
            var user = await _userRepository.AddAsync(_userBuilder.CreateUser());
            var allUser = await _userRepository.GetAllAsync();
            var userToDelete = allUser.LastOrDefault();
            var response = await Client.DeleteAsync($"api/user/{ userToDelete.Id }");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task user_RemoveAsync_Return_NotFound()
        {
            var response = await Client.DeleteAsync($"api/user/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
