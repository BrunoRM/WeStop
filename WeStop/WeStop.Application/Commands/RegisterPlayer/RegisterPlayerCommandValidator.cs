using FluentValidation;
using WeStop.Domain.Errors;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class RegisterPlayerCommandValidator : AbstractValidator<RegisterPlayerCommand>
    {
        public RegisterPlayerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(PlayerErrors.InvalidNameRequired)
                .MaximumLength(35).WithMessage(PlayerErrors.InvalidNameMaxLength);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(PlayerErrors.InvalidEmailRequired)
                .EmailAddress().WithMessage(PlayerErrors.InvalidEmail);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(PlayerErrors.InvalidUserNameRequired)
                .MaximumLength(20).WithMessage(PlayerErrors.InvalidUserNameMaxLength);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(PlayerErrors.InvalidPasswordRequired)
                .MinimumLength(5).WithMessage(PlayerErrors.InvalidPasswordMinLength);
        }    
    }
}
