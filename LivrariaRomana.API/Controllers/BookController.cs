using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Infrastructure.Notifications;

namespace LivrariaRomana.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly DatabaseContext _context;        
        private readonly IBookService _bookService;
        private ILoggerManager _logger;
        private NotificationContext _notification;

        public BookController(DatabaseContext context, IBookService bookService, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
            _bookService = bookService;
            _notification = new NotificationContext();
        }

        // GET: api/Livro
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetLivros()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os livros.");
                var books = await _bookService.GetAllAsync();

                var result = books.OrderBy(x => x.Title).ToList();

                _logger.LogInfo($"Retornando { result.Count() } livros.");
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
            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não foi encontrado.");
                return BadRequest("Livro não encontrado.");
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
                _logger.LogError($"[PUT]Parâmetros incorretos.");
                return BadRequest("Parâmetros incorretos.");
            }

            var book = new Book(bookDTO.title, bookDTO.author, bookDTO.originalTitle, bookDTO.publishingCompany, bookDTO.isbn, bookDTO.publicationYear, bookDTO.amount, bookDTO.id);

            if (book.Valid)
            {
                try
                {
                    _logger.LogInfo($"[PUT]Editando livro de ID: { id }");
                    await _bookService.UpdateAsync(book);

                    _logger.LogInfo($"Livro: { book.Title }, ID: { book.Id } editado com sucesso.");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro: { ex.Message }.");
                    return StatusCode(500, "Algo deu errado, verifique o LOG para mais informações.");
                }
            }
            else
            {
                _notification.AddNotifications(book.ValidationResult);

                return BadRequest(_notification);                
            }
        }

        // POST: api/Livro
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> PostLivro(BookDTO bookDTO)
        {
            var book = new Book(bookDTO.title, bookDTO.author, bookDTO.originalTitle, bookDTO.publishingCompany, bookDTO.isbn, bookDTO.publicationYear, bookDTO.amount);

            if (book.Valid)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo livro: { book.Title}.");
                    await _bookService.AddAsync(book);

                    _logger.LogInfo($"Livro { book.Title }, ID: { book.Id } adicionado com sucesso.");
                    return CreatedAtAction("GetLivro", new { id = book.Id }, book);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    return StatusCode(500, "Algo deu errado, verifique o LOG para mais informações.");
                }
            }
            else
            {                
                _notification.AddNotifications(book.ValidationResult);                            

                return BadRequest(_notification);
            }
        }

        // DELETE: api/Livro/5
        // [Authorize("Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> DeleteLivro(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando livro de ID: { id }.");
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não encontrado.");
                return BadRequest("Livro não encontrado.");
            }
            try
            {
                _logger.LogInfo($"Deletando livro: { book.Title }, ID: { book.Id }.");
                await _bookService.RemoveAsync(book);

                _logger.LogInfo($"Livro excluido com sucesso.");
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
