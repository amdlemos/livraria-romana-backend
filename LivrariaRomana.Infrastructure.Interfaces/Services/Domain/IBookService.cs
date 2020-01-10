using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Services.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.Interfaces.Services.Domain
{
    public interface IBookService : IServiceBase<Book>
    {
    }
}
