using FluentValidation;
using WeStop.Application.Dtos.Player;

namespace WeStop.Application.Validators.Player
{
    class RegisterPlayerDtoValidator : AbstractValidator<RegisterPlayerDto>
    {
        public RegisterPlayerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
