using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Common.AppServices
{
    public abstract class ApplicationServiceBase
    {
        protected Dictionary<string, ICollection<string>> ValidationErrors { get; set; }

        protected async Task<bool> IsValidAsync<TDto, TValidator>(TDto dto) where TValidator: AbstractValidator<TDto>, new()
        {
            if (dto is null)
                return true;

            var validationResult = await new TValidator().ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var validationError in validationResult.Errors)
                {
                    if (ValidationErrors.ContainsKey(validationError.PropertyName))
                        ValidationErrors[validationError.PropertyName].Add(validationError.ErrorMessage);
                    else
                        ValidationErrors.Add(validationError.PropertyName, new List<string> { validationError.ErrorMessage });
                }
            }

            return true;
        }

        protected Task<Result<T>> Ok<T>(string message) => 
            Task.FromResult(new Result<T>(message));

        protected Task<Result<T>> Ok<T>(string message, T data) =>
            Task.FromResult(new Result<T>(message, data));

        protected Task<Result<T>> Error<T>(ERRORS error) => 
            Task.FromResult(new Result<T>(error));

        protected Task<Result<T>> RequestValidationErrors<T>() => 
            Task.FromResult(new Result<T>(ValidationErrors));
    }
}
