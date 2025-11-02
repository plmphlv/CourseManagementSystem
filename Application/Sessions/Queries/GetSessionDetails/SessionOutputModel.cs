using Application.Sessions.Common;

namespace Application.Sessions.Queries.GetSessionDetails
{
    public class SessionOutputModel: SessionModel
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = null!;

        public string InstructorName { get; set; } = null!;

        public string? SubstituteInstructorName { get; set; }
    }
}