using Application.Interfaces;
using Domain.Abstract;
using MediatR;

namespace Application.CQRS.CheckoutCQRS
{
    public class RenewCheckoutCommand : ICommand<bool>
    {
        public Guid BookGuid { get; init; }
        public RenewCheckoutCommand(Guid bookGuid)
        {
            BookGuid = bookGuid;
        }

        public class RenewCheckoutCommandHander : ICommandHandler<RenewCheckoutCommand, bool>
        {
            ICheckoutRepository _checkoutRepository;

            public RenewCheckoutCommandHander(ICheckoutRepository checkoutRepository)
            {
                _checkoutRepository = checkoutRepository;
            }

            public async Task<Result<bool>> Handle(RenewCheckoutCommand request, CancellationToken cancellationToken)
            {
                return await _checkoutRepository.Renew(request.BookGuid);
            }
        }
    }
}
