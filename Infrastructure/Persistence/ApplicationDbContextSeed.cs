using Domain.Entities;
using Domain.Enums;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            IEnumerable<string> roles = Enum.GetNames(typeof(Role));

            foreach (string role in roles)
            {
                bool isExistingRole = await roleManager.RoleExistsAsync(role);

                if (!isExistingRole)
                {
                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedDevelopmentData(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            await SeedUsers(context, userManager, configuration);
            await SeedSampleData(context, userManager);

        }

        private static async Task SeedSampleData(ApplicationDbContext context, UserManager<User> userManager)
        {

        }

        public static async Task SeedUsers(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            IConfiguration? userData = configuration.GetSection("SeedingUserData");

            if (userData is null)
            {
                return;
            }

            List<AccountSettings>? accountSettings = userData.Get<List<AccountSettings>>();

            if (accountSettings is null)
            {
                return;
            }

            foreach (AccountSettings setting in accountSettings)
            {
                await CreateUser(userManager, context, setting);
            }
        }

        private static async Task CreateUser(
        UserManager<User> userManager,
        ApplicationDbContext context,
        AccountSettings settings)
        {
            string firstName = settings.FirstName;
            string lastName = settings.LastName;
            string username = settings.Username;
            string email = settings.Email;
            string password = settings.Password;
            string? role = settings.Role;

            bool isExistingUser = await userManager.Users
                .AnyAsync(u => u.UserName == username);

            if (!isExistingUser)
            {
                User user = new User
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return;
                }

                await context.SaveChangesAsync();

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                };


                if (!string.IsNullOrWhiteSpace(role))
                {
                    bool isValidRole = Enum.TryParse(role, true, out Role roleValue);

                    if (isValidRole)
                    {
                        if (string.Equals(role, Role.Instructor))
                        {
                            Instructor instructor = new Instructor
                            {
                                UserId = user.Id
                            };

                            context.Instructors.Add(instructor);

                            await context.SaveChangesAsync();

                            claims.Add(new Claim(InfrastructureConstants.InstructorId, instructor.Id.ToString()));
                        }

                        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);

                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                await userManager.AddClaimsAsync(user, claims);
            }
        }
    }
}
