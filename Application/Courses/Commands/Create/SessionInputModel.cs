namespace Application.Courses.Commands.Create
{
    public class SessionInputModel
    {
        public string? Notes { get; set; }

        public DateTime ScheduledDate { get; set; }

        public int DurationMinutes { get; set; }
    }
}