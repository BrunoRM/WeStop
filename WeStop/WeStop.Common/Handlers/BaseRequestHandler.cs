using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeStop.Common.Handlers
{
    public class BaseRequestHandler<TRequest>
    {
        private readonly IValidator<TRequest> _requestValidator;

        public BaseRequestHandler()
        {
            // Busca a classe de validação
            var assembly = AppDomain.CurrentDomain.Load("WeStop.Application");

            var assemblyScannerResult = AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .FirstOrDefault(x => x.InterfaceType == typeof(IValidator<TRequest>));

            if (assemblyScannerResult != null)
            {
                var constructorInfo = assemblyScannerResult.ValidatorType.GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    var instance = constructorInfo.Invoke(new object[] { });
                    _requestValidator = (IValidator<TRequest>) instance;
                }
            }
        }

        private ICollection<ValidationError> _validationErrors;

        public IReadOnlyCollection<ValidationError> RequestValidationErrors { get { return _validationErrors.ToList(); } }

        public async Task<bool> ValidateRequestAsync(TRequest request)
        {
            if (_requestValidator != null)
            {
                var validationResult = await _requestValidator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    _validationErrors = validationResult.Errors.Select(x => new ValidationError
                    {
                        Field = x.PropertyName,
                        Msg = x.ErrorMessage
                    }).ToList();

                    return false;
                }
                else
                    return true;
            }

            return true;
        }
    }
}
