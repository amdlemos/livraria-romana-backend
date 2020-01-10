using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Infrastructure.Services.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.Services.Domain
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _userRepository = repository;
        }
    }
}
