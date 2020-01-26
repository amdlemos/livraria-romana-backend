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
    public class UserIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {       
        private readonly UserBuilder _userBuilder;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        public UserIntegrationTest(CustomWebApplicationFactory<Startup> factory)
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
        public async Task User_GetAllAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            await _client.PostAsync("api/user/", contentString);

            // Act
            var response = await _client.GetAsync("api/user");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUsers = response.Content.ReadAsAsync<List<UserDTO>>();
            returnedUsers.Result.Count.Should().BePositive();
        }

        [Fact]
        public async Task User_GetByIdAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var createdUser = await _client.PostAsync("api/user/", contentString);
            var userDTO = await createdUser.Content.ReadAsAsync<UserDTO>();
            
            // Act
            var response = await _client.GetAsync($"api/user/{ userDTO.id }");
            
            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUsers = await response.Content.ReadAsAsync<UserDTO>();
            returnedUsers.username.Should().Be("user");
        }

        [Fact]
        public async Task User_GetByIdAsync_With_Invalid_Parameter_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.GetAsync("api/user/dfd");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_GetByIdAsync_WhenUserDoesntExist_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.GetAsync("api/user/9999999");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_UpdateAsync_With_Invalid_Parameters_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", contentString);
            var userDTO = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            StringContent putContentString = JsonSerialize.GenerateStringContent(userDTO);

            // Act
            var response = await _client.PutAsync("api/user/999/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task User_UpdateAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent postContentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", postContentString);
            var user = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            user.username = "theos_sistemas";
            StringContent putContentString = JsonSerialize.GenerateStringContent(user);

            // Act
            var response = await _client.PutAsync($"api/user/{ user.id }/", putContentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedUser = await response.Content.ReadAsAsync<UserDTO>();
            returnedUser.username.Should().Be("theos_sistemas");
        }

        [Fact]
        public async Task User_UpdateAsync_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            var id = 99999;
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
                       
            // Act
            var response = await _client.PutAsync($"api/user/{ id }/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_AddAsync_Return_OK()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());

            // Act
            var response = await _client.PostAsync("api/user/", contentString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var returnedUser = await response.Content.ReadAsAsync<UserDTO>();
            returnedUser.username.Should().Be("user");
        }

        [Fact]
        public async Task User_AddAsync_Return_BadRequest()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUserWithEmptyPassword());
            // Act
            var response = await _client.PostAsync("api/user/", contentString);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task User_AddAsync_With_Null_Parameters_Return_UnsupportedMediaType()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.PostAsync("api/user/", null);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_Ok()
        {
            // Arrange
            Authenticate();
            StringContent contentString = JsonSerialize.GenerateStringContent(_userBuilder.CreateUser());
            var postResponse = await _client.PostAsync("api/user/", contentString);
            var userDTO = postResponse.Content.ReadAsAsync<UserDTO>().Result;
            
            // Act
            var response = await _client.DeleteAsync($"api/user/{ userDTO.id }");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_RemoveAsync_Return_NotFound()
        {
            // Arrange
            Authenticate();
            // Act
            var response = await _client.DeleteAsync($"api/user/999999");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
