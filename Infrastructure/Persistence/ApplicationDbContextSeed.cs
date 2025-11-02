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
            User? instructor = await userManager.FindByEmailAsync("instructor@courses.com");

            if (instructor is null)
            {
                return;
            }

            User? user = await userManager.FindByEmailAsync("testuser@courses.com");

            if (user is null)
            {
                return;
            }

            Instructor? instructorAccount = await context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.UserId == instructor.Id);

            if (instructorAccount is null)
            {
                return;
            }

            Course course1 = new Course
            {
                Name = "Fundamentals of Programming",
                Description = "An introductory course to basic programming concepts using C#.",
                MemberLimit = 25,
                InstructorId = 1,
                StartDate = new DateTime(2025, 1, 10),
                EndDate = new DateTime(2025, 3, 10)
            };

            List<Session> sessions1 = [
                    new Session
                    {
                        Course = course1,
                        InstructorId = 1,
                        ScheduledTime = new DateTime(2025, 1, 10, 9, 0, 0),
                        DurationMinutes = 90,
                        IsConfirmed = true,
                        Notes = "Introduction to variables and data types."
                    },
                    new Session
                    {
                        Course = course1,
                        InstructorId = 1,
                        ScheduledTime = new DateTime(2025, 1, 17, 9, 0, 0),
                        DurationMinutes = 90,
                        IsConfirmed = true,
                        Notes = "Control structures and loops."
                    },
                    new Session
                    {
                        Course = course1,
                        InstructorId = 1,
                        ScheduledTime = new DateTime(2025, 1, 24, 9, 0, 0),
                        DurationMinutes = 90,
                        IsConfirmed = true,
                        Notes = "Methods and functions."
                    },
            ];

            course1.Sessions = sessions1;

            Course course2 = new Course
            {
                Name = "Advanced Object-Oriented Programming",
                Description = "Deep dive into OOP principles, patterns, and best practices in C#.",
                MemberLimit = 18,
                InstructorId = 1,
                StartDate = new DateTime(2025, 4, 1),
                EndDate = new DateTime(2025, 7, 1)
            };

            List<Session> sessions2 = [
                new Session
                {
                    Course= course2,
                    InstructorId = 1,
                    ScheduledTime = new DateTime(2025, 4, 1, 9, 30, 0),
                    DurationMinutes = 120,
                    IsConfirmed = true,
                    Notes = "Interfaces and abstract classes."
                },
                new Session
                {
                    Course= course2,
                    InstructorId = 1,
                    ScheduledTime = new DateTime(2025, 4, 8, 9, 30, 0),
                    DurationMinutes = 120,
                    IsConfirmed = true,
                    Notes = "SOLID principles deep dive."
                },
                new Session
                {
                    Course= course2,
                    InstructorId = 1,
                    ScheduledTime = new DateTime(2025, 4, 15, 9, 30, 0),
                    DurationMinutes = 120,
                    IsConfirmed = true,
                    Notes = "Whem breaking the rules is more beneficial."
                }
            ];
            course2.Sessions = sessions2;

            await context.AddRangeAsync(course1, course2);
            await context.SaveChangesAsync();

            CourseMember member = new CourseMember
            {
                MemberId = user.Id,
                CourseId = course2.Id,
            };

            await context.AddAsync(member);
            await context.SaveChangesAsync();

            List<Schedule> schedules = [
                new Schedule
                {
                     IsActive = true,
                     ScheduleDate =new DateTime(2025, 4, 1, 9, 30, 0),
                     AccountId = user.Id,
                     SessionId = sessions2[0].Id
                },
                new Schedule
                {
                     IsActive = false,
                     ScheduleDate =new DateTime(2025, 4, 8, 9, 30, 0),
                     AccountId = user.Id,
                     SessionId = sessions2[1].Id
                },
                new Schedule
                {
                     IsActive = true,
                     ScheduleDate =new DateTime(2025, 4, 15, 9, 30, 0),
                     AccountId = user.Id,
                     SessionId = sessions2[2].Id
                }
            ];

            await context.Schedules.AddRangeAsync(schedules);
            await context.SaveChangesAsync();
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
                        if (string.Equals(role, Role.Instructor.ToString()))
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
