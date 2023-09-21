using Domain.Abstract;
using Domain.Entities.Patron;

namespace Application
{
    public interface IPatronRepository
    {
        Result<Patron> AddPatron(Patron patron);
    }
}