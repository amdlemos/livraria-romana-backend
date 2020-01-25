using FluentAssertions;
using LivrariaRomana.Domain.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests.Authorization
{
    public class LoginAuthorizationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public LoginAuthorizationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Login_Post()
        {
            // Arrange            
            StringContent contentString = JsonSerialize.GenerateStringContent(new LoginDTO() { password = "admin", username = "admin" });
            // Act
            var response = await _client.PostAsync("api/login/", contentString);
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

    }
}
