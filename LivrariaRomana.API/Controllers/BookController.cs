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
using LivrariaRomana.Domain.DTO;

namespace LivrariaRomana.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IBookRepository _bookRepository;
        private ILoggerManager _logger;

        public BookController(DatabaseContext context, IBookRepository bookRepository, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
            _bookRepository = bookRepository;
        }

        // GET: api/Livro
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetLivros()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os livros.");
                var books = await _bookRepository.GetAllAsync();

                var result = books.OrderBy(x => x.Title).ToList();

                _logger.LogInfo($"Retornando { result.Count() } usuários.");
                return result;
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
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não foi encontrado.");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInfo($"Retornado livro: { book.Title }.");
            return book;
        }

        // PUT: api/Livro/5
        // [Authorize("Admin")]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutLivro(int id, BookDTO bookDTO)
        {
            if (id != bookDTO.id)
            {
                _logger.LogError($"[PUT-BOOK]Parâmetros incorretos.");
                return BadRequest();               
            }

            try
            {                
                var book = new Book(
                    bookDTO.title,
                    bookDTO.author,
                    bookDTO.originalTitle,
                    bookDTO.publishingCompany,
                    bookDTO.isbn,
                    bookDTO.publicationYear,
                    bookDTO.amount,
                    bookDTO.id);

                if (book.Valid)
                {                    
                    await _bookRepository.UpdateAsync(book);

                    _logger.LogInfo($"Editando livro: { bookDTO.title }, ID: { bookDTO.id }.");
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return StatusCode(500, book.ValidationResult.Errors);
                }

            }
            catch (Exception ex)
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

            _logger.LogInfo($"Livro: { bookDTO.title }, ID: { bookDTO.id } editado com sucesso.");
            return Ok();
        }

        // POST: api/Livro
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> PostLivro(BookDTO bookDTO)
        {
            if (bookDTO != null)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo livro: { bookDTO.title }.");
                    var book = new Book(
                    bookDTO.title,
                    bookDTO.author,
                    bookDTO.originalTitle,
                    bookDTO.publishingCompany,
                    bookDTO.isbn,
                    bookDTO.publicationYear,
                    bookDTO.amount,
                    bookDTO.id);
                    if (book.Valid)
                    {
                        await _bookRepository.AddAsync(book);
                    }
                    else
                    {
                        return StatusCode(500, book.ValidationResult.Errors);
                    }
                    
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

            _logger.LogInfo($"Livro { bookDTO.title }, ID: { bookDTO.id } adicionado com sucesso.");
            return CreatedAtAction("GetLivro", new { id = bookDTO.id }, bookDTO);
        }

        // DELETE: api/Livro/5
        // [Authorize("Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> DeleteLivro(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando livro de ID: { id }.");
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não encontrado.");
                return NotFound();
            }
            try
            {
                _logger.LogInfo($"Deletando livro: { book.Title }, ID: { book.Id }.");
                await _bookRepository.RemoveAsync(book);

                _logger.LogInfo($"Usuário excluido com sucesso.");
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");                
            }            
        }

        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.Id == id);
        }
    }
}
