using Domain.Abstract;
using Domain.Entities.Patron;

namespace Application
{
    public interface IPatronRepository
    {
        Task<Result<Patron>> AddPatron(Patron patron);

        Task<Result<IEnumerable<Patron>>> GetAllPatrons();
    }
}