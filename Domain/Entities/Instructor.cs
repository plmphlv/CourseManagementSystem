using Domain.Common;

namespace Domain.Entities
{
    public class Instructor : AuditableEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        public ICollection<Session> SubstituteSessions { get; set; } = new List<Session>();
    }
}
