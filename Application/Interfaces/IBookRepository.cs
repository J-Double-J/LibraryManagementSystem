using Domain.Abstract;
using Domain.Entities;

namespace Application
{
    public interface IBookRepository
    {
        Task<Result<IEnumerable<Book>>> GetAllBooks();
        Task<Result> AddBook(Book book);
        Task<Result> DeleteBook(Guid guid);
    }
}