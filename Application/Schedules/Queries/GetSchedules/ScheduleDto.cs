namespace Application.Schedules.Queries.GetSchedules
{
    public class ScheduleDto
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public DateTime ScheduleDate { get; set; }

        public bool IsConfirmed { get; set; }
    }
}