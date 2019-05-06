using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using WeStop.Domain.Errors;
using WeStop.Domain.Exceptions;

namespace WeStop.Application.Exceptions
{
    public class ValidationException : WeStopException
    {
        public ValidationException()    
            :base(CommonErrors.InvalidRequest)
        {
            Failures = new Dictionary<string, string[]>();
        }
        
        public ValidationException(List<ValidationFailure> failures) : this()
        {
            var propertyNames = failures
                .Select(e => e.PropertyName.ToLower())
                .Distinct();

            foreach (var propertyName in propertyNames)
            {
                var propertyFailures = failures
                    .Where(e => e.PropertyName.ToLower() == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                Failures.Add(propertyName, propertyFailures);
            }
        }

        public IDictionary<string, string[]> Failures { get; }
    }
}