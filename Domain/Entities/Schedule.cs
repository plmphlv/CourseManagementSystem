using Domain.Common;

namespace Domain.Entities
{
    public class Schedule : AuditableEntity
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime ScheduleDate { get; set; }

        public string AccountId { get; set; } = null!;

        public User Account { get; set; } = null!;

        public int SessionId { get; set; }

        public Session Session { get; set; } = null!;
    }
}