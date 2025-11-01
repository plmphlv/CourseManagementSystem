using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

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
                    //If result is unsuccessful log information after implementing logging
                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedDevelopmentData(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            //await SeedUsers(context, userManager, configuration);
            await SeedSampleData(context, userManager);

        }

        private static async Task SeedSampleData(ApplicationDbContext context,UserManager<User> userManager)
        {

        }
    }
}
