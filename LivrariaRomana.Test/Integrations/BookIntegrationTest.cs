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
using LivrariaRomana.Domain.DTO;
using System.Net.Http.Headers;

namespace LivrariaRomana.Test.Integrations
{
    public class BookIntegrationTest
    {
        private readonly TestServer _server;
        public HttpClient Client;

        public BookIntegrationTest()
        {
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
            var response = await Client.GetAsync("api/book/1");
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
        public async Task Book_UpdateAsync_Returns500()
        {            
            var book = new BookBuilder().CreateBook();
            byte[] result = Utf8Json.JsonSerializer.Serialize(book);
            var p2 = JsonSerializer.Deserialize<BookDTO>(result);
            var json = JsonSerializer.ToJsonString(p2);
            var url = $"api/book/1/{ json }";
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));            
            //cl.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token));
            //var userAgent = "d-fens HttpClient";
            //cl.DefaultRequestHeaders.Add("User-Agent", userAgent);
            var response = await Client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}

