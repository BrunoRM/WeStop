using FluentValidation;
using WeStop.Application.Commands;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class RegisterPlayerCommandValidator : AbstractValidator<RegisterPlayerCommand>
    {
        public RegisterPlayerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("name is required");
        }    
    }
}
