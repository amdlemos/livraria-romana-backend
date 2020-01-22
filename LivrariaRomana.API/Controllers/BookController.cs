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
using AutoMapper;

namespace LivrariaRomana.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]    
    public class BookController : ControllerBase
    {
        private readonly DatabaseContext _context;        
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private ILoggerManager _logger;
        private NotificationContext _notification;

        public BookController(DatabaseContext context, IBookService bookService, ILoggerManager logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
            _notification = new NotificationContext();
        }

        // GET: api/Livro
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetLivros()
        {
            try
            {
                _logger.LogInfo("[BOOK][GET]Buscando todos os livros.");
                var books = await _bookService.GetAllAsync();

                var result = books.OrderBy(x => x.Title).ToList();

                _logger.LogInfo($"Retornando { result.Count} livros.");
                var bookDTOs = result.Select(_mapper.Map<Book, BookDTO>).ToList();
                return bookDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                return StatusCode(500, _notification);
            }

        }

        // GET: api/Livro/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookDTO>> GetLivro(int id)
        {
            _logger.LogInfo($"[BOOK][GETbyID]Buscando livro de ID: { id }.");
            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não foi encontrado.");
                _notification.AddNotification("", "Livro não encontrado");
                return BadRequest(_notification);
            }

            var bookDTO = _mapper.Map<BookDTO>(book);

            _logger.LogInfo($"Retornado livro: { book.Title }.");
            return bookDTO;
        }

        // PUT: api/Livro/5        
        [HttpPut("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> PutLivro(int id, BookDTO bookDTO)
        {
            if (id != bookDTO.id)
            {
                _logger.LogError($"[BOOK][PUT]Parâmetros incorretos.");
                _notification.AddNotification("", "Parâmetros incorretos.");
                return BadRequest(_notification);
            }

            var book = new Book(bookDTO.title, bookDTO.author, bookDTO.originalTitle, bookDTO.publishingCompany, bookDTO.isbn, bookDTO.publicationYear, bookDTO.amount, bookDTO.id);

            if (book.Valid)
            {
                try
                {
                    _logger.LogInfo($"[BOOK][PUT]Editando livro de ID: { id }");
                    await _bookService.UpdateAsync(book);

                    _logger.LogInfo($"Livro: { book.Title }, ID: { book.Id } editado com sucesso.");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                    return StatusCode(500, _notification);
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
        [Authorize("admin")]
        public async Task<ActionResult<BookDTO>> PostLivro(BookDTO bookDTO)
        {
            var book = new Book(bookDTO.title, bookDTO.author, bookDTO.originalTitle, bookDTO.publishingCompany, bookDTO.isbn, bookDTO.publicationYear, bookDTO.amount);

            if (book.Valid)
            {
                try
                {
                    _logger.LogInfo($"[BOOK][POST]Adicionando novo livro: { book.Title}.");
                    await _bookService.AddAsync(book);

                    _logger.LogInfo($"Livro { book.Title }, ID: { book.Id } adicionado com sucesso.");
                    return CreatedAtAction("GetLivro", new { id = book.Id }, bookDTO);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                    return StatusCode(500, _notification);
                }
            }
            else
            {                
                _notification.AddNotifications(book.ValidationResult);                            

                return BadRequest(_notification);
            }
        }

        // DELETE: api/Livro/5        
        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<ActionResult<BookDTO>> DeleteLivro(int id)
        {
            _logger.LogInfo($"[BOOK][DELETE]Buscando livro de ID: { id }.");
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogError($"Livro de ID: { id } não encontrado.");
                _notification.AddNotification("", "Livro não encontrado.");
                return BadRequest(_notification);
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
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                return StatusCode(500, _notification);
            }
        }
    }
}
