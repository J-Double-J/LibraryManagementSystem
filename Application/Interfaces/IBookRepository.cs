using Domain.Entities;

namespace Application
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();
        Task AddBook(Book book);
    }
}