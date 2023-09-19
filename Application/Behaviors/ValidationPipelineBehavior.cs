using Application.CQRS;
using Domain.Abstract;
using Domain.CustomFluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandBase
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            try
            {

                // Validate with the 'fast' validators first.
                var libraryValidationResult = await Task.WhenAll(_validators.OfType<LibraryValidator<TRequest>>()
                                                   .Where(validator => !validator.IsSlowValidator)
                                                   .Select(validator => validator.ValidateAsync(context)));

                // Validate with 'slow' validators if there were no validation errors with fast validators.
                if (libraryValidationResult.All(result => result.IsValid == true))
                {
                    libraryValidationResult = await Task.WhenAll(_validators.OfType<LibraryValidator<TRequest>>()
                                                   .Where(validator => validator.IsSlowValidator)
                                                   .Select(validator => validator.ValidateAsync(context)));
                }

                ValidationError[] errors = libraryValidationResult
                    .Where(ValidationResult => !ValidationResult.IsValid)
                    .SelectMany(validationResult => validationResult.Errors)
                    .Select(failure => { _logger.LogInformation("Error code : {@error}", failure.ErrorCode); return failure; })
                    .Select(failure => new ValidationError(ErrorCode.ConstructFromStringRepresentation(failure.ErrorCode),
                                                           $"`{failure.PropertyName}`: {failure.ErrorMessage}"))
                    .Distinct()
                    .ToArray();

                if (errors.Any())
                {
                    return CreateValidationResult<TResponse>(errors);
                }

                return await next();
            }
            catch (InvalidCastException)
            {
                _logger.LogCritical($"All validators must be of type {typeof(LibraryValidator<>)}");
                throw;
            }
        }

        private static TResult CreateValidationResult<TResult>(ValidationError[] errors)
            where TResult : Result
        {
            if (typeof(TResult) == typeof(Result))
            {
                return (DomainValidationResult<TResult>.WithErrors(errors) as TResult)!;
            }

            object validationResult = typeof(DomainValidationResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(Domain.Abstract.DomainValidationResult.WithErrors))!
                .Invoke(null, new object?[] { errors })!;

            return (TResult)validationResult;
        }
    }
}
