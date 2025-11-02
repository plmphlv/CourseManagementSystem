namespace Application.Sessions.Common
{
    public class SessionModel
    {
        public DateTime ScheduledTime { get; set; }

        public int DurationMinutes { get; set; }

        public string? Notes { get; set; }

        public int CourseId { get; set; }

        public int InstructorId { get; set; }

        public int? SubstituteInstructorId { get; set; }
    }
}
