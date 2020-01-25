using FluentAssertions;
using LivrariaRomana.Test.Helper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests.Authorization
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


        protected void Authenticate()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetJwt());
        }

        private string GetJwt()
        {
            // Cria chave
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            // Cria token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim("bookStore", "admin")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Gera e retorna token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
            Authenticate();
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
            Authenticate();
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
            Authenticate();
            // Act
            var response = await _client.DeleteAsync($"api/user/1");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }
    }
}
