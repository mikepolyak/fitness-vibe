using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessVibe.Application.Behaviors
{
    /// <summary>
    /// Validation Behavior - the personal trainer that checks your form before you start.
    /// This ensures every command and query meets our fitness app's standards before execution,
    /// just like how a good trainer won't let you lift with bad form.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of the behavior
        /// </summary>
        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the request by validating it first
        /// </summary>
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var requestTypeName = typeof(TRequest).Name;
                
                _logger.LogWarning(
                    "Validation failed for {RequestType}. Errors: {Errors}",
                    requestTypeName,
                    string.Join("; ", failures.Select(f => f.ErrorMessage)));

                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
