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
using LivrariaRomana.Domain.Validators;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUserService _userService;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly NotificationContext _notification;

        public UserController(DatabaseContext context, IUserService userService, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _userService = userService;
            _notification = new NotificationContext();

        }

        // GET: api/Usuario
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsuarios()
        {
            try
            {
                // obtem
                _logger.LogInfo("[USER][GET]Buscando todos os usuários.");
                var users = await _userService.GetAllAsync();

                // mapeia lista
                var listUsers = users.ToList();
                var listUsersDTO = listUsers.Select(_mapper.Map<User, UserDTO>).ToList();

                // retorna
                _logger.LogInfo($"Retornando { listUsersDTO.Count } usuários.");
                return listUsersDTO;
            }
            catch (Exception ex)
            {
                // Erro
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                return StatusCode(500, _notification);
            }
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUsuario(int id)
        {
            // obtem 
            _logger.LogInfo($"[USER][GETbyID]Buscando usuário de ID: { id }.");
            var user = await _userService.GetByIdAsync(id);

            // valida
            if (user == null)
            {               
                // notifica 
                _logger.LogError($"Usuário { id } não foi encontrado ou não existe.");
                _notification.AddNotification("", "Usuário não encontrado");

                // retorna
                return BadRequest(_notification);
            }
            else
            {
                // mapeia 
                var userDTO = _mapper.Map<UserDTO>(user);

                _logger.LogInfo($"Retornado usuário: { userDTO.username }.");
                // retorna
                return Ok(userDTO);
            }
        }

        // PUT: api/Usuario/5               
        [HttpPut("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> PutUsuario(int id, UserDTO userDTO)
        {
            // mapeia
            var user  = _mapper.Map<User>(userDTO);

            // valida
            var userExist = await _userService.CheckUserExistById(id);
            if (user.Valid && userExist)
            {
                try
                {
                    
                    // edita
                    _logger.LogInfo($"[USER][PUT]Editando usuário de ID: { id }.");
                    var response = await _userService.UpdateAsync(user);

                    // retorna
                    _logger.LogInfo($"Usuário: { user.Username }, ID: { user.Id } editado com sucesso.");
                    userDTO.password = "p@ssword";

                    return Ok(userDTO);
                }
                catch (Exception ex)
                {
                    // error
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");

                    return StatusCode(500, _notification);
                }
            }
            else
            {
                // usuario inválido
                _notification.AddNotifications(user.ValidationResult);

                return BadRequest(_notification);
            }           
        }

        // POST: api/Usuario                
        [HttpPost]
        [Authorize("admin")]
        public async Task<ActionResult<UserDTO>> PostUsuario(UserDTO userDTO)
        {
            // cria 
            var user = new User(userDTO.username, userDTO.password, userDTO.email);

            // valida 
            if (user.Valid)
            {
                try
                {
                    // adicciona
                    _logger.LogInfo($"[USER][POST]Adicionando novo usuário: { user.Username }.");
                    user = await _userService.AddAsync(user);

                    if (user == null)
                    {
                        // falha
                        _notification.AddNotification("", "Não foi possível incluir o usuário pois o email ou username já estão em uso.");
                        return BadRequest(_notification);
                    }
                    else
                    {
                        // sucesso
                        userDTO = _mapper.Map<UserDTO>(user);
                        _logger.LogInfo($"Usuário { userDTO.username}, ID: { userDTO.id } adicionado com sucesso.");
                        return CreatedAtAction("GetUsuario", new { id = userDTO.id }, userDTO);
                    }
                    
                }
                catch (Exception ex)
                {
                    // error
                    _logger.LogError($"Algo deu errado: { ex.Message }.");
                    _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                    return StatusCode(500, _notification);
                }
            }
            else
            {
                // usuario inválido
                _notification.AddNotifications(user.ValidationResult);

                return BadRequest(_notification);
            }
        }

        // DELETE: api/Usuario/5        
        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<ActionResult<UserDTO>> DeleteUsuario(int id)
        {

            _logger.LogInfo($"[USER][DELETE]Buscando usuário de ID: { id }.");
            // obtem
            var user = await _userService.GetByIdAsync(id);

            // valida
            if (user == null)
            {
                // notifica
                _logger.LogError($"Usuário de ID: { id } não encontrado.");
                _notification.AddNotification("", "Usuário não encontrado.");
                // retorna
                return NotFound(_notification);
            }

            try
            {
                // remove
                _logger.LogInfo($"Deletando usuário: { user.Username }, ID: { user.Id }.");
                var removed = await _userService.RemoveAsync(user.Id);

                if (removed)
                {
                    // retorna ok
                    _logger.LogInfo($"Usuário excluido com sucesso.");
                    return Ok();
                }
                else
                {
                    _logger.LogError($"Não foi possível remover o usuário.");
                    return BadRequest();
                }
                
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
