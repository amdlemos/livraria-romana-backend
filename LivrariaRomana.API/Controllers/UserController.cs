using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Logger;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUserRepository _userRepository;
        private ILoggerManager _logger;
        public UserController(DatabaseContext context, IUserRepository userRepository, ILoggerManager logger)
        {
            _logger = logger;
            _context = context;
            _userRepository = userRepository;

        }

        // GET: api/Usuario
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<User>>> GetUsuarios()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os usuários.");
                var allUsers = await _userRepository.GetAllAsync();
                
                var result = allUsers.ToList();                

                _logger.LogInfo($"Retornando { result.Count } usuários.");                                
                return  result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }            
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> GetUsuario(int id)
        {
            _logger.LogInfo($"[GETbyID]Buscando usuário de ID: { id }.");
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogError($"Usuário de ID: { id } não foi encontrado.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Retornado usuário: { user.Username }.");
            return user;
        }

        // PUT: api/Usuario/5        
        //[Authorize("Admin")]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutUsuario(int id, User user)
        {            
            if (id != user.Id)
            {
                _logger.LogError($"[PUT-USER]Parâmetros incorretos.");
                return BadRequest();
            }

            try
            {
                _logger.LogInfo($"[PUT]Editando usuário de ID: { id }.");
                await _userRepository.UpdateAsync(user);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UsuarioExists(id))
                {
                    _logger.LogError($"Usuário não encontrado.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    return StatusCode(500, "Internal server error");
                }
            }
           
            _logger.LogInfo($"Usuário: { user.Username }, ID: { user.Id } editado com sucesso.");
            return NoContent();
        }

        // POST: api/Usuario        
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUsuario(User user)
        {

            if (user != null)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo usuário: { user.Username }.");
                    await _userRepository.AddAsync(user);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    return StatusCode(500, "Internal server error");
                }
            }
            else
            {
                _logger.LogError("[POST]Parâmetros incorretos.");
                return BadRequest();
            }
                

            _logger.LogInfo($"Usuário { user.Username}, ID: { user.Id } adicionado com sucesso.");
            return CreatedAtAction("GetUsuario", new { id = user.Id }, user);
        }

        // DELETE: api/Usuario/5
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> DeleteUsuario(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando usuário de ID: { id }.");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError($"Usuário de ID: { id } não encontrado.");
                return NotFound();
            }

            try
            {
                _logger.LogInfo($"Deletando usuário: { user.Username }, ID: { user.Id }.");
                await _userRepository.RemoveAsync(user);                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Usuário excluido com sucesso.");
            return user;
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
