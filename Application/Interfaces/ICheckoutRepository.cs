using Domain.Abstract;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICheckoutRepository
    {
        public Task<Result> AddCheckoutTransaction(Checkout checkout);
        public Task<Result> ReturnBook(Guid bookGuid);
    }
}
