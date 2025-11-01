namespace Application.Users.Commands.ChangePassword
{
    internal class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(c => c.CurrentPassword)
                .NotEmpty()
                .WithMessage("Enter current password")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long");

            RuleFor(c => c.NewPassword)
                .NotEmpty()
                .WithMessage("Enter new password")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .NotEqual(c => c.CurrentPassword)
                .WithMessage("New password cannot be the same as the current password");

            RuleFor(c => c.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Enter new password")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .Equal(c => c.NewPassword)
                .WithMessage("Passwords do not match")
                .NotEqual(c => c.CurrentPassword)
                .WithMessage("New password cannot be the same as the current password");
        }

    }
}
