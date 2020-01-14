using LivrariaRomana.API.Controllers;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Logger;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using System.Net.Http;
using Xunit;
using LivrariaRomana.Test.DBConfiguration;
using System.Threading.Tasks;
using System.Linq;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Infrastructure.Services.Domain;

namespace LivrariaRomana.Test.Controllers
{
    public class BookControllerTest
    {
        private readonly DatabaseContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly ILoggerManager _logger;        
        
        public HttpClient Client;
        

        public BookControllerTest()
        {
            _dbContext = new Connection().DatabaseConfiguration();        
            _bookRepository = new BookRepository(_dbContext);
            _bookService = new BookService(_bookRepository);
            _logger = new LoggerManager();          
        }

        [Fact]
        public async Task CreateBookControllerTest()
        {
            var controller = new BookController(_dbContext, _bookService, _logger);
            var countBooks = await controller.GetLivros().ToAsyncEnumerable().Count();
            Assert.True(countBooks > 0);
        }
    }
}

