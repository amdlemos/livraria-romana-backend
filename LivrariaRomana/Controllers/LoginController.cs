using System;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Logger;
using LivrariaRomana.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILoggerManager _logger;

        public LoginController(DatabaseContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostLogin(Login login)
        {
            try
            {
                var service = new LoginService(_context);
                _logger.LogInfo($"[POST]Usuário: { login.Username } tentando fazer login.");
                var user = service.Authenticate(login.Username, login.Password);

                if (user == null)
                {
                    _logger.LogError($"USUÁRIO: { login.Username } não foi encontrado.");                    
                    return BadRequest(new { message = "Usuário ou senha inválidos" });
                }

                _logger.LogInfo($"USUÁRIO: { login.Username } logado.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}