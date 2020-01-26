using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using System.Threading.Tasks;

namespace LivrariaRomana.IServices
{
    public interface IBookService : IServiceBase<Book>
    {
        Task<bool> CheckBookExistById(int id);

        Task<int> UpdateAmountInStock(BookUpdateAmountDTO bookUpdateAmountDTO);
    }
}
