using FluentAssertions;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests
{
    public class LoginIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {        
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;                
    
        public LoginIntegrationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

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
        public async Task Login_Post_Return_OK()
        {
            // Arrange                   
            await AuthenticateAsync();
            StringContent userContentString = JsonSerialize.GenerateStringContent(new User("admin","admin","admin@email.com","admin"));
            await _client.PostAsync("api/user/", userContentString);

            StringContent loginContentString = JsonSerialize.GenerateStringContent(new LoginDTO() { password = "admin", username = "admin" });
            // Act
            var response = await _client.PostAsync("api/login/", loginContentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
