using Application;
using Domain.Abstract;
using Domain.Entities;
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
                return Result.Failure<Patron>(null, new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_DB_ERROR, ex.Message));
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
                return Result.Failure(Enumerable.Empty<Patron>(), new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result<Patron>> GetPatronByID(Guid id)
        {
            try
            {
                Patron? patron = _context.Patrons.FirstOrDefault(p => p.Id == id);

                if (patron is null)
                {
                    return Result.Failure<Patron>(null, new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_ID_NOT_FOUND, $"Patron with ID `{id}` not found."));
                }

                return patron!;
            }
            catch (Exception ex)
            {
                return Result.Failure<Patron>(null, new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result> DeletePatron(Guid id)
        {
            try
            {
                Patron? patron = await _context.Patrons.FirstOrDefaultAsync(p => p.Id == id);

                if (patron is null)
                {
                    return Result.Failure<Patron>(null, new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_ID_NOT_FOUND, $"Patron with ID `{id}` not found."));
                }

                _context.Patrons.Remove(patron);
                await _context.SaveChanges();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure<Patron>(null, new Error(InfrastructureErrors.PatronRepositoryErrors.PATRON_DB_ERROR, ex.Message));
            }
        }
    }
}
