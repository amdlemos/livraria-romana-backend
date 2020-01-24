using FluentAssertions;
using LivrariaRomana.API.Tests;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Services;
using LivrariaRomana.TestingAssistent.DataBuilder;
using LivrariaRomana.TestingAssistent.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests
{
    public class UserAuthorizationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly UserBuilder _userBuilder;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public UserAuthorizationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _userBuilder = new UserBuilder();
        }

        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {

            var response = await _client.PostAsJsonAsync("/api/login", new LoginDTO
            {
                username = "test@integration.com",
                password = "SomePass1234!"

            });

            var registrationResponse = await response.Content.ReadAsAsync<UserDTO>();
            return registrationResponse.token;

        }

        [Fact]
        public async Task User_GetAll_Without_Authentication_Return_Unauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/user");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);            
        }

        [Fact]
        public async Task User_GetById_Without_Authentication_Return_Unauthorized()
        {
            // Act
            var response = await _client.GetAsync("api/user/1");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Update_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            // Act
            var response = await _client.PutAsync("api/user/1/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Update_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            // Act
            var response = await _client.PutAsync($"api/user/{ 1 }/", contentString);
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task User_Post_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            //Act
            var response = await _client.PostAsync("api/user/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Post_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            // Act
            var response = await _client.PostAsync($"api/user/", contentString);
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task User_Remove_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            // Act
            var response = await _client.DeleteAsync("api/user/0");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task User_Remove_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await _client.DeleteAsync($"api/user/1");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }
    }
}
