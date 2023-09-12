using Application;
using Domain.Abstract;
using Domain.Entities;
using Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class BookRepository : IBookRepository
    {
        IApplicationDbContext _context;
        public BookRepository(IApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Result<IEnumerable<Book>>> GetAllBooks()
        {
            try
            {
                return _context.Books;
            }
            catch (Exception ex)
            {
                return Result.Failure(Enumerable.Empty<Book>(), new(InfrastructureErrors.BookRepositoryErrors.BOOK_GETALL_ERROR, ex.Message));

            }
        }

        public async Task<Result> AddBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_ADD_ERROR, ex.Message));
            }
            
            return Result.Success();
        }

        public async Task<Result> DeleteBook(Guid guid)
        {
            try
            {
                Book? book = _context.Books.FirstOrDefault(book => book.Id == guid);

                if (book is not null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChanges();
                    return Result.Success();
                }
                else
                {
                    return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND, $"No book with id '{guid}' was found."));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND, ex.Message));
            }

        }
    }
}
