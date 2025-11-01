using Application.Users.Commands.Common;

namespace Application.Users.Commands.Login
{
    public class LoginCommand : IRequest<AuthenticationResponse>
    {
        public string IdentifyingCredential { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResponse>
    {
        private readonly IIdentityService identityService;
        private readonly IJwtManager jwtManager;

        public LoginCommandHandler(IJwtManager jwtManager, IIdentityService identityService)
        {
            this.jwtManager = jwtManager;
            this.identityService = identityService;
        }

        public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            string userIdentifier = request.IdentifyingCredential;
            string password = request.Password;

            bool isValidLogin = await identityService.ValidateLoginAsync(userIdentifier, password, cancellationToken);

            if (!isValidLogin)
            {
                return new AuthenticationResponse
                {
                    IsSuccessful = false,
                    ErrorMessage = UserMessages.InvalidUser
                };
            }

            string accessToken = await jwtManager.GenerateAccessTokenAsync(userIdentifier);
            string refreshToken = await jwtManager.GenerateRefreshTokenAsync(userIdentifier);

            return new AuthenticationResponse
            {
                IsSuccessful = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
