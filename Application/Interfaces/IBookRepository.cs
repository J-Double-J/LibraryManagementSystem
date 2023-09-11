using Domain.Entities;

namespace Application
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
    }
}