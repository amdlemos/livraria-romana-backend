using FluentAssertions;
using LivrariaRomana.Test.Helper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Xunit;
using LivrariaRomana.Domain.DTO;

namespace LivrariaRomana.API.Tests.Authorization
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
            Authenticate();
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
            Authenticate();
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
            Authenticate();
            // Act
            var response = await _client.DeleteAsync($"api/book/1");
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Book_Stock_Update_Without_Authentication_Return_Unauthorized()
        {
            // Arrange
            var bookUpdateAmount = new BookUpdateAmountDTO();
            bookUpdateAmount.id = 1;
            bookUpdateAmount.addToAmount = 5;
            bookUpdateAmount.removeToAmount = 1;
            StringContent contentString = JsonSerialize.GenerateStringContent(bookUpdateAmount);
            // Act
            var response = await _client.PutAsync("api/bookstock/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Book_Stock_Update_With_Authentication_Not_Return_Unauthorized_or_Forbidden()
        {
            // Arrange
            Authenticate();
            var bookUpdateAmount = new BookUpdateAmountDTO();
            bookUpdateAmount.id = 1;
            bookUpdateAmount.addToAmount = 5;
            bookUpdateAmount.removeToAmount = 1;
            StringContent contentString = JsonSerialize.GenerateStringContent(bookUpdateAmount);
            // Act
            var response = await _client.PutAsync("api/bookstock/", contentString);
            // Assert
            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }
    }
}
