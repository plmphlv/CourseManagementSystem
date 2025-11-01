namespace Application.Users.Commands.Common
{
    public class AuthenticationResponse
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public bool IsSuccessful { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
