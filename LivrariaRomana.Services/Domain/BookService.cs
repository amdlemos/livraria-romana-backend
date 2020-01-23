using LivrariaRomana.Domain.Entities;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;

namespace LivrariaRomana.Services
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
