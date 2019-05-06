using FluentValidation;
using WeStop.Application.Commands.RegisterPlayer;
using WeStop.Domain.Errors;

namespace WeStop.Application.Commands.CreateGameRoom
{
    public class CreateGameRoomCommandValidator : AbstractValidator<CreateGameRoomCommand>
    {
        public CreateGameRoomCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(GameRoomErrors.InvalidNameRequired)
                .MaximumLength(15).WithMessage(GameRoomErrors.InvalidNameMaxLength);

            RuleFor(x => x.Password)
                .MinimumLength(4).WithMessage(GameRoomErrors.InvalidPasswordMinLength)
                .MaximumLength(6).WithMessage(GameRoomErrors.InvalidPasswordMaxLength);

            RuleFor(x => x.NumberOfRounds)
                .NotEmpty().WithMessage(GameRoomErrors.InvalidNumberOfRoundsRequired)
                .GreaterThanOrEqualTo(3).WithMessage(GameRoomErrors.InvalidNumberOfRoundsMin)
                .LessThanOrEqualTo(10).WithMessage(GameRoomErrors.InvalidNumberOfRoundsMax);

            RuleFor(x => x.NumberOfPlayers)
                .NotEmpty().WithMessage(GameRoomErrors.InvalidNumberOfPlayersRequired)
                .GreaterThanOrEqualTo(2).WithMessage(GameRoomErrors.InvalidNumberOfPlayersMin)
                .LessThanOrEqualTo(10).WithMessage(GameRoomErrors.InvalidNumberOfPlayersMax);
        }
    }
}
