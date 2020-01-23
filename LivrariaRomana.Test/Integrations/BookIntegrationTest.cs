﻿using FluentAssertions;
using LivrariaRomana.API;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using LivrariaRomana.Repositories;
using LivrariaRomana.Services;
using LivrariaRomana.Test.DataBuilder;
using LivrariaRomana.Test.DBConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.Test.Integrations
{
    public class BookIntegrationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly BookBuilder _bookBuilder;
        private readonly TestServer _testServer;
        private readonly Authentication _authentication;
        private HttpClient Client;

        public BookIntegrationTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();
            _bookRepository = new BookRepository(_dbContext);
            _userRepository = new UserRepository(_dbContext);
            _userService = new UserService(_userRepository);
            _bookBuilder = new BookBuilder();
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());            
            _authentication = new Authentication();

            // Autentica um usuário admin para poder realizar os testes.
            var userDTO = _authentication.LoginAsAdmin(_userService);
            
            Client = _authentication.CreateLoggedHttpClient(userDTO, _testServer);
        }



        [Fact]       
        public async Task Book_GetAllAsync_Return_OK()
        {
            var response = await Client.GetAsync("api/book");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);           
        }

        [Fact]
        public async Task Book_GetByIdAsync_Return_OK()
        {
            var response = await Client.GetAsync("api/book/4");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_GetByIdAsync_With_InvalidParameter_Return_BadRequest()
        {           
            var response = await Client.GetAsync("api/book/dfd");
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_GetByIdAsync_Return_BadRequest()
        {
            var response = await Client.GetAsync("api/book/9999999");
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_UpdateAsync_With_Invalid_Parameters_Return_BadRequest()
        {            
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PutAsync("api/book/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_OK()
        {
            var book = _bookBuilder.CreateBookWithId();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PutAsync("api/book/4/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_BadRequest()
        {
            var book = _bookBuilder.CreateBookWithNonexistentId();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PutAsync("api/book/9999999/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_AddAsync_Return_OK()
        {
            var book = _bookBuilder.CreateValidBook();

            StringContent contentString = JsonSerialize.GenerateStringContent(book);

            var response = await Client.PostAsync("api/book/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Book_AddAsync_Return_BadRequest()
        {
            var book = new Book();
            
            StringContent contentString = JsonSerialize.GenerateStringContent(book);
            
            var response = await Client.PostAsync("api/book/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_AddAsync_With_Null_Parameters_Return_UnsupportedMediaType()
        {            
            var response = await Client.PostAsync("api/book/", null);

            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Book_RemoveAsync_Return_Ok()
        {
            var book  = await _bookRepository.AddAsync(_bookBuilder.CreateValidBook());
            var allBook = await _bookRepository.GetAllAsync();
            var bookToDelete = allBook.LastOrDefault();
            var response = await Client.DeleteAsync($"api/book/{ bookToDelete.Id }");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_RemoveAsync_Return_BadRequest()
        {          
            var response = await Client.DeleteAsync($"api/book/999999");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

