using FluentAssertions;
using LivrariaRomana.API.Tests;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests
{
    public class UserIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {       
        private readonly UserBuilder _userBuilder;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public UserIntegrationTest(CustomWebApplicationFactory<Startup> factory)
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
        public async Task User_GetAllAsync_Return_OK()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            await _client.PostAsync("api/user/", contentString);

            // Act
            var response = await _client.GetAsync("api/user");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUsers = response.Content.ReadAsAsync<List<UserDTO>>();
            returnedUsers.Result.Count.Should().BePositive();
        }

        [Fact]
        public async Task User_GetByIdAsync_Return_OK()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var createdUser = await _client.PostAsync("api/user/", contentString);
            var id = createdUser.Content.ReadAsAsync<UserDTO>().Result.id;
            
            // Act
            var response = await _client.GetAsync($"api/user/{ id }");
            
            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUsers = await response.Content.ReadAsAsync<UserDTO>();
            returnedUsers.username.Should().Be("user");
        }

        [Fact]
        public async Task User_GetByIdAsync_With_Invalid_Parameter_Return_BadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await _client.GetAsync("api/user/dfd");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_GetByIdAsync_WhenUserDoesntExist_Return_BadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await _client.GetAsync("api/user/9999999");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_UpdateAsync_With_Invalid_Parameters_Return_BadRequest()
        {           
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", contentString);
            var userDTO = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            StringContent putContentString = JsonSerialize.GenerateStringContent(userDTO);

            // Act
            var response = await _client.PutAsync("api/user/1/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task User_UpdateAsync_Return_OK()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", postContentString);
            var userDTO = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            userDTO.username = "theos_sistemas";
            StringContent putContentString = JsonSerialize.GenerateStringContent(userDTO);

            // Act
            var response = await _client.PutAsync($"api/user/{ userDTO.id }/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUser = await response.Content.ReadAsAsync<UserDTO>();
            returnedUser.username.Should().Be("theos_sistemas");
        }

        [Fact]
        public async Task User_UpdateAsync_Return_BadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var id = 99999;
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
                       
            // Act
            var response = await _client.PutAsync($"api/user/{ id }/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_AddAsync_Return_OK()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());

            // Act
            var response = await _client.PostAsync("api/user/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var returnedUser = await response.Content.ReadAsAsync<UserDTO>();
            returnedUser.username.Should().Be("user");
        }

        [Fact]
        public async Task User_AddAsync_Return_BadRequest()
        {
            // Arrange
            await AuthenticateAsync();            
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUserWithEmptyPassword());
            // Act
            var response = await _client.PostAsync("api/user/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_AddAsync_With_Null_Parameters_Return_UnsupportedMediaType()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await _client.PostAsync("api/user/", null);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_Ok()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", contentString);
            var userDTO = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            
            // Act
            var response = await _client.DeleteAsync($"api/user/{ userDTO.id }");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_NotFound()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await _client.DeleteAsync($"api/user/999999");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
