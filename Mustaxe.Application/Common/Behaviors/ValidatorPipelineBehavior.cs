using FluentValidation;
using MediatR;
using Mustaxe.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mustaxe.Application.Common.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) 
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) 
        {
            var fails = _validators
                .Select(validador => validador.Validate(request))
                .SelectMany(result => result.Errors)
                .ToArray();

            if (fails.Length > 0) 
            {
                var errors = fails
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(k => k.Key, v => v.Select(x => x.ErrorMessage).ToArray());

                throw new InputValidationException(errors);
            }

            return next();
        }
    }
}
