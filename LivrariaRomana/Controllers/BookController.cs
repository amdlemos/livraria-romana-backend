using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LivrariaRomana.Logger;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;

namespace LivrariaRomana.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly DataBaseContext _context;
        private ILoggerManager _logger;

        public BookController(DataBaseContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Livro
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetLivros()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os livros.");
                var books = _context.Livros.OrderBy(x => x.Title).ToListAsync();

                _logger.LogInfo($"Retornando { books.Result.Count } usuários.");
                return await books;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }
            
        }

        // GET: api/Livro/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> GetLivro(int id)
        {
            _logger.LogInfo($"[GETbyID]Buscando livro de ID: { id }.");
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
            {
                _logger.LogError($"Livro de ID: { id } não foi encontrado.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Retornado livro: { livro.Title }.");
            return livro;
        }

        // PUT: api/Livro/5
        // [Authorize("Admin")]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutLivro(int id, Book livro)
        {
            if (id != livro.Id)
            {
                _logger.LogError($"[PUT-BOOK]Parâmetros incorretos.");
                return BadRequest();                
            }
            _logger.LogInfo($"[PUT]Buscando livro de ID: { id }.");
            _context.Entry(livro).State = EntityState.Modified;

            try
            {
                _logger.LogInfo($"Editando livro: { livro.Title }, ID: { livro.Id }.");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!LivroExists(id))
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

            _logger.LogInfo($"Livro: { livro.Title }, ID: { livro.Id } editado com sucesso.");
            return NoContent();
        }

        // POST: api/Livro
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> PostLivro(Book livro)
        {
            if (livro != null)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo livro: { livro.Title }.");
                    await _context.SaveChangesAsync();
                    _context.Livros.Add(livro);
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

            _logger.LogInfo($"Livro { livro.Title }, ID: { livro.Id } adicionado com sucesso.");
            return CreatedAtAction("GetLivro", new { id = livro.Id }, livro);
        }

        // DELETE: api/Livro/5
        // [Authorize("Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> DeleteLivro(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando livro de ID: { id }.");
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                _logger.LogError($"Livro de ID: { id } não encontrado.");
                return NotFound();
            }

            try
            {
                _logger.LogInfo($"Deletando livro: { livro.Title }, ID: { livro.Id }.");
                _context.Livros.Remove(livro);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");                
            }

            _logger.LogInfo($"Usuário excluido com sucesso.");
            await _context.SaveChangesAsync();

            return livro;
        }

        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id);
        }
    }
}
