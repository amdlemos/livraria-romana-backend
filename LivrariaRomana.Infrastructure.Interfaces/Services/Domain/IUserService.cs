using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Services.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LivrariaRomana.Infrastructure.Interfaces.Services.Domain
{
    public interface IUserService : IServiceBase<User>
    {
        Task<UserDTO> Authenticate(string username, string password);
    }
}
