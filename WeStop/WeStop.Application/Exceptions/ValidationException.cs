using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using WeStop.Application.Errors;

namespace WeStop.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Error = CommonErrors.InvalidRequest;
            Failures = new Dictionary<string, string[]>();
        }
        
        public ValidationException(List<ValidationFailure> failures)
        {
            var propertyNames = failures
                .Select(e => e.PropertyName)
                .Distinct();

            foreach (var propertyName in propertyNames)
            {
                var propertyFailures = failures
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                Failures.Add(propertyName, propertyFailures);
            }
        }

        public IDictionary<string, string[]> Failures { get; }
        public string Error { get; private set; }
    }
}