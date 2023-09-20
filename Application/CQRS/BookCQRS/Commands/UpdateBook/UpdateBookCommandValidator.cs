using Domain.CustomFluentValidation;
using FluentValidation;

namespace Application.CQRS.BookCQRS.Commands.UpdateBook
{
    public class UpdateBookCommandValidator : LibraryValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
            : base(LibraryValidatorType.Request)
        {
            RuleFor(command => command.BookToUpdateID).NotNull();

            RuleFor(command => command.Author).NotEmpty().When(command => command.Author != null);

            RuleFor(command => command.Title).NotEmpty().When(command => command.Title != null);

            RuleFor(command => command.Description).NotEmpty().When(command => command.Description != null);

            RuleFor(command => command.Pages).GreaterThan(0);

            RuleFor(command => command.DatePublished).Must(BeAValidDateIfNotNull);

            RuleFor(command => command.Publisher).NotEmpty().When(command => command.Publisher != null);
        }

        private bool BeAValidDateIfNotNull(DateOnly dateOnly)
        {
            // We treat min value as null
            if (dateOnly == DateOnly.MinValue)
            {
                return true;
            }

            if (dateOnly > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return false;   
            }

            return true;
        }
    }
}
