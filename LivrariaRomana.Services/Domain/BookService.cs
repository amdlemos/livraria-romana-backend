using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.IRepositories;
using LivrariaRomana.IServices;
using System.Threading.Tasks;

namespace LivrariaRomana.Services
{
    public class BookService : ServiceBase<Book>, IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository repository) : base(repository)
        {
            _bookRepository = repository;
        }

        /// <summary>
        /// Verifica se livro existe.
        /// </summary>
        /// <param name="id">Id do livro.</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckBookExistById(int id)
        {
            var book = await base.GetByIdAsync(id);
            return book != null;
        }

        public async Task<int> UpdateAmountInStock(BookUpdateAmountDTO bookUpdateAmountDTO)
        {
            var book = await base.GetByIdAsync(bookUpdateAmountDTO.id);

            var totalAdded = bookUpdateAmountDTO.addToAmount - bookUpdateAmountDTO.removeToAmount;

            book.Amount = book.Amount + totalAdded;
            
            return  await base.UpdateAsync(book);
        }

        public async override Task<int> UpdateAsync(Book obj)
        {
            // Valido que o usuário não mudou o quantidade em estoque
            var book = await base.GetByIdAsync(obj.Id);
            obj.Amount = book.Amount;

            // Salvo o objeto
            return await base.UpdateAsync(obj);
        }
    }
}
