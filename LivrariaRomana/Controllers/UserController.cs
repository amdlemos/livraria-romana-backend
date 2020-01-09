using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LivrariaRomana.Logger;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;

namespace LivrariaRomana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private ILoggerManager _logger;
        public UserController(DatabaseContext context, ILoggerManager logger)
        {
            _logger = logger;
            _context = context;

        }

        // GET: api/Usuario
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<User>>> GetUsuarios()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os usuários.");                
                var users = _context.Usuarios.ToListAsync();

                _logger.LogInfo($"Retornando { users.Result.Count } usuários.");                                
                return await users;
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
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                _logger.LogError($"Usuário de ID: { id } não foi encontrado.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Retornado usuário: { usuario.Username }.");
            return usuario;
        }

        // PUT: api/Usuario/5        
        //[Authorize("Admin")]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutUsuario(int id, User usuario)
        {            
            if (id != usuario.Id)
            {
                _logger.LogError($"[PUT-USER]Parâmetros incorretos.");
                return BadRequest();
            }

            _logger.LogInfo($"[PUT]Buscando usuário de ID: { id }.");
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                _logger.LogInfo($"Editando usuário: { usuario.Username }, ID: { usuario.Id }.");
                await _context.SaveChangesAsync();

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
           
            _logger.LogInfo($"Usuário: { usuario.Username }, ID: { usuario.Id } editado com sucesso.");
            return NoContent();
        }

        // POST: api/Usuario        
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUsuario(User usuario)
        {

            if (usuario != null)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo usuário: { usuario.Username }.");
                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
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
                

            _logger.LogInfo($"Usuário { usuario.Username}, ID: { usuario.Id } adicionado com sucesso.");
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuario/5
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> DeleteUsuario(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando usuário de ID: { id }.");
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                _logger.LogError($"Usuário de ID: { id } não encontrado.");
                return NotFound();
            }

            try
            {
                _logger.LogInfo($"Deletando usuário: { usuario.Username }, ID: { usuario.Id }.");
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Usuário excluido com sucesso.");
            return usuario;
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
