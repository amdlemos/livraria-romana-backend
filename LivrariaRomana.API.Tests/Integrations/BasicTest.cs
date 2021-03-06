﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace LivrariaRomana.API.Tests.Integrations
{
    public class BasicTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private string urlAPI = "http://localhost:4726";
        public BasicTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]        
        [InlineData("/api/book")]        
        public async Task Get_Endpoints_ReturnSuccess_And_ApplicationJson_ContentType(string route)
        {
            // Arrange
            var url = urlAPI + route;
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert            
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/")]        
        public async Task Get_Endpoints_ReturnSuccess_And_TextHtml_ContentType(string route)
        {
            // Arrange
            var url = urlAPI + route;
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert            
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/user")]
        public async Task Get_Endpoints_Return_Unauthorized(string url)
        {
            var urlApi = "http://localhost:4726" + url;
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(urlApi);

            // Assert            
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);            
        }
    }
}
