using Application;
using Application.Interfaces;
using Domain.Abstract;
using Domain.Entities;
using Infrastructure.Errors;

namespace Infrastructure.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        IApplicationDbContext _context;

        public CheckoutRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> AddCheckoutTransaction(Checkout checkout)
        {
            try
            {
                _context.Checkout.Add(checkout);

                await _context.SaveChanges();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_DB_ERROR, ex.Message));
            }
        }
    }
}
