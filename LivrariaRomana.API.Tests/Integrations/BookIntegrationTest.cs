using FluentAssertions;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Test.Helper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests.Integrations
{
    public class BookIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {   
        private readonly BookBuilder _bookBuilder;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;        

        public BookIntegrationTest(CustomWebApplicationFactory<Startup> factory)
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
        public async Task Book_GetAllAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            await _client.PostAsync("api/book/", contentString);

            // Act
            var response = await _client.GetAsync("api/book");
            
            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedBooks = response.Content.ReadAsAsync<List<BookDTO>>();
            returnedBooks.Result.Count.Should().BePositive();
        }

        [Fact]
        public async Task Book_GetByIdAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var createdBook = await _client.PostAsync("api/book/", contentString);
            var id = createdBook.Content.ReadAsAsync<BookDTO>().Result.id;

            // Act
            var response = await _client.GetAsync($"api/book/{ id }");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedBook = await response.Content.ReadAsAsync<BookDTO>();
            returnedBook.author.Should().Be("Author from Builder");
        }

        [Fact]
        public async Task Book_GetByIdAsync_With_InvalidParameter_Return_BadRequest()
        {     
            // Act
            var response = await _client.GetAsync("api/book/dfd");            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_GetByIdAsync_WhenBookDoesntExist_Return_BadRequest()
        {
            // Act
            var response = await _client.GetAsync("api/book/9999999");            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_UpdateAsync_With_Invalid_Parameters_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var postResponse = await _client.PostAsync("api/book/", postContentString);
            var bookDTO = postResponse.Content.ReadAsAsync<BookDTO>().Result;
            StringContent putContentString = JsonSerialize.GenerateStringContent(bookDTO);

            // Act
            var response = await _client.PutAsync($"api/book/{ 9999 } /", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var postResponse = await _client.PostAsync("api/book/", postContentString);
            var bookDTO = postResponse.Content.ReadAsAsync<BookDTO>().Result;
            bookDTO.title = "titulo editado";
            StringContent putContentString = JsonSerialize.GenerateStringContent(bookDTO);

            // Act
            var response = await _client.PutAsync($"api/book/{ bookDTO.id }/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedBook = await response.Content.ReadAsAsync<BookDTO>();
            returnedBook.title.Should().Be("titulo editado");
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            var id = 99999;
            StringContent contentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateBookWithNonexistentId(id));

            // Act
            var response = await _client.PutAsync($"api/book/{ id }/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_AddAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            
            // Act
            var postResponse = await _client.PostAsync("api/book/", postContentString);            
            
            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdBook = await postResponse.Content.ReadAsAsync<BookDTO>();
            createdBook.author.Should().Be("Author from Builder");

        }

        [Fact]
        public async Task Book_AddAsync_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            var bookDTO = new BookDTO();            
            StringContent contentString = JsonSerialize.GenerateStringContent(bookDTO);
            
            // Act
            var response = await _client.PostAsync("api/book/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_AddAsync_With_Null_Parameters_Return_UnsupportedMediaType()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.PostAsync("api/book/", null);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Book_RemoveAsync_Return_Ok()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var postResponse = await _client.PostAsync("api/book/", postContentString);
            var bookDTO = postResponse.Content.ReadAsAsync<BookDTO>().Result;

            // Act
            var response = await _client.DeleteAsync($"api/book/{ bookDTO.id }");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_RemoveAsync_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.DeleteAsync($"api/book/999999");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_Store_Update_Amount_Return_OK()
        {
            // Arrange
            Authenticate();            
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var postResponse = await _client.PostAsync("api/book/", postContentString);
            var bookDTO = await postResponse.Content.ReadAsAsync<BookDTO>();

            var bookUpdateAmount = new BookUpdateAmountDTO();
            bookUpdateAmount.id = bookDTO.id;
            bookUpdateAmount.addToAmount = 5;
            bookUpdateAmount.removeToAmount = 1;
            StringContent putContentString = JsonSerialize.GenerateStringContent(bookUpdateAmount);

            // Act
            var response = await _client.PutAsync("api/bookstock/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_Store_Update_Amount_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_bookBuilder.CreateValidBook());
            var postResponse = await _client.PostAsync("api/book/", postContentString);
            var bookDTO = await postResponse.Content.ReadAsAsync<BookDTO>();

            var bookUpdateAmount = new BookUpdateAmountDTO();
            bookUpdateAmount.id = bookDTO.id;
            bookUpdateAmount.addToAmount = -5;
            bookUpdateAmount.removeToAmount = 1;
            StringContent putContentString = JsonSerialize.GenerateStringContent(bookUpdateAmount);

            // Act
            var response = await _client.PutAsync("api/bookstock/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

