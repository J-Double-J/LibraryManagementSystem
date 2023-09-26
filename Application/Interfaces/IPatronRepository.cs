using Domain.Abstract;
using Domain.Entities;

namespace Application
{
    public interface IPatronRepository
    {
        Task<Result<Patron>> AddPatron(Patron patron);

        Task<Result<IEnumerable<Patron>>> GetAllPatrons();

        Task<Result<Patron>> GetPatronByID(Guid id);

        Task<Result> DeletePatron(Guid id);
    }
}