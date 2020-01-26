using System;
using System.Threading.Tasks;
using LivrariaRomana.API.Notifications;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivrariaRomana.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookStockController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IBookService _bookService;
        private ILoggerManager _logger;
        private NotificationContext _notification;

        public BookStockController(DatabaseContext context, IBookService bookService, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
            _bookService = bookService;            
            _notification = new NotificationContext();
        }

        // PUT: api/BookStock/5       
        [HttpPut]
        [Authorize("admin")]
        public async Task<ActionResult<bool>> PutBookUpdateAmount(BookUpdateAmountDTO bookUpdateAmountDTO)
        {
            // valida
            if (bookUpdateAmountDTO.addToAmount < 0 || bookUpdateAmountDTO.removeToAmount < 0)
            {
                // erro
                _notification.AddNotification("", "Os valores devem ser sempre positivo.");
                return BadRequest(_notification);
            }
            else
            {
                try
                {
                    if (bookUpdateAmountDTO.id > 0)
                    {
                        // edita
                        var updated = await _bookService.UpdateAmountInStock(bookUpdateAmountDTO);
                        if (updated == 1)
                        {
                            // sucesso
                            return Ok(true);
                        }
                        else
                        {
                            // falha
                            _logger.LogError($"Não foi possível atualizar o estoque.");
                            _notification.AddNotification("", "Algo deu errado, verifique se o livro já foi adicionado ao sistema.");

                            return BadRequest(_notification);
                        }
                    }
                    else
                    {
                        // erro
                        _logger.LogError($"Não foi possível atualizar o estoque, livro inexistente.");
                        _notification.AddNotification("", "Algo deu errado, verifique se o livro já foi adicionado ao sistema.");

                        return BadRequest(_notification);
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    // Erro
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                    
                    return StatusCode(500, _notification);
                }
            }
        }
    }
}
