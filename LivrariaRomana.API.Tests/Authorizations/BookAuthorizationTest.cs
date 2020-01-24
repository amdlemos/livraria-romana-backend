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
    public class BookAuthorizationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly BookBuilder _bookBuilder;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public BookAuthorizationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _bookBuilder = new BookBuilder();
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
        public async Task Book_GetAll_Without_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Act
            var response = await _client.GetAsync("api/book");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_GetById_Without_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Act
            var response = await _client.GetAsync("api/book/1");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Update_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            // Act
            var response = await _client.PutAsync("api/book/3/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Update_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
          
            // Act
            var response = await _client.PutAsync($"api/book/{ 1 }/", contentString);
            
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Post_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            // Act
            var response = await _client.PostAsync("api/book/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Post_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            // Act 
            var response = await _client.PostAsync($"api/book/", contentString);
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Remove_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            // Act
            var response = await _client.DeleteAsync("api/book/0");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Remove_With_Admin_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            await AuthenticateAsync();           
            // Act
            var response = await _client.DeleteAsync($"api/book/1");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }
    }
}
