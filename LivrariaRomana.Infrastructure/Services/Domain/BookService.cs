using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Infrastructure.Services.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.Services.Domain
{
    public class BookService : ServiceBase<Book>, IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository repository) : base(repository)
        {
            _bookRepository = repository;
        }
    }
}
