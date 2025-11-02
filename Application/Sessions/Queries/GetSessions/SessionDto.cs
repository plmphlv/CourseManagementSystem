namespace Application.Sessions.Queries.GetSessions
{
    public class SessionDto
    {
        public int Id { get; set; }

        public DateTime ScheduledTime { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsConfirmed { get; set; }
    }
}