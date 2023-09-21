﻿using Application;
using Domain.Abstract;
using Domain.Entities.Patron;
using Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        IApplicationDbContext _context;
        public PatronRepository(IApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Result<Patron>> AddPatron(Patron patron)
        {
            try
            {
                _context.Patrons.Add(patron);
                await _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Result.Failure<Patron>(null, new Error(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }

            return Result.Success(patron);
        }

        public async Task<Result<IEnumerable<Patron>>> GetAllPatrons()
        {
            try
            {
                return await _context.Patrons.ToArrayAsync();
            }
            catch(Exception ex)
            {
                return Result.Failure(Enumerable.Empty<Patron>(), new Error(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }
        }
    }
}
