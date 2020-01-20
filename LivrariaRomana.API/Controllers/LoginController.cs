using System;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using System.Threading.Tasks;


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
        public async Task<ActionResult<User>> PostAuthenticate(User user)
        {
            try
            {                
                _logger.LogInfo($"[POST]Usuário: { user.Username  } tentando fazer login.");
                var userLogado = await _userService.Authenticate(user.Username, user.Password);                

                if (userLogado == null)
                {
                    _logger.LogError($"USUÁRIO: { user.Username } não foi encontrado.");                    
                    return BadRequest(new { message = "Usuário ou senha inválidos" });
                }

                _logger.LogInfo($"USUÁRIO: { user.Username } logado.");
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