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
        public async Task Book_GetAllAsync_Return_OK()
        {
            // Arrange
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
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
            await AuthenticateAsync();
            // Act
            var response = await _client.PostAsync("api/book/", null);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Book_RemoveAsync_Return_Ok()
        {
            // Arrange
            await AuthenticateAsync();
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
            await AuthenticateAsync();
            // Act
            var response = await _client.DeleteAsync($"api/book/999999");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

