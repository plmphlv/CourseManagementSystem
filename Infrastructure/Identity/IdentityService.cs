using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Commands.Register;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly IApplicationDbContext context;

        public IdentityService(UserManager<User> userManager, IApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<bool> ValidateLoginAsync(string userIdentifier, string password, CancellationToken cancellationToken)
        {
            User? user = await FindUserByUsernameOrEmailAsync(userIdentifier);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userIdentifier);
            }

            bool isValidPassword = await userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
            {
                await userManager.AccessFailedAsync(user);
            }


            //bool isEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

            //if (!isEmailConfirmed)
            //{
            //    throw new ValidationException("Email is not confirmed");
            //}

            return isValidPassword;
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string userIdentifier, CancellationToken cancellationToken)
        {
            User? user = await FindUserByUsernameOrEmailAsync(userIdentifier);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userIdentifier);
            }

            IEnumerable<Claim> claims = await userManager.GetClaimsAsync(user);

            return claims;
        }

        private async Task<User?> FindUserByUsernameOrEmailAsync(string userIdentifier)
        {
            User? user = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == userIdentifier || u.Email == userIdentifier);

            return user;
        }

        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            User? user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            IdentityResult result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.ToApplicationResult();
        }

        public async Task<Result> AddRoleAsync(string userId, string role, CancellationToken cancellationToken)
        {
            User? user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                return roleResult.ToApplicationResult();
            }

            IdentityResult claimsResult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));

            if (!claimsResult.Succeeded)
            {
                await userManager.RemoveFromRoleAsync(user, role);

                return claimsResult.ToApplicationResult();
            }

            return Result.Success();
        }

        public async Task<Result> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken)
        {
            User? user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            IdentityResult roleResult = await userManager
                .RemoveFromRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                return roleResult.ToApplicationResult();
            }

            IdentityResult claimsResult = await userManager
                .RemoveClaimAsync(user, new Claim(ClaimTypes.Role, role));

            if (!claimsResult.Succeeded)
            {
                await userManager.RemoveFromRoleAsync(user, role);

                return claimsResult.ToApplicationResult();
            }

            return Result.Success();
        }

        public async Task<Result> CreateUserAsync(RegisterCommand model, CancellationToken cancellationToken)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            IdentityResult result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return result.ToApplicationResult();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            IdentityResult claimsResult = await userManager.AddClaimsAsync(user, claims);

            if (!claimsResult.Succeeded)
            {
                return claimsResult.ToApplicationResult();
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<User?> FindUserByIdAsync(string id)
        {
            User? user = await userManager.FindByIdAsync(id);

            return user;
        }
    }
}
