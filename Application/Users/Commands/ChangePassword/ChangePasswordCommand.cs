using Application.Users.Commands.Register;
using FluentValidation.Results;

namespace Application.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public string CurrentPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;

        public string ConfirmNewPassword { get; set; } = null!;
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUserService;

        public ChangePasswordCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService)
        {
            this.currentUserService = currentUserService;
            this.identityService = identityService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            string? userIdentifier = currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                throw new ForbiddenAccessException();
            }

            Result result = await identityService.ChangePasswordAsync(userIdentifier, request.CurrentPassword, request.NewPassword, cancellationToken);

            if (!result.Succeeded)
            {
                throw new ValidationException(
                    result.Errors.Select(e => new ValidationFailure(nameof(RegisterCommand), e)));
            }
        }
    }
}
