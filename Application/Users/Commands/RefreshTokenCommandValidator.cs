using Application.Users.Commands.RefreshToken;
using FluentValidation;

namespace Application.Users.Commands
{
    internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(c => c.RefreshToken)
           .NotEmpty()
           .WithMessage("Refresh token is required");
        }
    }
}
