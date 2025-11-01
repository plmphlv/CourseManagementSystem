using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public Instructor? Instructor { get; set; }

        public ICollection<CourseMember> Courses { get; set; } = new List<CourseMember>();

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
