using Application;
using Domain.Entities;

namespace Infrastructure
{
    public class BookRepository : IBookRepository
    {
        IApplicationDbContext _context;
        public BookRepository(IApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChanges();
        }
    }
}
