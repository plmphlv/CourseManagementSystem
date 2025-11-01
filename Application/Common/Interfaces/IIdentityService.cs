using Application.Common.Models;
using Application.Users.Commands.Register;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> ValidateLoginAsync(string identityCredential, string password, CancellationToken cancellationToken);

        Task<User?> FindUserByIdAsync(string id);

        Task<IEnumerable<Claim>> GetUserClaimsAsync(string identityCredential, CancellationToken cancellationToken);

        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

        Task<Result> AddRoleAsync(string userId, string role, CancellationToken cancellationToken);

        Task<Result> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken);

        Task<Result> CreateUserAsync(RegisterCommand command, CancellationToken cancellationToken);
    }
}
