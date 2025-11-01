using Domain.Common;

namespace Domain.Entities
{
    public class Course : AuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? MemberLimit { get; set; }

        public int InstructorId { get; set; }

        public Instructor Instructor { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<CourseMember> Members { get; set; } = new List<CourseMember>();

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
