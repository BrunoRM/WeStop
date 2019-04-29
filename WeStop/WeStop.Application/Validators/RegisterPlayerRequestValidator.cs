using FluentValidation;
using WeStop.Application.Commands;

namespace WeStop.Application.Validators
{
    public class RegisterPlayerRequestValidator : AbstractValidator<RegisterPlayerRequest>
    {
        public RegisterPlayerRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("name is required");
        }    
    }
}
