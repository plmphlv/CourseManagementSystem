using Domain.Common;

namespace Domain.Entities
{
    public class Session : AuditableEntity
    {
        public int Id { get; set; }

        public DateTime ScheduledTime { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsConfirmed { get; set; }

        public string? Notes { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public int InstructorId { get; set; }

        public Instructor Instructor { get; set; } = null!;

        public int? SubstituteInstructorId { get; set; }

        public Instructor? SubstituteInstructor { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}