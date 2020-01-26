using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Domain.DTO;
using AutoMapper;
using LivrariaRomana.IServices;
using LivrariaRomana.API.Notifications;

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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetLivros()
        {
            try
            {
                // obtem
                _logger.LogInfo("[BOOK][GET]Buscando livros.");
                var books = await _bookService.GetAllAsync();

                // ordena e mapeia               
                var result = books.OrderBy(x => x.Title).ToList();
                var bookDTOs = result.Select(_mapper.Map<Book, BookDTO>).ToList();
                
                // retorna
                _logger.LogInfo($"Retornando { result.Count} livros.");                
                
                return bookDTOs;
            }
            catch (Exception ex)
            {
                // erro
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                
                return StatusCode(500, _notification);
            }

        }

        // GET: api/Livro/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDTO>> GetLivro(int id)
        {
            // obtem
            _logger.LogInfo($"[BOOK][GETbyID]Buscando livro de ID: { id }.");
            var book = await _bookService.GetByIdAsync(id);

            // valida
            if (book == null)
            {
                // notifica
                _logger.LogError($"Livro de ID: { id } não foi encontrado.");
                _notification.AddNotification("", "Livro não encontrado");
                return BadRequest(_notification);
            }

            // mapeia
            var bookDTO = _mapper.Map<BookDTO>(book);

            // retorna
            _logger.LogInfo($"Retornado livro: { book.Title }.");
            
            return bookDTO;
        }

        // PUT: api/Livro/5        
        [HttpPut("{id}")]
        [Authorize("admin")]
        public async Task<ActionResult<BookDTO>> PutLivro(int id, BookDTO bookDTO)
        {

            // obtem
            var book = await _bookService.GetByIdAsync(id);

            // valida existência
            if (book != null)
            {
                // mapeia
                book = _mapper.Map<Book>(bookDTO);

                // valida dados
                if (book.Valid)
                {
                    try
                    {
                        // edita
                        _logger.LogInfo($"[BOOK][PUT]Editando livro de ID: { id }");

                        await _bookService.UpdateAsync(book);

                        // mapeia
                        bookDTO = _mapper.Map<BookDTO>(book);

                        // retorna
                        _logger.LogInfo($"Livro: { bookDTO.title }, ID: { bookDTO.id } editado com sucesso.");

                        return Ok(bookDTO);
                    }
                    catch (Exception ex)
                    {
                        // erro
                        _logger.LogError($"Erro: { ex.Message }.");
                        _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");

                        return StatusCode(500, _notification);
                    }
                }
                else
                {
                    // livro inexistente
                    _notification.AddNotifications(book.ValidationResult);

                    return BadRequest(_notification);
                }
            }
            else
            {
                // livro inexistente
                _notification.AddNotification("","Livro não existe.");

                return BadRequest(_notification);                
            }
        }

        // POST: api/Livro       
        [HttpPost]
        [Authorize("admin")]
        public async Task<ActionResult<BookDTO>> PostLivro(BookDTO bookDTO)
        {
            // cria
            var book = new Book(bookDTO.title, bookDTO.author, bookDTO.originalTitle, bookDTO.publishingCompany, bookDTO.isbn, bookDTO.publicationYear, bookDTO.amount);

            // valida
            if (book.Valid)
            {
                try
                {
                    // adiciona
                    _logger.LogInfo($"[BOOK][POST]Adicionando novo livro: { book.Title}.");
                    book = await _bookService.AddAsync(book);

                    if (book == null)
                    {
                        // falha
                        _notification.AddNotification("", "Não foi possível incluir o livro.");
                        
                        return BadRequest(_notification);
                    }

                    // mapeia
                    bookDTO = _mapper.Map<BookDTO>(book);

                    // retorno
                    _logger.LogInfo($"Livro { bookDTO.title }, ID: { bookDTO.id } adicionado com sucesso.");
                    
                    return CreatedAtAction("GetLivro", new { id = book.Id }, bookDTO);
                }
                catch (Exception ex)
                {
                    // erro
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                    
                    return StatusCode(500, _notification);
                }
            }
            else
            {     
                // livro inválido
                _notification.AddNotifications(book.ValidationResult);                            

                return BadRequest(_notification);
            }
        }

        // DELETE: api/Livro/5        
        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<ActionResult<BookDTO>> DeleteLivro(int id)
        {
            // obtem
            _logger.LogInfo($"[BOOK][DELETE]Buscando livro de ID: { id }.");            
            var book = await _bookService.GetByIdAsync(id);

            // valida
            if (book == null)
            {
                // notifica
                _logger.LogError($"Livro de ID: { id } não encontrado.");
                _notification.AddNotification("", "Livro não encontrado.");

                return BadRequest(_notification);
            }
            try
            {
                // remove
                _logger.LogInfo($"Deletando livro: { book.Title }, ID: { book.Id }.");
                await _bookService.RemoveAsync(book.Id);

                // retorna
                _logger.LogInfo($"Livro excluido com sucesso.");               
                
                return Ok();
            }
            catch (Exception ex)
            {
                // erro
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                
                return StatusCode(500, _notification);
            }
        }
    }
}
