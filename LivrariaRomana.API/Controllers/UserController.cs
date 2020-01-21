﻿using System;
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
using static LivrariaRomana.Domain.Entities.User;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Infrastructure.Notifications;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUserService _userService;
        private ILoggerManager _logger;
        private NotificationContext _notification;

        public UserController(DatabaseContext context, IUserService userService, ILoggerManager logger)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
            _notification = new NotificationContext();

        }

        // GET: api/Usuario
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<User>>> GetUsuarios()
        {
            try
            {
                _logger.LogInfo("[GET]Buscando todos os usuários.");
                var allUsers = await _userService.GetAllAsync();

                var result = allUsers.ToList();

                _logger.LogInfo($"Retornando { result.Count } usuários.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                return StatusCode(500, _notification);
            }
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> GetUsuario(int id)
        {
            _logger.LogInfo($"[GETbyID]Buscando usuário de ID: { id }.");
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogError($"Usuário { id } não foi encontrado ou não existe.");
                _notification.AddNotification("", "Usuário não encontrado");
                return BadRequest(_notification);
            }

            _logger.LogInfo($"Retornado usuário: { user.Username }.");
            return user;
        }

        // PUT: api/Usuario/5        
        //[Authorize("Admin")]
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutUsuario(int id, UserDTO userDTO)
        {   
            if (id != userDTO.id)
            {
                _logger.LogError($"[PUT]Parâmetros incorretos.");
                _notification.AddNotification("", "Parâmetros incorretos.");
                return BadRequest(_notification);
            }

            var user = new User(userDTO.username, userDTO.password, userDTO.email, userDTO.id);

            if (user.Valid)
            {
                try
                {
                    _logger.LogInfo($"[PUT]Editando usuário de ID: { id }.");
                    await _userService.UpdateAsync(user);
                    
                    _logger.LogInfo($"Usuário: { user.Username }, ID: { user.Id } editado com sucesso.");
                    return Ok();
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
                _notification.AddNotifications(user.ValidationResult);

                return BadRequest(_notification);
            }
        }

        // POST: api/Usuario        
        // [Authorize("Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUsuario(UserDTO userDTO)
        {
            var user = new User(userDTO.username, userDTO.password, userDTO.email);

            if (user.Valid)
            {
                try
                {
                    _logger.LogInfo($"[POST]Adicionando novo usuário: { user.Username }.");
                    await _userService.AddAsync(user);

                    _logger.LogInfo($"Usuário { user.Username}, ID: { user.Id } adicionado com sucesso.");
                    return CreatedAtAction("GetUsuario", new { id = user.Id }, user);
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
                _notification.AddNotifications(user.ValidationResult);

                return BadRequest(_notification);
            }
        }

        // DELETE: api/Usuario/5
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> DeleteUsuario(int id)
        {
            _logger.LogInfo($"[DELETE]Buscando usuário de ID: { id }.");
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError($"Usuário de ID: { id } não encontrado.");
                _notification.AddNotification("", "Usuário não encontrado.");
                return NotFound(_notification);
            }

            try
            {
                _logger.LogInfo($"Deletando usuário: { user.Username }, ID: { user.Id }.");
                await _userService.RemoveAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Algo deu errado: { ex.Message }.");
                _notification.AddNotification("", "Algo deu errado, verifique o LOG para mais informações.");
                return StatusCode(500, _notification);
            }

            _logger.LogInfo($"Usuário excluido com sucesso.");
            return user;
        }
    }
}
