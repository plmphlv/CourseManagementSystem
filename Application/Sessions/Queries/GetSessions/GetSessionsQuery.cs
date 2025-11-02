namespace Application.Sessions.Queries.GetSessions
{
    public class GetSessionsQuery : IRequest<List<SessionDto>>
    {
        public int? CourseId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsConfirmed { get; set; }

        public int? InstructorId { get; set; }

        public int? SubstituteInstructorId { get; set; }

    }

    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<SessionDto>>
    {

        private readonly IApplicationDbContext context;

        public GetSessionsQueryHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Session> query = context.Sessions;


            bool? isConfirmed = request.IsConfirmed;

            if (isConfirmed.HasValue)
            {
                query = query.Where(s => s.IsConfirmed == isConfirmed);
            }

            DateTime? startDate = request.StartDate;

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ScheduledTime >= startDate);
            }

            DateTime? endDate = request.EndDate;

            if (endDate.HasValue)
            {
                query = query.Where(s => s.ScheduledTime >= endDate);
            }

            int? courseId = request.CourseId;

            if (courseId.HasValue)
            {
                query = query.Where(s => s.CourseId == courseId);
            }

            int? instructorId = request.InstructorId;

            if (instructorId.HasValue)
            {
                query = query.Where(s => s.InstructorId == instructorId);
            }

            int? substituteInstructorId = request.SubstituteInstructorId;

            if (substituteInstructorId.HasValue)
            {
                query = query.Where(s => s.SubstituteInstructorId == substituteInstructorId);
            }

            List<SessionDto> sessions = await query.Select(s => new SessionDto
            {
                Id = s.Id,
                ScheduledTime = s.ScheduledTime,
                DurationMinutes = s.DurationMinutes,
                IsConfirmed = s.IsConfirmed,
            }).ToListAsync(cancellationToken);

            return sessions;
        }
    }
}
