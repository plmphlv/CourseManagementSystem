using Application.Users.Commands.Common;

namespace Application.Users.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthenticationResponse>
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResponse>
    {
        private readonly IJwtManager jwtManager;
        private readonly ICurrentUserService currentUserService;

        public RefreshTokenCommandHandler(IJwtManager jwtManager, ICurrentUserService currentUserService)
        {
            this.jwtManager = jwtManager;
            this.currentUserService = currentUserService;
        }

        public async Task<AuthenticationResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            string token = request.RefreshToken;

            bool isValidRefreshToken = await jwtManager.IsValidRefreshTokenAsync(currentUserService.UserId!, token);

            if (!isValidRefreshToken)
            {
                return new AuthenticationResponse()
                {
                    ErrorMessage = UserMessages.InvalidRefreshToken,
                    IsSuccessful = false,
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty
                };
            }

            string userName = currentUserService.UserName!;

            string accessToken = await jwtManager.GenerateAccessTokenAsync(userName);
            string refreshToken = await jwtManager.GenerateRefreshTokenAsync(userName);

            return new AuthenticationResponse()
            {
                IsSuccessful = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ErrorMessage = string.Empty
            };
        }
    }
}
