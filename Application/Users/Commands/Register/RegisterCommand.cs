using FluentValidation.Results;

namespace Application.Users.Commands.Register
{
    public class RegisterCommand : IRequest
    {
        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IIdentityService identityService;

        public RegisterCommandHandler(IIdentityService identityService)
        {
            this.identityService = identityService;
        }


        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            Result result = await identityService.CreateUserAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                throw new ValidationException(
                    result.Errors.Select(e => new ValidationFailure(nameof(RegisterCommand), e)));
            }
        }
    }
}
