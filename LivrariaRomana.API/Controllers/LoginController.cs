﻿using System;
using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using System.Threading.Tasks;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Infrastructure.Notifications;
using AutoMapper;
using LivrariaRomana.IServices;

namespace LivrariaRomana.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILoggerManager _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private NotificationContext _notification;

        public LoginController(DatabaseContext context, ILoggerManager logger, IUserService userService, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _notification = new NotificationContext();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> PostAuthenticate(LoginDTO loginDTO)
        {
            try
            {                
                _logger.LogInfo($"[POST]Usuário: { loginDTO.username  } tentando fazer login.");
                var userLogado = await _userService.Authenticate(loginDTO.username, loginDTO.password);                

                if (userLogado == null)
                {
                    _logger.LogError($"USUÁRIO: { loginDTO.username } não foi encontrado.");
                    _notification.AddNotification("", "Usuário ou senha inválidos");

                    return BadRequest(_notification);
                }

                var userDTO = _mapper.Map<UserDTO>(userLogado);

                _logger.LogInfo($"USUÁRIO: { userDTO.username } logado.");
                return userDTO;
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