using Application;
using Domain.Abstract;
using Domain.Entities.Patron;
using Infrastructure.Errors;

namespace Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        IApplicationDbContext _context;
        public PatronRepository(IApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public Result<Patron> AddPatron(Patron patron)
        {
            try
            {
                _context.Patrons.Add(patron);
            }
            catch (Exception ex)
            {
                return Result.Failure<Patron>(null, new Error(InfrastructureErrors.BookRepositoryErrors.BOOK_DB_ERROR, ex.Message));
            }

            return Result.Success(patron);
        }
    }
}
