using System.Net.Http;
using System.Net;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using LivrariaRomana.API;
using LivrariaRomana.Test.DataBuilder;
using Utf8Json;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Test.DBConfiguration;
using System;

namespace LivrariaRomana.Test.Integrations
{
    public class BookIntegrationTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly BookBuilder _bookBuilder;
        private readonly TestServer _server;
        public HttpClient Client;

        public BookIntegrationTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();            
            _bookRepository = new BookRepository(_dbContext);
            _bookBuilder = new BookBuilder();
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = _server.CreateClient();
        }

        [Fact]       
        public async Task Book_GetAllAsync_ReturnsOkResponse()
        {
            var response = await Client.GetAsync("api/book");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);           
        }

        [Fact]
        public async Task Book_GetByIdAsync_ReturnsOkResponse()
        {
            var response = await Client.GetAsync("api/book/4");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_GetByIdAsync_ReturnBadRequest()
        {
            var response = await Client.GetAsync("api/book/dfd");
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_GetByIdAsync_Returns500()
        {
            var response = await Client.GetAsync("api/book/9999999");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_BadRequest()
        {            
            var book = _bookBuilder.CreateValidBook();
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");                                    
            var response = await Client.PutAsync("api/book/1/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_OkResponse()
        {
            var book = _bookBuilder.CreateBookWithId();
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PutAsync("api/book/4/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Book_UpdateAsync_Return_InternalServerError()
        {
            var book = _bookBuilder.CreateBookWithNonexistentId();
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PutAsync("api/book/9999999/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Book_AddAsync_Return_OkResponse()
        {
            var book = _bookBuilder.CreateValidBook();
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync("api/book/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Book_AddAsync_Return_500()
        {
            var book = new Book();
            var jsonSerialized = JsonSerialize.Serialize(book);
            var contentString = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync("api/book/", contentString);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Book_AddAsync_Return_UnsupportedMediaType_with_null_parameters()
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
        public async Task Book_RemoveAsync_Return_NotFound()
        {          
            var response = await Client.DeleteAsync($"api/book/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

