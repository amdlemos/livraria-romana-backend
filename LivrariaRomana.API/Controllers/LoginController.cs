using System;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using System.Threading.Tasks;
using LivrariaRomana.Domain.DTO;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILoggerManager _logger;
        private readonly IUserService _userService;

        public LoginController(DatabaseContext context, ILoggerManager logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostAuthenticate(LoginDTO login)
        {
            try
            {                
                _logger.LogInfo($"[POST]Usuário: { login.username } tentando fazer login.");
                var user = await _userService.Authenticate(login.username, login.password);                

                if (user == null)
                {
                    _logger.LogError($"USUÁRIO: { login.username } não foi encontrado.");                    
                    return BadRequest(new { message = "Usuário ou senha inválidos" });
                }

                _logger.LogInfo($"USUÁRIO: { login.username } logado.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}