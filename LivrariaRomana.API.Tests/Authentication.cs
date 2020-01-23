using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.IServices;

namespace LivrariaRomana.API.Test
{
    /// <summary>
    /// Classe que auxilia na autenticação dos testes.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// Loga na aplicação como Admin
        /// </summary>
        /// <param name="_userService">IUserService interface de serviço de usuário</param>
        /// <returns>UserDTO com Token Válido</returns>
        public UserDTO LoginAsAdmin(IUserService _userService)
        {
            try
            {
                var user = _userService.GetAllAsync().Result.Where(x => x.Role == "admin").FirstOrDefault();
                
                if (user == null)
                {
                    var newUser = new User("admin", "admin", "admin@admin.com", "admin");
                    user = _userService.AddAsync(newUser).Result;                    
                }

                return _userService.Authenticate(user.Username, user.Password).Result;                
            }
            catch (Exception)
            {

                throw new Exception("Problemas com a autenticação, verifique os dados do usuário no banco.");
            }
        }

        /// <summary>
        /// Cria HttpClient com autorização Bearer        
        /// </summary>
        /// <param name="userDTO">UserDTO</param>
        /// <param name="testServer">TestServer</param>
        /// <returns>HttpClient com autorização Bearer</returns>
        public HttpClient CreateLoggedHttpClient(UserDTO userDTO, TestServer testServer)
        {
            try
            {
                var client = testServer.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {userDTO.token}");
                return client;
            }
            catch (Exception)
            {
                throw new Exception("Problemas com a criação do Client (HttpClient).");                
            }
         
        }
    }
}
