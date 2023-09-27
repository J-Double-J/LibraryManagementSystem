using Application;
using Domain.Abstract;
using Domain.Entities;
using Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;
using System;

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
                return await _context.Books.ToArrayAsync();
            }
            catch (Exception ex)
            {
                return Result.Failure(Enumerable.Empty<Book>(), new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
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
                return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
            
            return Result.Success();
        }

        public async Task<Result> DeleteBook(Guid guid)
        {
            try
            {
                Book? book = await _context.Books.FirstOrDefaultAsync(book => book.Id == guid);

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
                return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }

        }

        public async Task<Result<IEnumerable<Book>>> GetBooksByTitle(string title)
        {
            try
            {
                IEnumerable<Book> books = await _context.Books.Where(book => book.Title.ToUpper() == title.ToUpper()).ToArrayAsync();
                return Result.Success(books);
            }
            catch (Exception ex)
            {
                return Result.Failure(Enumerable.Empty<Book>(), new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result<Book>> GetBookByID(Guid id)
        {
            try
            {
                Book? book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

                if (book is not null)
                {
                    return book;
                }

                return Result.Failure<Book>(null, new(InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND,
                                                  $"No book with the ID `{id}` was found."));
            }
            catch (Exception ex)
            {
                return Result.Failure<Book>(null, new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result<Book>> UpdateBook(Guid guid, Book book)
        {
            try
            {
                Book? existingBook = await _context.Books.FirstOrDefaultAsync(book => book.Id == guid);

                if (existingBook is not null)
                {
                    book.CopyTo(existingBook);
                    await _context.SaveChanges();
                    return existingBook;
                }

                return Result.Failure<Book>(null, new(InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND,
                                                  $"No book with the ID `{guid}` was found."));
            }
            catch (Exception ex)
            {
                return Result.Failure<Book>(null, new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result> CheckoutBook(Book book)
        {
            try
            {
                Book? existingBook = await _context.Books.FindAsync(book.Id);

                if (existingBook is null)
                {
                    return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND,
                                              $"No book with the ID `{book.Id}` was found."));
                }
                
                existingBook.UpdateBookToCheckedOut();

                await _context.SaveChanges();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
        }
    }
}
