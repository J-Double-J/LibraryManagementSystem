﻿using Domain.Abstract;
using Domain.Entities;

namespace Application
{
    public interface IBookRepository
    {
        Task<Result<IEnumerable<Book>>> GetAllBooks();

        Task<Result<Book>> GetBookByID(Guid id);

        Task<Result> AddBook(Book book);

        Task<Result> DeleteBook(Guid guid);

        Task<Result<IEnumerable<Book>>> GetBooksByTitle(string title);

        Task<Result<Book>> UpdateBook(Guid guid, Book book);
        Task<Result> CheckoutBook(Book value);
    }
}